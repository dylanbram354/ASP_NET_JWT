using JWT_API.Data;
using JWT_API.DataTransferObjects;
using JWT_API.Models;
using JWT_API.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JWT_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ApplicationDbContext _context;
        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, ApplicationDbContext context) //constructor setting up references to framework stuff
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _context = context;
        }

        public async Task<Dictionary<string, string>> RegisterAsync(UserForRegistration model) //passing in a UserForRegistration object which will be converted to a User
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email); //checking if user with this email already exists
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password); //creating User object using UserManager (Identity class)
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "USER"); //adding new user to default role of "User"
                    return new Dictionary<string, string>
                    {
                        {
                            "Message", $"User Registered with username {user.UserName}"
                        }
                    };
                }
                else
                {
                    var errorResult = new Dictionary<string, string> { };
                    foreach (var error in result.Errors)
                    {
                        errorResult.Add(error.Code, error.Description);
                    }
                    return errorResult;
                }

            }
            else
            {
                return new Dictionary<string, string>
                {
                    {
                        "Message", $"Email {user.Email} is already registered."
                    }
                };
            }
        }

        public async Task<AuthenticationResponse> GetTokenAsync(UserForAuthentication model)
        {
            var authenticationResponse = new AuthenticationResponse(); //Instantiating an AuthenticationResponse model in order to set up a response to the request.
            var user = await _userManager.FindByNameAsync(model.UserName); //Checking database for user with the given username
            if (user == null)
            {
                authenticationResponse.IsAuthenticated = false;
                authenticationResponse.Message = $"No accounts registered with username {model.UserName}";
                return authenticationResponse;
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password)) //checking whether password is valid
            {
                authenticationResponse.IsAuthenticated = true;
                JwtSecurityToken jwt = await CreateJwtToken(user); //Creating token using method defined below
                authenticationResponse.Token = new JwtSecurityTokenHandler().WriteToken(jwt); //Using built-in Identity method to set up JWT token
                authenticationResponse.Email = user.Email;
                authenticationResponse.UserName = user.UserName;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false); //getting this user's roles, if any
                authenticationResponse.Roles = rolesList.ToList();
                return authenticationResponse;
            }
            authenticationResponse.IsAuthenticated = false;
            authenticationResponse.Message = $"Incorrect credentials for user {user.Email}."; //Returning this message if password is invalid
            return authenticationResponse;

        }

        public async Task<JwtSecurityToken> CreateJwtToken(User user) //Generating a JWT ith a user's info encoded (lots of "framework stuff" here)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            for(int i = 0; i< roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i])); //adding user's roles to a list
            }

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model) //Adding a user to a role when they submit their username, password, and the name of the role.
                                                                   //This could be modified on the UserController to require Admin authorization if needed.
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return $"No accounts with username {model.UserName}.";
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var validRoles = _context.Roles.Select(r => r.NormalizedName).ToList();
                if (validRoles.Contains(model.Role.ToUpper()))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return $"Added {model.Role} to user {model.UserName}.";
                }
                else
                {
                    return $"Role {model.Role} not found.";
                }
            }
            return $"Incorrect Credentials for user {user.UserName}.";
        }
    }
}
