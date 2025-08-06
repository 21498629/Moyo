using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moyo.Models;
using Moyo.View_Models;

namespace Moyo.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository _repository;

        public VendorsController(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET ALL VENDORS
        [HttpGet]
        [Route("GetAllVendors")]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            try
            {
                var results = await _repository.GetAllVendorsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET VENDOR BY ID
        [HttpGet]
        [Route("GetVendor/{VendorID}")]
        public async Task<ActionResult> GetVendor(int VendorID)
        {
            try
            {
                var vendor = await _repository.GetVendorAsync(VendorID);
                if (vendor == null) return NotFound("Vendor does not exist.");
                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EDIT VENDOR
        [HttpPut]
        [Route("EditVendor/{VendorID}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VendorVM>> EditVendor(int vendorID, VendorVM vvm)

        {
            try
            {
                var existingVendor = await _repository.GetVendorAsync(vendorID);

                if (existingVendor == null) return NotFound("Vendor does not exist.");

                existingVendor.Name = vvm.Name;
                existingVendor.Email = vvm.Email;
                existingVendor.Address = vvm.Address;
                existingVendor.PhoneNumber = vvm.PhoneNumber;

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingVendor);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound("Your request is invalid.");
        }

        // ADD VENDOR
        [HttpPost]
        [Route("AddVendor")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddVendor(VendorVM vvm)
        {
            
                var newVendor = new Vendor
                {
                    Name = vvm.Name,
                    Email = vvm.Email,
                    Address = vvm.Address,
                    PhoneNumber = vvm.PhoneNumber,
                    CreatedAt = DateTime.Now
                };

            try
            {
                _repository.Add(newVendor);
                if (await _repository.SaveChangesAsync())
                {
                    return CreatedAtAction(nameof(GetVendor), new { VendorID = newVendor.Id }, newVendor);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound("Your request is invalid.");
        }

        // DELETE VENDOR
        [HttpDelete]
        [Route("DeleteVendor/{VendorID}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVendor(int vendorID)
        {
            try
            {
                var existingVendor = await _repository.GetVendorAsync(vendorID);

                if (existingVendor == null) return NotFound("Vendor does not exist.");

                _repository.Delete(existingVendor);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok("Vendor deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound("Your request is invalid.");
        }
    }
}
