using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LAB06_RodrigoLupo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LAB06_RodrigoLupo.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
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