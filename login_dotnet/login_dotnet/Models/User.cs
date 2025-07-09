namespace login_dotnet.Models;

public class User {
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}

public class RegisterDto { public string Username, Password; }
public class LoginDto { public string Username, Password; }