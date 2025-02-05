using ChatAppBE.Data;
using ChatAppBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ChatAppBE.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult GetUsers()
        {
            // 🔥 Lấy user hiện tại từ JWT token
            string currentUser = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser))
                return Unauthorized(new { message = "Unauthorized" });

            Console.WriteLine($"🛠 DEBUG: Current User = {currentUser}");

            var users = _context.Users
                .Where(u => u.Username != currentUser) // ❌ Loại bỏ user hiện tại
                .Select(u => new { u.Username })
                .ToList();

            return Ok(users);
        }
    }
}