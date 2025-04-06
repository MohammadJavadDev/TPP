using Microsoft.AspNetCore.Mvc;
using RegistrationApi.Business.Interfaces;
using RegistrationApi.Models;

namespace RegistrationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            return Ok(new { Success = true, Data = user });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var (success, message, userId) = await _userService.CreateUserAsync(user);

            if (!success)
                return BadRequest(new { Success = false, Message = message });

            return CreatedAtAction(nameof(GetUserById), new { id = userId },
                new { Success = true, Message = message, UserId = userId });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { Success = false, Message = "ID mismatch." });

            var (success, message) = await _userService.UpdateUserAsync(user);

            if (!success)
                return NotFound(new { Success = false, Message = message });

            return Ok(new { Success = true, Message = message });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);

            if (!success)
                return NotFound(new { Success = false, Message = message });

            return Ok(new { Success = true, Message = message });
        }
    }
}