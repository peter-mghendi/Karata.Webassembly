using Karata.Server.Data;
using Karata.Server.Models;
using Karata.Server.Services;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karata.Server.Controllers
{
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

        // GET: api/me
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            var user = await _context.Users.SingleAsync(u => u.Email == User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            return _userService.ItemToDTO(user);
        }

        // PUT: api/me
        [HttpPut]
        public async Task<IActionResult> PutUser(UserDTO userDTO)
        {
            var email = User.Identity.Name;
            var user = await _context.Users.SingleAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Id != userDTO.Id)
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
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // TODO Change password

        // DELETE: api/me
        [HttpDelete]
        public async Task<ActionResult<User>> DeleteUser()
        {
            var user = await _context.Users.SingleAsync(u => u.Email == User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }

}
