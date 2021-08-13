using JWT_API.DataTransferObjects;
using JWT_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) //setting up IUserService in order to use its methods.. but why the interface here instead of the class?
                                                        //What if multiple classes inherited the interface; how would it know which method implementation to use?
        {
            _userService = userService;
        }

        [HttpPost("register")]

        public async Task<ActionResult> RegisterAsync(UserForRegistration model)
        {
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync(UserForAuthentication model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }
    }
}
