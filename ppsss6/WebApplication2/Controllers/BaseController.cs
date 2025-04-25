using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DbContexts;
using WebApplication2.Entities;

namespace WebApplication2.Controllers
{

    // Для CRUD-операций.

    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase where T : class
    {
        protected readonly TestdbContext _dbContext;

        public BaseController(TestdbContext dbContext)
        {
            _dbContext = dbContext;
        }


        ///Получить все сущности.

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entities = await _dbContext.Set<T>().ToListAsync();
                if (entities == null || !entities.Any())
                {
                    return NotFound(new { Message = "Данные не найдены." });
                }

                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при получении данных.", Error = ex.Message });
            }
        }


        // Получить сущность по идентификатору.

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var entity = await _dbContext.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { Message = "Сущность не найдена." });
                }

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при получении сущности.", Error = ex.Message });
            }
        }

        // Создать новую сущность.

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Некорректные данные.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                await _dbContext.Set<T>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var id = (int)entity.GetType().GetProperty("Id").GetValue(entity);

                return CreatedAtAction(nameof(GetById), new { id }, entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при создании сущности.", Error = ex.Message });
            }
        }


        // Обновить существующую сущность.

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Некорректные данные.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity);

                if (id != entityId)
                {
                    return BadRequest(new { Message = "Несоответствие идентификационных данных." });
                }

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.Set<T>().AnyAsync(e => (int)e.GetType().GetProperty("Id").GetValue(e) == id))
                {
                    return NotFound(new { Message = "Сущность не найдена." });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при обновлении сущности.", Error = ex.Message });
            }
        }


        // Удалить сущность по идентификатору.

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _dbContext.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { Message = "Сущность не найдена." });
                }

                _dbContext.Set<T>().Remove(entity);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Произошла ошибка при удалении сущности.", Error = ex.Message });
            }
        }
    }
}