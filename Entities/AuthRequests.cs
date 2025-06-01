namespace SOCRATIC_LEARNING_DOTNET.Entities;

public class UserRegisterRequest
{
    public string name { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}

public class UserLoginRequest
{
    public string email { get; set; }
    public string password { get; set; }
}
