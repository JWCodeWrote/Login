using System.Threading.Tasks;
using login_dotnet.Models;

namespace login_dotnet.DapperService;

public interface IAuthDapperService
{
    Task<bool> Register(RegisterDto r);
    Task<string> Login(LoginDto l);
}