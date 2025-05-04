using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CarRental.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public UsersController(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "Пользователи не найдены." });
            }

            var userResponses = users.Select(user => new UserResponse(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role?.RoleName ?? "Роль не указана",
                user.Password
            )).ToList();

            return Ok(userResponses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении списка пользователей.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID пользователя должен быть положительным числом." });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден." });
            }

            var userResponse = new UserResponse(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role?.RoleName ?? "Роль не указана",
                user.Password
            );

            return Ok(userResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при получении пользователя.",
                Error = ex.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные пользователя.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
                return BadRequest(new { Message = "Имя обязательно." });

            if (string.IsNullOrWhiteSpace(user.LastName))
                return BadRequest(new { Message = "Фамилия обязательна." });

            if (string.IsNullOrWhiteSpace(user.Email) || !new EmailAddressAttribute().IsValid(user.Email))
                return BadRequest(new { Message = "Некорректный email." });

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8)
                return BadRequest(new { Message = "Пароль должен содержать минимум 8 символов." });

            if (await _userRepository.ExistsAsync(u => u.Email == user.Email))
            {
                return Conflict(new { Message = "Пользователь с таким email уже существует." });
            }

            user.Password = _jwtService.HashPassword(user.Password);

            await _userRepository.AddAsync(user);

            var userResponse = new UserResponse(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role?.RoleName ?? "Роль не указана",
                user.Password
            );

            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, userResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при создании пользователя.",
                Error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { Message = "ID пользователя должен быть положительным числом." });

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Некорректные данные пользователя.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            if (id != user.UserId)
            {
                return BadRequest(new
                {
                    Message = "Идентификатор пользователя в URL не совпадает с идентификатором в теле запроса."
                });
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
                return BadRequest(new { Message = "Имя обязательно." });

            if (string.IsNullOrWhiteSpace(user.LastName))
                return BadRequest(new { Message = "Фамилия обязательна." });

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = "Пользователь не найден." });
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;

            if (!string.IsNullOrEmpty(user.Password))
            {
                if (user.Password.Length < 8)
                    return BadRequest(new { Message = "Пароль должен содержать минимум 8 символов." });

                existingUser.Password = _jwtService.HashPassword(user.Password);
            }

            await _userRepository.UpdateAsync(existingUser);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при обновлении пользователя.",
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
                return BadRequest(new { Message = "ID пользователя должен быть положительным числом." });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден." });
            }

            await _userRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Произошла ошибка при удалении пользователя.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == 0)
                return Unauthorized(new { Message = "Пользователь не авторизован." });

            var user = await _userRepository.GetByIdWithRoleAsync(userId);
            if (user == null)
                return NotFound(new { Message = "Пользователь не найден." });

            var userResponse = new UserResponse(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role?.RoleName ?? "Роль не указана",
                user.Password
            );

            return Ok(userResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Ошибка при получении данных пользователя.",
                Error = ex.Message
            });
        }
    }

    [HttpPut("me/update")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserRequest request)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == 0)
                return Unauthorized(new { Message = "Пользователь не авторизован." });

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "Пользователь не найден." });

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                if (string.IsNullOrWhiteSpace(request.FirstName))
                    return BadRequest(new { Message = "Имя не может быть пустым." });
                user.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                if (string.IsNullOrWhiteSpace(request.LastName))
                    return BadRequest(new { Message = "Фамилия не может быть пустой." });
                user.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                if (!new EmailAddressAttribute().IsValid(request.Email))
                    return BadRequest(new { Message = "Некорректный email." });
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone;

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                if (string.IsNullOrEmpty(request.CurrentPassword))
                    return BadRequest(new { Message = "Текущий пароль обязателен." });

                if (!_jwtService.VerifyPassword(request.CurrentPassword, user.Password))
                    return BadRequest(new { Message = "Неверный текущий пароль." });

                if (request.NewPassword.Length < 8)
                    return BadRequest(new { Message = "Новый пароль должен содержать минимум 8 символов." });

                user.Password = _jwtService.HashPassword(request.NewPassword);
            }

            await _userRepository.UpdateAsync(user);

            return Ok(new { Message = "Данные пользователя успешно обновлены." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Ошибка при обновлении данных.",
                Error = ex.Message
            });
        }
    }
}