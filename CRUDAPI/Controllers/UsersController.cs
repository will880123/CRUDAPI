using CRUDAPI.Data;
using CRUDAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CRUDAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 取得所有用戶
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Received request to fetch all users.");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var users = await _context.Users.ToListAsync();

                stopwatch.Stop();
                _logger.LogInformation("Successfully fetched {UserCount} users in {ElapsedMilliseconds} ms.", users.Count, stopwatch.ElapsedMilliseconds);

                return Ok(users);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error occurred while fetching users. Elapsed time: {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// 根據 ID 取得單一用戶
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("Received request to fetch user with ID: {Id}", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {Id}", id);
                    throw new ArgumentException("Id must be greater than zero.");
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID: {Id} not found.", id);
                    throw new KeyNotFoundException("User not found.");
                }

                stopwatch.Stop();
                _logger.LogInformation("Successfully fetched user with ID: {Id} in {ElapsedMilliseconds} ms. User: {@User}", id, stopwatch.ElapsedMilliseconds, user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error occurred while fetching user with ID: {Id}. Elapsed time: {ElapsedMilliseconds} ms.", id, stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }


        /// <summary>
        /// 新增一位用戶
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            _logger.LogInformation("Received request to add a new user: {@User}", user);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
                {
                    _logger.LogWarning("Invalid user data provided: {@User}", user);
                    throw new ArgumentException("Invalid user data. Name and Email are required.");
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                _logger.LogInformation("Successfully added user: {@User} in {ElapsedMilliseconds} ms.", user, stopwatch.ElapsedMilliseconds);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error occurred while adding a new user: {@User}. Elapsed time: {ElapsedMilliseconds} ms.", user, stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }


        /// <summary>
        /// 更新指定 ID 的用戶
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            _logger.LogInformation("Received request to update user with ID: {Id}. New data: {@UpdatedUser}", id, updatedUser);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {Id}", id);
                    throw new ArgumentException("Id must be greater than zero.");
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID: {Id} not found.", id);
                    return NotFound(new { Message = "User not found." });
                }

                if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Name) || string.IsNullOrEmpty(updatedUser.Email))
                {
                    _logger.LogWarning("Invalid updated user data provided for ID: {Id}", id);
                    return BadRequest(new { Message = "Invalid user data. Name and Email are required." });
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;

                await _context.SaveChangesAsync();

                stopwatch.Stop();
                _logger.LogInformation("Successfully updated user with ID: {Id} in {ElapsedMilliseconds} ms. New data: {@User}", id, stopwatch.ElapsedMilliseconds, user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error occurred while updating user with ID: {Id}. Elapsed time: {ElapsedMilliseconds} ms.", id, stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }


        /// <summary>
        /// 刪除指定 ID 的用戶
        /// </summary>[HttpDelete("{id}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Received request to delete user with ID: {Id}", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {Id}", id);
                    throw new ArgumentException("Id must be greater than zero.");
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID: {Id} not found.", id);
                    throw new KeyNotFoundException("User not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                _logger.LogInformation("Successfully deleted user with ID: {Id} in {ElapsedMilliseconds} ms.", id, stopwatch.ElapsedMilliseconds);

                return Ok(new { Message = $"User deleted successfully" });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error occurred while deleting user with ID: {Id}. Elapsed time: {ElapsedMilliseconds} ms.", id, stopwatch.ElapsedMilliseconds);

                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }
    }
}
