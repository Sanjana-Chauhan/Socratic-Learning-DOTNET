using SOCRATIC_LEARNING_DOTNET.Entities;

namespace SOCRATIC_LEARNING_DOTNET.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(string userId);
        Task<User?> AddUserAsync(string name, string email, string password);
        Task<User?> ValidateUserAsync(string email, string password);
    }
}
