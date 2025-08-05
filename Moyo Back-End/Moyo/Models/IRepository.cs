using Moyo.Models;
using Moyo.View_Models;

namespace Moyo.Models
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        // USER
        Task<User[]> GetAllUsersAsync();
        Task<User> GetUserAsync(int userID);

        // PRODUCT CATEGORIES
        Task<ProductCategory[]> GetAllProductCategoriesAsync();
        Task<ProductCategory> GetProductCategoryAsync(int productCategoryID);

        // PRODUCTS
        Task<Product[]> GetAllProductsAsync();
        Task<Product> GetProductAsync(int productID);
        Task<Product[]> GetProductsByCategoryAsync(int categoryID);
        Task<Product[]> GetProductsByVendorAsync(int vendorID);

        // VENDORS
        Task<Vendor[]> GetAllVendorsAsync();
        Task<Vendor> GetVendorAsync(int vendorID);

        // ORDERS
        Task<Order[]> GetAllOrdersAsync();
        Task<Order> GetOrderAsync(int orderID);

        // ORDER ITEMS
        Task<OrderItem[]> GetAllOrderItemsAsync();
        Task<OrderItem> GetOrderItemAsync(int orderItemID);
    }
}
