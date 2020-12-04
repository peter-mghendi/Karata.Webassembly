using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karata.Server.Models;
using Karata.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Karata.Server.Infrastructure;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Karata.Server.Services;
using Microsoft.AspNetCore.Http;

namespace Karata.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
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

        [HttpGet]
        [Authorize(Policy = Policies.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserList() =>
            await _userService.GetUserListAsync();

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]    
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUser(long id)   
        {
            var user = await _userService.FindUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _userService.ItemToDTO(user);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
