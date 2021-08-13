using JWT_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWT_API.Models;

namespace JWT_API.Controllers
{
    [Route("api/secured")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SecuredController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult SecuredEndpoint()
        {
            var userId = User.FindFirstValue("uid"); //This corresponds to the "claims" added in the register method on UserServices
            string userName = _context.Users.Where(u => u.Id == userId).Select(u => u.UserName).SingleOrDefault();
            return Ok($"Your username is {userName}. This endpoint is only available to Admin users.");
        }
    }
}
