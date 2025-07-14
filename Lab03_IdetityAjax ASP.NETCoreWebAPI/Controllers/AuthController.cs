using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Entities;
using BusinessObjects.Models.Accounts;
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountDAO _accountDao;
        private readonly IConfiguration _config;

        public AuthController(IAccountDAO accountDao, IConfiguration config)
        {
            _accountDao = accountDao;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var acct = await _accountDao.GetByEmailAsync(req.Email);
            if (acct == null ||
                !BCrypt.Net.BCrypt.Verify(req.Password, acct.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,   acct.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, acct.Email),
                new Claim(ClaimTypes.Role,
                          ((RoleType)acct.RoleId).ToString())
            };

            // Create JWT
            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponse
            {
                Token = tokenString,
                Expires = expires,
                Role = ((RoleType)acct.RoleId).ToString()
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            // 1) Passwords match?
            if (req.Password != req.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            // 2) Email available?
            var existing = await _accountDao.GetByEmailAsync(req.Email);
            if (existing is not null)
                return BadRequest("Email is already in use.");

            // 3) Create & hash with BCrypt
            var acct = new Account
            {
                AccountName = req.AccountName,
                Email = req.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = Constants.RoleCustomer
            };

            await _accountDao.InsertAsync(acct);
            await _accountDao.SaveAsync();

            return Ok("Registration successful.");
        }
    }
}
