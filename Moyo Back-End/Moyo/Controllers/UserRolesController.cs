using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moyo.Models;
using Moyo.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Moyo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IRepository _repository;

        public UserRolesController(AppDbContext context, UserManager<User> userManager, IRepository repository, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
        }

        // GET ALL USER ROLES
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = _roleManager.Roles.Select(r => new RoleVM
                {
                    RoleId = r.Id,
                    Name = r.Name
                }).ToList();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET USER ROLE BY ID
        [HttpGet]
        [Route("GetUserRole/{roleId}")]
        public async Task<ActionResult> GetRole(int roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null) return NotFound("User role does not exist.");
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // ADD USER ROLE
        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] RoleVM urvm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new Role { Name = urvm.Name };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetAllRoles), new { id = role.Id }, 
                    new RoleVM { RoleId = role.Id, Name = role.Name });

            return BadRequest(result.Errors);
        }

        // EDIT USER ROLE
        [HttpPut]
        [Route("EditRole/{roleId}")]
        public async Task<ActionResult> EditRole(int roleId, [FromBody] RoleVM rvm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound();

            role.Name = rvm.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }

        // DELETE USER ROLE
        [HttpDelete]
        [Route("DeleteUserRole/{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var existingRole = await _roleManager.FindByIdAsync(roleId.ToString());
            if (existingRole == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(existingRole);
            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors);
        }
    }
}
