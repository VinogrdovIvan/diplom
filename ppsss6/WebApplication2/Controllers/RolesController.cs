using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRepository<Role> _roleRepository;

    public RolesController(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync();
            if (roles == null || !roles.Any())
            {
                return NotFound(new { Message = "Роли не найдены." });
            }

            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении списка ролей.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID роли должен быть положительным числом." });

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { Message = "Роль не найдена." });
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении роли.",
                Error = ex.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Role role)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные роли.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (string.IsNullOrWhiteSpace(role.RoleName))
                return BadRequest(new { Message = "Название роли обязательно." });

            if (role.RoleName.Length > 50)
                return BadRequest(new { Message = "Название роли не должно превышать 50 символов." });

            if (await _roleRepository.ExistsAsync(r => r.RoleName == role.RoleName))
            {
                return Conflict(new { Message = "Роль с таким именем уже существует." });
            }

            await _roleRepository.AddAsync(role);

            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при создании роли.",
                Error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID роли должен быть положительным числом." });

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные роли.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (id != role.RoleId)
            {
                return BadRequest(new
                {
                    Message = "Идентификатор роли в URL не совпадает с идентификатором в теле запроса."
                });
            }

            if (string.IsNullOrWhiteSpace(role.RoleName))
                return BadRequest(new { Message = "Название роли обязательно." });

            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(new { Message = "Роль не найдена." });
            }

            if (existingRole.RoleName != role.RoleName &&
                await _roleRepository.ExistsAsync(r => r.RoleName == role.RoleName))
            {
                return Conflict(new { Message = "Роль с таким именем уже существует." });
            }

            existingRole.RoleName = role.RoleName;
            existingRole.Description = role.Description;

            await _roleRepository.UpdateAsync(existingRole);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при обновлении роли.",
                Error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID роли должен быть положительным числом." });

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { Message = "Роль не найдена." });
            }

            await _roleRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при удалении роли.",
                Error = ex.Message
            });
        }
    }
}