using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LAB06_RodrigoLupo.Models;
using LAB06_RodrigoLupo.Repository.Unit;
using LAB06_RodrigoLupo.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LAB06_RodrigoLupo.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtService _tokenService;

    public AuthController(IConfiguration configuration,IUnitOfWork unitOfWork, JwtService tokenService)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }
    
    [HttpPost("login-mejorado")]
    public async Task<IActionResult> LoginMejorado([FromBody] LoginModel model)
    {
        var users = await _unitOfWork.Repository<User>()
            .GetByStringProperty(
                propertyName: "Username",
                value: model.UserName,
                include: q => q.Include(u => u.Roles)
            );

        var user = users.FirstOrDefault();

        if (user == null || user.Password != model.Password)
            return Unauthorized(new { message = "Credenciales inválidas" });

        var role = user.Roles.FirstOrDefault()?.Name ?? "User";

        var token = _tokenService.GenerateToken(user.Username, role);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (model.UserName == "admin" && model.Password == "admin")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        return Unauthorized();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult GetAdminData()
    {
        return Ok("Datos solo para administradores");
    }
    
}