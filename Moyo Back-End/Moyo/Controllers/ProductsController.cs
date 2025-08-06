using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moyo.Models;
using Moyo.View_Models;

namespace Moyo.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository _repository;

        public ProductsController(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET ALL PRODUCTS
        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var results = await _repository.GetAllProductsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET PRODUCT BY ID
        [HttpGet]
        [Route("GetProduct/{ProductID}")]
        public async Task<ActionResult> GetUProduct(int ProductID)
        {
            try
            {
                var product = await _repository.GetProductAsync(ProductID);
                if (product == null) return NotFound("Product does not exist.");
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET PRODUCTS BY CATEGORY
        [HttpGet]
        [Route("GetProductsByCategory/{CategoryId}")]
        public async Task<ActionResult> GetProductsByCategory(int CategoryId)
        {
            try
            {
                var products = await _repository.GetProductsByCategoryAsync(CategoryId);

                if (products == null || !products.Any())
                    return NotFound("No products found for this category.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET PRODUCTS BY VENDOR
        [HttpGet]
        [Route("GetProductsByVendor/{VendorId}")]
        public async Task<ActionResult> GetProductsByVendor(int VendorId)
        {
            try
            {
                var products = await _repository.GetProductsByVendorAsync(VendorId);

                if (products == null || !products.Any())
                    return NotFound("No products found for this vendor.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ADD PRODUCT
        [HttpPost]
        [Route("AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromForm] ProductVM pvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imagePath = null;

            if (pvm.ImageFile != null && pvm.ImageFile.Length > 0)
            {
                // VALIDATE FILE IS AN IMAGE
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(pvm.ImageFile.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (JPG, JPEG, PNG, GIF) are allowed.");
                    return BadRequest("Image format incorrect.");
                }

                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedMimeTypes.Contains(pvm.ImageFile.ContentType.ToLowerInvariant()))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (JPG, JPEG, PNG, GIF) are allowed.");
                    return BadRequest("Image format incorrect.");
                }

                // GENERATE UNIQUE FILENAME
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(pvm.ImageFile.FileName);
                var uploadsFolder = Path.Combine("Images", "Products");
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadsFolder, uniqueFileName);

                // CREATE DIRECTORY IF IT DOESN'T EXIST
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // SAVE FILE
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await pvm.ImageFile.CopyToAsync(stream);
                }

                // STORE PATH
                imagePath = Path.Combine("Images", "Products", uniqueFileName).Replace("\\", "/");
            }

            var product = new Product
            {
                Name = pvm.Name,
                Description = pvm.Description,
                Price = pvm.Price,
                Image = imagePath,
                Quantity = pvm.Quantity,
                IsActive = pvm.IsActive,
                VendorId = pvm.VendorId,
                ProductCategoryId = pvm.ProductCategoryId,
            };

            try
            {
                _repository.Add(product);
                await _repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");

            }

            return Ok(product);
        }

        // EDIT PRODUCT
        [HttpPut]
        [Route("EditProduct/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditProduct(int id, [FromForm] ProductVM pvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get existing product
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            string imagePath = existingProduct.Image; // Keep existing image unless new one is uploaded

            if (pvm.ImageFile != null && pvm.ImageFile.Length > 0)
            {
                // VALIDATE FILE IS AN IMAGE
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(pvm.ImageFile.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (JPG, JPEG, PNG, GIF) are allowed.");
                    return BadRequest("Image format incorrect.");
                }

                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedMimeTypes.Contains(pvm.ImageFile.ContentType.ToLowerInvariant()))
                {
                    ModelState.AddModelError("ImageFile", "Only image files (JPG, JPEG, PNG, GIF) are allowed.");
                    return BadRequest("Image format incorrect.");
                }

                // GENERATE UNIQUE FILENAME
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadsFolder = Path.Combine("Images", "Products");
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadsFolder, uniqueFileName);

                // CREATE DIRECTORY IF IT DOESN'T EXIST
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // SAVE FILE
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await pvm.ImageFile.CopyToAsync(stream);
                }

                // STORE PATH
                imagePath = Path.Combine("Images", "Products", uniqueFileName).Replace("\\", "/");

                // Optionally delete old image file if it exists
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), existingProduct.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            // Update product properties
            existingProduct.Name = pvm.Name;
            existingProduct.Description = pvm.Description;
            existingProduct.Price = pvm.Price;
            existingProduct.Image = imagePath;
            existingProduct.Quantity = pvm.Quantity;
            existingProduct.IsActive = pvm.IsActive;
            existingProduct.VendorId = pvm.VendorId;
            existingProduct.ProductCategoryId = pvm.ProductCategoryId;

            try
            {
                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingProduct);
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            return Ok(existingProduct);
        }

        // DELETE PRODUCT
        [HttpDelete]
        [Route("DeleteProduct/{ProductID}")]
        //[Authorize("Admin, Vendor")]
        public async Task<IActionResult> DeleteProduct(int ProductID)
        {
            try
            {
                var existingProduct = await _repository.GetProductAsync(ProductID);

                if (existingProduct == null) return NotFound($"The product does not exist");

                _repository.Delete(existingProduct);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingProduct);
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
