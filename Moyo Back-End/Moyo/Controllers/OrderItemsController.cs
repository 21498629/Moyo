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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository _repository;

        public OrderItemsController(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET ALL ORDER ITEMS
        [HttpGet]
        [Route("GetAllOrderItems")]
        public async Task<IActionResult> GetAllOrderItems()
        {
            try
            {
                var results = await _repository.GetAllOrderItemsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET ORDER ITEM BY ID
        [HttpGet]
        [Route("GetOrderItem/{OrderItemID}")]
        public async Task<IActionResult> GetOrderItem(int OrderItemID)
        {
            try
            {
                var item = await _repository.GetOrderItemAsync(OrderItemID);
                if (item == null) return NotFound("Order Item does not exist.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ADD ORDER ITEM
        [HttpPost]
        [Route("AddOrderItem")]
        public async Task<IActionResult> AddOrderItem([FromBody] OrderItemVM ovm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = new OrderItem
            {
                OrderId = ovm.OrderId,
                ProductId = ovm.ProductId,
                Quantity = ovm.Quantity
            };

            try
            {
                _repository.Add(item);
                await _repository.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        // EDIT ORDER ITEM
        [HttpPut]
        [Route("EditOrderItem/{OrderItemID}")]
        public async Task<IActionResult> EditOrderItem(int OrderItemID, [FromBody] OrderItemVM ovm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingItem = await _repository.GetOrderItemAsync(OrderItemID);

                if (existingItem == null) return NotFound("The order item does not exist");

                existingItem.OrderId = ovm.OrderId;
                existingItem.ProductId = ovm.ProductId;
                existingItem.Quantity = ovm.Quantity;

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingItem);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Your request is invalid");
        }

        // DELETE ORDER ITEM
        [HttpDelete]
        [Route("DeleteOrderItem/{OrderItemID}")]
        public async Task<IActionResult> DeleteOrderItem(int OrderItemID)
        {
            try
            {
                var existingItem = await _repository.GetOrderItemAsync(OrderItemID);

                if (existingItem == null) return NotFound("The order item does not exist");

                _repository.Delete(existingItem);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(existingItem);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Your request is invalid");
        }


    }
}
