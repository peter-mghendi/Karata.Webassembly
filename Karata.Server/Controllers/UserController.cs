using Karata.Server.Data;
using Karata.Server.Services;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Karata.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/me")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly KarataContext _context;
        private readonly IUserService _userService;

        public UserController(KarataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            var user = await _context.Users.SingleAsync(u => u.Email == User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            return _userService.ItemToDTO(user);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutUser(UserDTO userDTO)
        {
            var email = User.Identity.Name;
            var user = await _context.Users.SingleAsync(u => u.Email == email);

            if (user == null || user.Id != userDTO.Id)
            {
                return BadRequest();
            }

            user.Email = userDTO.Email;
            user.Username = userDTO.Username;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userService.IsAnExistingUserAsync(email))
                {
                    return BadRequest();
                }

                throw;
            }

            return NoContent();
        }

        // TODO: Change password

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> DeleteUser()
        {
            var user = await _context.Users.SingleAsync(u => u.Email == User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return _userService.ItemToDTO(user);
        }
    }

}
