using Karata.Server.Data;
using Karata.Server.Infrastructure;
using Karata.Server.Models;
using Karata.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karata.Server.Services
{
    public class UserService : IUserService
    {
        private readonly KarataContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordService _passwordService;

        public UserService(ILogger<UserService> logger, KarataContext context, IPasswordService passwordService)
        {
            _logger = logger;
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<List<UserDTO>> GetUserListAsync() => 
            await _context.Users.Select(x => ItemToDTO(x)).ToListAsync();

        public async Task<bool> IsValidUserCredentialsAsync(string email, string password)
        {
            _logger.LogInformation($"Validating user [{email}]");

            if (string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password)
                || !await IsAnExistingUserAsync(email))
            {
                return false;
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var user = await _context.Users.SingleAsync(u => u.Email == email);
            return _passwordService.VerifyHash(passwordBytes, user.Salt, user.Password);
        }

        public async Task<bool> IsAnExistingUserAsync(long id) =>
            await _context.Users.AnyAsync(e => e.Id == id);

        public async Task<bool> IsAnExistingUserAsync(string email) =>
            await _context.Users.AnyAsync(e => e.Email.Equals(email));

        public UserDTO ItemToDTO(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
        };

        public async Task<User> CreateUser(User user)
        {
            user.Role = await _context.Users.AnyAsync() ? Policies.User : Policies.Admin;

            byte[] salt = null;
            user.Password = _passwordService.HashPassword(user.Password, ref salt);
            user.Salt = salt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> FindUserByIdAsync(long id) =>
            await _context.Users.FindAsync(id);

        public async Task<User> FindUserByEmailAsync(string email) =>
            await _context.Users.SingleAsync(u => u.Email == email);

        public async Task<User> DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> ModifyUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
