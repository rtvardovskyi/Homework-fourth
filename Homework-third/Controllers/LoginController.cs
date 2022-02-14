using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using BLL.Interfaces;
using Domain;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Homework_third.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    //public static User user = new User();

    private readonly IConfiguration _configuration;
    private readonly ApplicationContext _context;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public LoginController(IConfiguration configuration, ApplicationContext context, IUserService userService, IMapper mapper)
    {
        _configuration = configuration;
        _context = context;
        _userService = userService;
        _mapper = mapper;
    }
    
    [HttpPost("register")]
    public ActionResult Register(RegisterDto request)
    {
        var userFromTable = _userService.GetUserByName(request.Username);

        if (userFromTable != null)
        {
            return BadRequest("User already exist");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = _mapper.Map<User>(request);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        
        _userService.AddUser(user);
        _userService.SaveChanges();
        
        return Ok("Successfully registered");
    }
    

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    [HttpPost("login")]
    public ActionResult<string> Login(LoginDto request)
    {
        var user = _userService.GetUserByName(request.Username);

        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest("Wrong password.");
        }

        string token = CreateToken(user);
        return Ok(token);
    }

    [HttpPatch("{username}"), Authorize(Roles = "Admin")]
    public ActionResult UpdateRole(string username, JsonPatchDocument<UpdateRoleDto> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Wrong request format");
        }

        var user = _userService.GetUserByName(username);

        if (user == null)
        {
            return BadRequest("User not found");
        }

        var userToPatch = _mapper.Map<UpdateRoleDto>(user);
        
        patchDoc.ApplyTo(userToPatch, ModelState);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(userToPatch, user);

        _userService.SaveChanges();

        return NoContent();

    }
    
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
    
    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}