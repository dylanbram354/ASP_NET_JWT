using JWT_API.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_API.Services
{
    public interface IUserService
    {
        Task<Dictionary<string,string>> RegisterAsync(UserForRegistration model); //Method signature for registration; implemented by UserService class and called by UserController.
                                                               //Receives UserForRegistration object as argument.
        Task<AuthenticationResponse> GetTokenAsync(UserForAuthentication model); //Method signature for login; implemented by UserService class and called by UserController.
                                                                                 //Receives UserForAuthentication object as argument.
        Task<string> AddRoleAsync(AddRoleModel model); //Adding a user to a role
    }
}
