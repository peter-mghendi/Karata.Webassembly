using Karata.Server.Models;
using Karata.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Karata.Server.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User user);
        Task<User> DeleteUser(User user);
        Task<User> FindUserByEmailAsync(string email);
        Task<User> FindUserByIdAsync(long id);
        Task<List<UserDTO>> GetUserListAsync();
        Task<string> GetUserRoleAsync(string email);
        Task<bool> IsAnExistingUserAsync(long id);
        Task<bool> IsAnExistingUserAsync(string email);
        Task<bool> IsValidUserCredentialsAsync(string email, string password);
        UserDTO ItemToDTO(User user);
        Task<User> ModifyUser(User user);
    }
}