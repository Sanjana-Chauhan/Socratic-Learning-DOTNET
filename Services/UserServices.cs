using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SOCRATIC_LEARNING_DOTNET.Data;
using SOCRATIC_LEARNING_DOTNET.Entities;
using SOCRATIC_LEARNING_DOTNET.Interfaces;

namespace SOCRATIC_LEARNING_DOTNET.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string userId)
        {
            return await _context.User.FindAsync(userId);
        }

        public async Task<User?> AddUserAsync(string name, string email, string password)
        {
            // Check if user already exists
            if (await _context.User.AnyAsync(u => u.Email == email))
                return null;

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password),
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            var hash = HashPassword(password);
            return user.PasswordHash == hash ? user : null;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
