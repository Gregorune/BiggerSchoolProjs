using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FitnesAPI.Database;
using BCryptC = BCrypt.Net.BCrypt;

namespace FitnesAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _dbCtx;
    private readonly JwtHandler _jwtHandler;

    public AuthController(DatabaseContext dbCtx, JwtHandler jwtHandler)
    {
        _dbCtx = dbCtx;
        _jwtHandler = jwtHandler;
    }

    #region REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterReqDto req)
    {
        if(!Validators.IsEmail(req.Email))
            return BadRequest("Email is invalid.");
        
        if (await _dbCtx.Users.AnyAsync(u => u.Email == req.Email))
            return Conflict("Email jest już zajęty.");

        var user = new User {
            Email = req.Email,
            Username = req.Username,
            PasswordHash = BCryptC.HashPassword(req.Password)
        };

        _dbCtx.Users.Add(user);
        await _dbCtx.SaveChangesAsync();
        return Created();
    }
    public record RegisterReqDto(
        string Email, 
        string Password, 
        string Username
    );
    #endregion
    #region LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReqDto req)
    {
        var user = await _dbCtx.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        
        if (user == null || !BCryptC.Verify(req.Password, user.PasswordHash))
            return BadRequest("Błędne dane.");

        var tokenContent = new JwtHandler.JwtContent()
        {
            Uid = user.Id
        };
        
        var token = _jwtHandler.CreateAccessToken(tokenContent);
        var refresh = _jwtHandler.GenerateRefreshToken();

        user.RefreshToken = refresh;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _dbCtx.SaveChangesAsync();

        return Ok(new TokensResDto(token, refresh));
    }
    public record LoginReqDto(
        string Email, 
        string Password
    );
    #endregion
    #region REFRESH TOKEN
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshReqDto req)
    {
        User? user = await _dbCtx.Users.FirstOrDefaultAsync(u => u.RefreshToken == req.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _dbCtx.SaveChangesAsync();
            }
            
            return Unauthorized("Session expired.");
        }
        

        var tokenContent = new JwtHandler.JwtContent()
        {
            Uid = user.Id
        };

        string newJwtToken = _jwtHandler.CreateAccessToken(tokenContent);
        string newRefreshToken = _jwtHandler.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken; 
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _dbCtx.SaveChangesAsync();

        return Ok(new TokensResDto(newJwtToken, newRefreshToken));
    }
    public record RefreshReqDto(string RefreshToken);
    #endregion
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        JwtHandler.JwtContent authData = _jwtHandler.GetJwtContent(User);
        User? user = await _dbCtx.Users.FindAsync(authData.Uid);

        if (user != null) {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _dbCtx.SaveChangesAsync();
        }

        return Ok(new {Message = "Logged out successfully."});
    }
}
public record TokensResDto(string JwtToken, string RefreshToken);