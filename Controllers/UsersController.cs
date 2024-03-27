using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System;
using System.Linq;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ServerDbContext _serverDbContext;

        public UsersController(ServerDbContext serverDbContext)
        {
            _serverDbContext = serverDbContext;
        }

        [Authorize]
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _serverDbContext.Users.FirstOrDefaultAsync(u => u.Id ==id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, UserUpdateDto updateUserRequest)
        {
            var user = await _serverDbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound();
            // Update user information
            user.Username = updateUserRequest.Username;
            user.Email = updateUserRequest.Email;
            // Add more properties as needed

            _serverDbContext.SaveChanges();

            return Ok(user);
        }

        public class UserUpdateDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            // Add other fields that can be updated
        }
    }
}
