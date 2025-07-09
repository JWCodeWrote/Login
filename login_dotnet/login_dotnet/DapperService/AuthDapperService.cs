using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using Dapper;
using login_dotnet.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace login_dotnet.DapperService;

public class AuthDapperService : IAuthDapperService
{
    private readonly DapperContext _ctx;
    private readonly IConfiguration _config;
    public AuthDapperService(DapperContext ctx, IConfiguration cfg) {
        _ctx = ctx; _config = cfg;
    }

    public async Task<bool> Register(RegisterDto r) {
        using var db = _ctx.CreateConnection();
        var exists = await db.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE Username=@Username",
            new { r.Username });
        if (exists > 0) return false;

        var pwh = BCrypt.Net.BCrypt.HashPassword(r.Password);
        await db.ExecuteAsync(
            "INSERT INTO Users (Username,PasswordHash) VALUES (@Username,@pwh)",
            new { r.Username, pwh });
        return true;
    }

    public async Task<string> Login(LoginDto l) {
        using var db = _ctx.CreateConnection();
        var user = await db.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username=@Username",
            new { l.Username });
        if (user == null || !BCrypt.Net.BCrypt.Verify(l.Password, user.PasswordHash))
            return null;

        var jwt = _config.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]);
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}