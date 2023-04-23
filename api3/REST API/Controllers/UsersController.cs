/*
* $Archive: $
* $Revision: $ 1.0
* $Date: $     03-03-2023
* $Author: $   Subiya
*
* 
* All rights reserved.
* 
* * This software is the confidential and proprietary information
* of Schneider Electric.  Copying or reproduction without prior written                                                                                                                                                    
* approval is prohibited.
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using DLMSScheduler_API;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using FluentAssertions.Common;

namespace DLMSScheduler_API
{
    /// <summary>
    /// this class provides a foundation for implementing user management functionality with support for authentication and role-based access control.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.0        03-03-2023          Subiya              Added required methods for DLMS Scheduler.

    [Route(Constants.UserController)]
    [Authorize]
    [AllowAnonymous]
    [ApiController]
    //    //This class is derived from the ControllerBase class, which provides common functionality for HTTP controllers.
    public class UsersController : ControllerBase
    {
        //responsible for managing user accounts.
        private readonly UserManager<Users> _userManager;
        //responsible for managing roles in the application.
        private readonly RoleManager<IdentityRole> _roleManager;
        // used for accessing configuration data in the application.
        private readonly IConfiguration _configuration;
        //used for logging messages in the application.
        private readonly ILogger<ApiController> _logger;

        //This defines a constructor for an UserController class. 
        public UsersController(UserManager<Users> userManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, ILogger<ApiController> logger)
        {
            // Constructor implementation
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Code snippet for creating a new role in an Identity-based authentication system
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <returns></returns>
        [HttpPost(Constants.CreateRole)]
        public async Task<IActionResult> CreateRole(CreateRoleDTO roleDTO)
        {
            try
            {
                //The CreateAsync method of _roleManager is called to create a new role. 
                var response = await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleDTO.RoleName
                });

                if (response.Succeeded)
                {
                    return Ok("New Role Created");
                }
                else
                {
                    return BadRequest(response.Errors);
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to store data in DB",ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Code snippet for assigning a role to a user in an Identity-based authentication system.
        /// </summary>
        /// <param name="assignRoleToUserDTO"></param>
        /// <returns></returns>
        [HttpPost(Constants.AssignRoleToUser)]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserDTO assignRoleToUserDTO)
        {
            try
            {

                //The method uses the injected _userManager instance to find the user details by email using the FindByEmailAsync method.
                var userDetails = await _userManager.FindByEmailAsync(assignRoleToUserDTO.Email);
                //If the userDetails are not null, it means that a user exists with the provided email, 
                if (userDetails != null)
                {

                    var userRoleAssignResponse = await _userManager.AddToRoleAsync(userDetails, assignRoleToUserDTO.RoleName);

                    if (userRoleAssignResponse.Succeeded)
                    {
                        return Ok("Role Assigned to User: " + assignRoleToUserDTO.RoleName);
                    }
                    else
                    {
                        return BadRequest(userRoleAssignResponse.Errors);
                    }
                }
                else
                {
                    return BadRequest("There are no user exist with this email");
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to assign a role",ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }
        /// <summary>
        /// This code snippet is for refreshing an access token in an authentication system.
        /// </summary>
        /// <param name="refreshTokenRequest"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpPost(Constants.RefreshToken)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var response = new MainResponse();
                if (refreshTokenRequest is null)
                {
                    response.ErrorMessage = "Invalid  request";
                    return BadRequest(response);
                }
                var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);
                if (principal != null)
                {
                        var email = principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email);

                        var user = await _userManager.FindByEmailAsync(email?.Value);

                        if (user is null || user.RefreshToken != refreshTokenRequest.RefreshToken)
                        {
                            response.ErrorMessage = "Invalid Request";
                            return BadRequest(response);
                        }

                        string newAccessToken = GenerateAccessToken(user);
                        string refreshToken = GenerateRefreshToken();

                        user.RefreshToken = refreshToken;
                        await _userManager.UpdateAsync(user);

                        response.IsSuccess = true;
                        response.Content = new AuthenticationResponse
                        {
                            RefreshToken = refreshToken,
                            AccessToken = newAccessToken
                        };
                        return Ok(response);
                }
                else
                {
                    return ErrorResponse.ReturnErrorResponse("Invalid Token Found");
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to check data in DB", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// This code snippet is for authenticating a user in an authentication system 
        /// </summary>
        /// <param name="authenticateUser"></param>
        /// <returns></returns>

        //[Authorize]
        [HttpPost(Constants.AuthenticateUser)]
        public async Task<IActionResult> AuthenticateUser(AuthenticateUser authenticateUser)
        {
            try
            {
                //The method calls the FindByNameAsync method of the injected _userManager to find the user with the provided user name. 
                var user = await _userManager.FindByNameAsync(authenticateUser.UserName);
                if (user == null) return Unauthorized();
                //This method checks whether the provided password is valid for the user.
                bool isValidUser = await _userManager.CheckPasswordAsync(user, authenticateUser.Password);

                if (isValidUser)
                {
                    string accessToken = GenerateAccessToken(user);
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    await _userManager.UpdateAsync(user);

                    var response = new MainResponse
                    {
                        Content = new AuthenticationResponse
                        {
                            RefreshToken = refreshToken,
                            AccessToken = accessToken
                        },
                        IsSuccess = true,
                        ErrorMessage = ""
                    };
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to check data in DB", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        /// <summary>
        /// This method provides a secure and efficient way to generate access tokens for users.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateAccessToken(Users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} { user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserAvatar", $"{user.UserAvatar}"),

            };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = _configuration["JWT:Audience"],
                    Issuer = _configuration["JWT:Issuer"],
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyDetail), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                // Handle the exception
                _logger.LogError("Unable to Generate AccessToken", ex);

                return null;
            }


        }
        /// <summary>
        /// This method generates a JSON Web Token (JWT) access token for a given user object.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {


                //creates and validates JWT tokens.
                var tokenHandler = new JwtSecurityTokenHandler();
                //Generates the key used for signing the JWT token from the configuration file.

                var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var tokenValidationParameter = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(keyDetail),
                };
              
                //Specifies the audience, issuer, expiration, subject, and signing credentials for the JWT token.
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                return principal;

            }
            catch (Exception ex)
            {
               
           
                // Handle the exception
                _logger.LogError("Unable to Generate JWT Token", ex);
                return null;

            }


        }
        /// <summary>
        /// This method generates a random refresh token for use in a web application's authentication and authorization system.
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            try
            {

                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to Generate RefreshToken", ex);
                return ("error");
            }
        }


        /// <summary>
        ///This method provides a secure and efficient way of registering new users in a web application.The method also handles errors gracefully and returns appropriate responses.
        /// </summary>
        /// <param name="registerUserDTO"></param>
        /// <returns></returns>
        //[Authorize]
        //[AllowAnonymous]

        [HttpPost(Constants.RegisterUser)]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            try
            {

                    var userToBeCreated = new Users
                    {
                        Email = registerUserDTO.Email,
                        FirstName = registerUserDTO.FirstName,
                        LastName = registerUserDTO.LastName,
                        UserName = registerUserDTO.Email,
                        Address = registerUserDTO.Address,
                        Gender = registerUserDTO.Gender
                    };
                

                 if (!string.IsNullOrWhiteSpace(registerUserDTO.UserAvatar))
                    {
                        byte[] imgBytes = Convert.FromBase64String(registerUserDTO.UserAvatar);
                        string fileName = $"{Guid.NewGuid()}_{userToBeCreated.FirstName.Trim()}_{userToBeCreated.LastName.Trim()}.jpeg";
                        string avatar = await UploadFile(imgBytes, fileName);
                        userToBeCreated.UserAvatar = avatar;
                 }
                
                 var response = await _userManager.CreateAsync(userToBeCreated, registerUserDTO.Password);
                 if (response.Succeeded)
                 {
                     return Ok(new MainResponse
                     {
                          IsSuccess = true,
                     });
                 }
                 else
                 {
                    return ErrorResponse.ReturnErrorResponse(response.Errors?.ToString() ?? "");
                 }
                  
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to register user.", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// This code is defining an asynchronous method named UploadFile that takes in a byte array bytes and a string fileName as parameters. The method returns a Task<string> object that represents the asynchronous operation of uploading the file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<string> UploadFile(byte[] bytes, string fileName)
        {
            try
            {
                string uploadsFolder = Path.Combine("Images", fileName);
                Stream stream = new MemoryStream(bytes);
                using (var ms = new FileStream(uploadsFolder, FileMode.Create))
                {
                    await stream.CopyToAsync(ms);
                }
                return uploadsFolder;
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to upload file", ex);
                return null;
            }
        }

        /// <summary>
        /// This method provides basic functionality for deleting a user from a database.
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete(Constants.DeleteUser)]
        public async Task<IActionResult> DeleteUser(DeleteUserDTO userDetails)
        {
            try
            {

                var existingUser = await _userManager.FindByEmailAsync(userDetails.Email);
                if (existingUser != null)
                {
                    var response = await _userManager.DeleteAsync(existingUser);

                    if (response.Succeeded)
                    {
                        return Ok(new MainResponse
                        {
                            IsSuccess = true,
                        });
                    }
                    else
                    {
                        return ErrorResponse.ReturnErrorResponse(response.Errors?.ToString() ?? "");
                    }
                }
                else
                {
                    return ErrorResponse.ReturnErrorResponse("No User found with this email");
                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to delete data from DB.", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
    
        }
    }
}
