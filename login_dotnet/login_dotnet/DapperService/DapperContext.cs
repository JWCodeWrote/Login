using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace login_dotnet.DapperService;

public class DapperContext {
    private readonly IConfiguration _config;
    public DapperContext(IConfiguration config) {
        _config = config;
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
}
