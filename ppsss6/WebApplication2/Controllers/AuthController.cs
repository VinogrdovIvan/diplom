using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Repositories;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly JwtService _jwtService;

    public AuthController(
        IUserRepository userRepository,
        IRepository<Session> sessionRepository,
        JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _sessionRepository = sessionRepository;
    }

    // Регистрация нового пользователя.
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            if (await _userRepository.ExistsAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { Message = "Пользователь с таким адресом электронной почты уже существует." });
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Password = _jwtService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                RoleId = 1
            };

            await _userRepository.AddAsync(user);
            return Ok(new { Message = "Пользователь успешно зарегистрирован." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при регистрации пользователя.", Error = ex.Message });
        }
    }

    // Вход в систему.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var user = await _userRepository.GetByUsernamaeWithRole(request.Email);
            if (user == null || !_jwtService.VerifyPassword(request.Password, user.Password))
            {
                return Unauthorized(new { Message = "Неверный адрес электронной почты или пароль." });
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var session = new Session()
            {
                UserId = user.UserId,
                Token = refreshToken,
                Expires = DateTime.Now.AddDays(7)
            };
            await _sessionRepository.AddAsync(session);

            return Ok(new TokenResponse() { AccessToken = accessToken, RefreshToken = refreshToken });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при входе в систему.", Error = ex.Message });
        }
    }

    // Обновление токена доступа.
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { Message = "Требуется refresh token." });
            }

            var session = (await _sessionRepository.GetAllAsync())
                .FirstOrDefault(s => s.Token == request.RefreshToken);

            if (session == null)
            {
                return Unauthorized(new { Message = "Недействительный refresh token." });
            }

            if (session.Expires < DateTime.Now)
            {
                return Unauthorized(new { Message = "Срок действия refresh token истек." });
            }

            var user = await _userRepository.GetByIdWithRoleAsync(session.UserId);
            if (user == null)
            {
                return Unauthorized(new { Message = "Пользователь не найден." });
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            session.Token = newRefreshToken;
            session.Expires = DateTime.Now.AddDays(7);
            await _sessionRepository.UpdateAsync(session);

            return Ok(new TokenResponse() { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при обновлении токена.", Error = ex.Message });
        }
    }

    // Проверка текущего пароля.
    [HttpPost("verify-password")]
    [Authorize] 
    public async Task<IActionResult> VerifyPassword(VerifyPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Некорректные данные.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден." });
            }

            var isCorrectOldPassword = _jwtService.VerifyPassword(request.CurrentPassword, user.Password);
            if (!isCorrectOldPassword)
            {
                return BadRequest("Неправильный старый пароль!");
            }

            return Ok(new { IsPasswordValid = isCorrectOldPassword });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Произошла ошибка при проверке пароля.", Error = ex.Message });
        }
    }
}