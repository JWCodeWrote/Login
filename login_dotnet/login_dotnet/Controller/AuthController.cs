using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using login_dotnet.DapperService;
using login_dotnet.Models;
using login_dotnet.DapperService;

namespace login_dotnet.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthDapperService _repo;
    public AuthController(IAuthDapperService repo){ _repo=repo; }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto){
        if (!await _repo.Register(dto)) return BadRequest("重複帳號");
        return Ok("註冊成功");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto){
        var token = await _repo.Login(dto);
        if (token==null) return Unauthorized("帳號或密碼錯誤");
        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
        => Ok(new { userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value });
}
