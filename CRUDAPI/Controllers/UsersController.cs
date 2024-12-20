using CRUDAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUDAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> _users = new List<User>();
        private static int _idCounter = 1;

        /// <summary>
        /// 取得所有用戶
        /// </summary>
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_users);
        }

        /// <summary>
        /// 根據 ID 取得單一用戶
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be greater than zero.");

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// 新增一位用戶
        /// </summary>
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { Message = "Invalid user data. Name and Email are required." });
            }

            user.Id = _idCounter++;
            _users.Add(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// 更新指定 ID 的用戶
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be greater than zero.");

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Name) || string.IsNullOrEmpty(updatedUser.Email))
                    return BadRequest(new { Message = "Invalid user data. Name and Email are required." });

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }

        /// <summary>
        /// 刪除指定 ID 的用戶
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id must be greater than zero.");

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                _users.Remove(user);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Detail = ex.Message });
            }
        }
    }
}
