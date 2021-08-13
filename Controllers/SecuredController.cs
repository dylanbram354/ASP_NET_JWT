using JWT_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWT_API.Controllers
{
    [Route("api/secured")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SecuredController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult SecuredEndpoint()
        {
            var userId = User.FindFirstValue("id");
            string userName = _context.Users.Where(u => u.Id == userId).Select(u => u.UserName).SingleOrDefault();
            return Ok($"Your username is {userName}.");
        }
    }
}
