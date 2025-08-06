using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moyo.View_Models;

namespace Moyo.Models
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // USER
        public async Task<User[]> GetAllUsersAsync()
        {
            IQueryable<User> query = _context.Users;
            return await query.ToArrayAsync();
        }

        public async Task<User> GetUserAsync(int userID)
        {
            IQueryable<User> query = _context.Users;
            return await query.FirstOrDefaultAsync();
        }

        // PRODUCT CATEGORIES
        public async Task<ProductCategory[]> GetAllProductCategoriesAsync()
        {
            IQueryable<ProductCategory> query = _context.ProductCategories;
            return await query.ToArrayAsync();
        }

        public async Task<ProductCategory> GetProductCategoryAsync(int categoryID)
        {
            IQueryable<ProductCategory> query = _context.ProductCategories.Where(p => p.Id == categoryID);
            return await query.FirstOrDefaultAsync();
        }

        // PRODUCTS
        public async Task<Product[]> GetAllProductsAsync()
        {
            IQueryable<Product> query = _context.Products;
            return await query.ToArrayAsync();
        }

        public async Task<Product> GetProductAsync(int productID)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.Id == productID);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Product[]> GetProductsByCategoryAsync(int categoryID)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.ProductCategoryId == categoryID);
            return await query.ToArrayAsync();
        }

        public async Task<Product[]> GetProductsByVendorAsync(int vendorID)
        {
            IQueryable<Product> query = _context.Products.Where(v => v.VendorId == vendorID);
            return await query.ToArrayAsync();
        }

        // VENDORS
        public async Task<Vendor[]> GetAllVendorsAsync()
        {
            IQueryable<Vendor> query = _context.Vendors;
            return await query.ToArrayAsync();
        }

        public async Task<Vendor> GetVendorAsync(int vendorID)
        {
            IQueryable<Vendor> query = _context.Vendors.Where(v => v.Id == vendorID);
            return await query.FirstOrDefaultAsync();
        }

        // ORDERS
        public async Task<Order[]> GetAllOrdersAsync()
        {
            IQueryable<Order> query = _context.Orders;
            return await query.ToArrayAsync();
        }

        public async Task<Order> GetOrderAsync(int orderID)
        {
            IQueryable<Order> query = _context.Orders
                .Where(o => o.Id == orderID);
                //.Include(o => o.OrderItems)
                //.ThenInclude(oi => oi.Product);
            return await query.FirstOrDefaultAsync();
        }

        // ORDER ITEMS
        public async Task<OrderItem[]> GetAllOrderItemsAsync()
        {
            IQueryable<OrderItem> query = _context.OrderItems;
            return await query.ToArrayAsync();
        }

        public async Task<OrderItem> GetOrderItemAsync(int orderItemID)
        {
            IQueryable<OrderItem> query = _context.OrderItems
                .Where(oi => oi.Id == orderItemID)
                .Include(oi => oi.Product);
            return await query.FirstOrDefaultAsync();
        }
    }
}
