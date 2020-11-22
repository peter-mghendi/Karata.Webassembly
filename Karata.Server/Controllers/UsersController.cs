using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karata.Server.Data;
using Karata.Server.Models;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Karata.Server.Infrastructure;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Karata.Server.Services;

namespace Karata.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IPasswordService _passwordService;

        public UsersController(
            ILogger<UsersController> logger,
            IUserService userService,
            IJwtAuthManager jwtAuthManager,
            IPasswordService passwordService)
        {
            _logger = logger;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _passwordService = passwordService;
        }

        // GET: api/users
        [HttpGet]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserList() =>
            await _userService.GetUserListAsync();

        // GET: api/users/5
        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            var user = await _userService.FindUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _userService.ItemToDTO(user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {

                await _userService.ModifyUser(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userService.IsAnExistingUserAsync(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(
            [FromBody] SignupRequest request,
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            User user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = Encoding.UTF8.GetBytes(request.Password)
            };

            try
            {
                user = await _userService.CreateUser(user);
            }
            catch (DbUpdateException)
            {
                if (await _userService.IsAnExistingUserAsync(user.Email))
                {
                    ModelState.AddModelError(nameof(user.Email), "That email address is already in use");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
                else throw;
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _userService.ItemToDTO(user));
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<UserDTO>> DeleteUser(long id)
        {
            var user = await _userService.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user = await _userService.DeleteUser(user);

            return _userService.ItemToDTO(user);
        }
    }
}
