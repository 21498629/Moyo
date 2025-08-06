using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Moyo.Models;
using Moyo.View_Models;

namespace Moyo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _appDbContext;

        public UsersController(IRepository repository, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ITokenService tokenService, SignInManager<User> signInManager, AppDbContext appDbContext)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
        }

        // REGISTER USER
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserVM uvm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(uvm.Password))
                    return BadRequest(new { message = "Password is required" });

                var user = new User
                {
                    Name = uvm.Name,
                    Surname = uvm.Surname,
                    Email = uvm.Email,
                    UserName = uvm.Email, 
                    Address = uvm.Address,
                    PhoneNumber = uvm.PhoneNumber,
                    CreatedAt = uvm.CreatedAt ?? DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, uvm.Password);
                if (!result.Succeeded)
                    return StatusCode(500, result.Errors);

                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, roles, new[] { "api.read" });

                return Ok(new
                {
                    message = "User registered successfully",
                    token
                    /*user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email
                    },
                    token = _tokenService?.CreateToken(user)*/
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }


        // LOGIN
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM lvm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(lvm.UserName.ToLower());

                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, lvm.Password, false);

                var roles = await _userManager.GetRolesAsync(user);

                var scopes = roles.Contains("Admin")
                ? new[] { "api.read", "api.write" }
                : new[] { "api.read" };

                var token = _tokenService.CreateToken(user, roles, scopes);

                if (!result.Succeeded)
                {
                    return Unauthorized("Email Address not found and/or Password incorrect.");
                }

                return Ok(new
                {
                    token, roles, scopes
                    //user.UserName,
                    //Token = _tokenService.CreateToken(user),
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET ALL USERS
        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var results = await _repository.GetAllUsersAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET USER BY ID
        [HttpGet]
        [Route("GetUser/{UserID}")]
        public async Task<ActionResult> GetUser(int UserID)
        {
            try
            {
                var users = await _repository.GetUserAsync(UserID);
                if (users == null) return NotFound("User does not exist.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // ADD USER
        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] UserVM uvm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrWhiteSpace(uvm.Password))
                    return BadRequest(new { message = "Password is required" });

                if (string.IsNullOrWhiteSpace(uvm.RoleName))
                    return BadRequest(new { message = "Role name is required." });

                var user = new User
                {
                    Name = uvm.Name,
                    Surname = uvm.Surname,
                    Email = uvm.Email,
                    UserName = uvm.Email,
                    Address = uvm.Address,
                    PhoneNumber = uvm.PhoneNumber,
                    CreatedAt = uvm.CreatedAt ?? DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, uvm.Password);
                if (!result.Succeeded)
                    return StatusCode(500, result.Errors);

                if (!await _roleManager.RoleExistsAsync(uvm.RoleName))
                    return BadRequest(new { message = $"Role '{uvm.RoleName}' does not exist." });

                var roleResult = await _userManager.AddToRoleAsync(user, uvm.RoleName);
                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);

                return Ok(new
                {
                    message = "User created and assigned to role.",
                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        Role = uvm.RoleName
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // EDIT USER
        [HttpPut("EditUser/{UserID}")]
        public async Task<IActionResult> EditUser(int UserID, [FromBody] UserVM uvm)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserID.ToString());
                if (user == null)
                    return NotFound(new { message = "User not found." });

                user.Name = uvm.Name;
                user.Surname = uvm.Surname;
                user.Email = uvm.Email;
                user.UserName = uvm.Email;
                user.Address = uvm.Address;
                user.PhoneNumber = uvm.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return StatusCode(500, updateResult.Errors);

                if (!string.IsNullOrWhiteSpace(uvm.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, uvm.Password);
                    if (!passwordResult.Succeeded)
                        return StatusCode(500, passwordResult.Errors);
                }

                if (!string.IsNullOrWhiteSpace(uvm.RoleName))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return StatusCode(500, removeResult.Errors);

                    if (!await _roleManager.RoleExistsAsync(uvm.RoleName))
                        return BadRequest(new { message = $"Role '{uvm.RoleName}' does not exist." });

                    var addRole = await _userManager.AddToRoleAsync(user, uvm.RoleName);
                    if (!addRole.Succeeded)
                        return StatusCode(500, addRole.Errors);
                }

                return Ok(new
                {
                    message = "User updated successfully",
                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        Role = uvm.RoleName
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }



        // DELETE USER
        [HttpDelete]
        [Route("DeleteUser/{UserID}")]
        public async Task<IActionResult> DeleteUser(int UserID)
        {
            try
            {
                var existingUser = await _repository.GetUserAsync(UserID);

                if (existingUser == null) return NotFound($"The user does not exist");

                _repository.Delete(existingUser);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingUser);
                }
                ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Your request is invalid");
        }
    }
}
