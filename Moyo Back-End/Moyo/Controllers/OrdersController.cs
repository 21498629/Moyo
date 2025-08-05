using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moyo.Models;
using Moyo.View_Models;
using NuGet.Protocol.Core.Types;

namespace Moyo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository _repository;

        public OrdersController(AppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET ALL ORDERS
        [HttpGet]
        [Route("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _repository.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET ORDER BY ID
        [HttpGet]
        [Route("GetOrder/{OrderID}")]
        public async Task<IActionResult> GetOrder(int OrderID)
        {
            try
            {
                var order = await _repository.GetOrderAsync(OrderID);
                if (order == null) return NotFound("Order not found.");
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ADD ORDER
        [HttpPost]
        [Route("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderVM ovm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (ovm.OrderItems == null || !ovm.OrderItems.Any())
               // return BadRequest("At least one order item is required.");

            var order = new Order
            {
                OrderNumber = ovm.OrderNumber,
                CreatedAt = ovm.CreatedAt ?? DateTime.UtcNow,
                /*OrderItems = ovm.OrderItems.Select(oi => new OrderItem
                {
                    OrderId = ovm.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()*/

            };

            try
            {
                _repository.Add(order);
                await _repository.SaveChangesAsync();

                /*var orderItem = ovm.OrderItems.Select(oi => new OrderItem
                {
                    OrderId = ovm.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                });*/

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }

        }


        // EDIT ORDER
        [HttpPut]
        [Route("EditOrder/{OrderID}")]
        public async Task<IActionResult> EditOrder(int OrderID, [FromBody] OrderVM ovm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingOrder = await _repository.GetOrderAsync(OrderID);
                if (existingOrder == null) return NotFound("Order not found.");

                // Update order fields
                existingOrder.OrderNumber = ovm.OrderNumber;
                existingOrder.CreatedAt = ovm.CreatedAt ?? existingOrder.CreatedAt;

                // Option 1: Remove all items and re-add (simple)
                existingOrder.OrderItems.Clear();
                foreach (var oi in ovm.OrderItems)
                {
                    existingOrder.OrderItems.Add(new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity
                    });
                }

                if (await _repository.SaveChangesAsync())
                    return Ok(existingOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Update failed.");
        }


        // DELETE ORDER
        [HttpDelete]
        [Route("DeleteOrder/{OrderID}")]
        public async Task<IActionResult> DeleteOrder(int OrderID)
        {
            try
            {
                var existingOrder = await _repository.GetOrderAsync(OrderID);
                if (existingOrder == null) return NotFound("Order not found.");

                _repository.Delete(existingOrder);
                if (await _repository.SaveChangesAsync())
                    return Ok(existingOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Delete failed.");
        }

    }
}
