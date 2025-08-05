using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Moyo.Models;
using Moyo.View_Models;
using Microsoft.AspNetCore.Authorization;

namespace Moyo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository _repository;

        public ProductCategoriesController(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET ALL PRODUCT CATEGORIES
        [HttpGet]
        [Route("GetAllProductCategories")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            try
            {
                var results = await _repository.GetAllProductCategoriesAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET PRODUCT CATEGORY BY ID
        [HttpGet]
        [Route("GetProductCategories/{CategoryID}")]
        public async Task<ActionResult> GetProductCategories(int CategoryID)
        {
            try
            {
                var category = await _repository.GetProductCategoryAsync(CategoryID);
                if (category == null) return NotFound("Product category does not exist.");
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // ADD PRODUCT CATEGORY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProductCategory")]
        public async Task<IActionResult> AddProductCategory(ProductCategoryVM cvm)
        {
            var category = new ProductCategory
            {
                Name = cvm.Name,
                Description = cvm.Description,
            };

            try
            {
                _repository.Add(category);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

            return Ok(category);
        }

        // EDIT PRODUCT CATEGORY
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("EditProductCategory/{CategoryID}")]
        public async Task<ActionResult<ProductCategoryVM>> EditProductCategory(int CategoryID, ProductCategoryVM cvm)
        {
            try
            {
                var existingCategory = await _repository.GetProductCategoryAsync(CategoryID);

                if (existingCategory == null) return NotFound($"The product category does not exist");

                existingCategory.Name = cvm.Name;
                existingCategory.Description = cvm.Description;


                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingCategory);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Your request is invalid");
        }

        // DELETE PRODUCT CATEGORY
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteProductCategory/{CategoryID}")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            try
            {
                var existingCategory = await _repository.GetProductCategoryAsync(CategoryID);

                if (existingCategory == null) return NotFound($"The product category does not exist");

                _repository.Delete(existingCategory);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingCategory);
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