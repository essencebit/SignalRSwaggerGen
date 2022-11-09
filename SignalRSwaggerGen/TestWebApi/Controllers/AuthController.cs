using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestWebApi.Controllers
{
	[AllowAnonymous]
	[ApiExplorerSettings(GroupName = "controllers")]
	[ApiController]
	[Route("[controller]")]
	public class LoginController : ControllerBase
	{
		[HttpGet, Route("login")]
		public IActionResult Login()
		{
			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecretkey@123"));
			var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
			var jwtSecurityToken = new JwtSecurityToken(
				issuer: "ABCXYZ",
				claims: new List<Claim> { new Claim("username", "test") },
				expires: DateTime.Now.AddMinutes(1000000),
				signingCredentials: signinCredentials);
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var token = jwtTokenHandler.WriteToken(jwtSecurityToken);
			return Ok(token);
		}
	}
}
