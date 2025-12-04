using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Configuration;
using WebApi.Dto;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly BookstoreContext _db;

    private readonly IMapper _mapper;

    public UsersController(BookstoreContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto loginUserRequest)
    {
        var user = await _db.Users
            .Include(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email.ToLower() == loginUserRequest.Login.ToLower());

        if (user == null)
        {
            return NotFound("Login not found");
        }

        if (user.Password != loginUserRequest.Password)
        {
            return Conflict("Bad password.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(5),
            SigningCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        };

        var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
        var encodedAccessToken = tokenHandler.WriteToken(accessToken);

        var token = new TokenResponseDto()
        {
            Token = encodedAccessToken,
            Expires = accessToken.ValidTo,
        };

        return Ok(token);
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var currentUser = await _db.Users
            .Include(x => x.Role)
            .SingleAsync(x => x.Email == User.Identity.Name);

        return Ok(_mapper.Map<UserResponseDto>(currentUser));
    }

}
