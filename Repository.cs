using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Shop3D.DataAccessLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infosys.Shop3D.DataAccessLayer
{
    public class Shop3DRepository
    {
        private Shop3DContext context;
        public Shop3DRepository(Shop3DContext context)
        {
            this.context=context;
        }


        #region GetAllProducts - Returns all products
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                products = (from product in context.Products select product).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion


        #region SearchProducts - Searches products by term
        public List<Product> SearchProducts(string searchTerm)
        {
            List<Product> products = new List<Product>();
            try
            {
                var parameter = new SqlParameter("@SearchTerm", searchTerm);

                products = context.Products
                    .FromSqlRaw("EXEC sp_SearchProducts @SearchTerm", parameter)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion


        #region RegisterUser - Registers a new user
        public User RegisterUser(string email, string passwordHash, string firstName, string lastName, string phoneNumber,
                                string address, string city, string state, string postalCode, string country, int roleId)
        {
            User newUser = null;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@FirstName", firstName),
                    new SqlParameter("@LastName", lastName),
                    new SqlParameter("@PhoneNumber", phoneNumber),
                    new SqlParameter("@Address", address),
                    new SqlParameter("@City", city),
                    new SqlParameter("@State", state),
                    new SqlParameter("@PostalCode", postalCode),
                    new SqlParameter("@Country", country),
                    new SqlParameter("@RoleID", roleId)
                };

                var userId = context.Database
                    .ExecuteSqlRaw("EXEC sp_RegisterUser @Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, " +
                                  "@Address, @City, @State, @PostalCode, @Country, @RoleID", parameters);

                if (userId > 0)
                {
                    newUser = context.Users.FirstOrDefault(u => u.Email == email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return newUser;
        }
        #endregion


        #region AuthenticateUser - Authenticates user with email and password
        public User AuthenticateUser(string email, string passwordHash)
        {
            User user = null;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@PasswordHash", passwordHash)
                };

                var users = context.Users
                    .FromSqlRaw("EXEC sp_AuthenticateUser @Email, @PasswordHash", parameters)
                    .ToList();

                user = users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion



        #region UpdateUserDetails - Updates user profile details
        public bool UpdateUserDetails(User userObj)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userObj.UserId),
                    new SqlParameter("@FirstName", userObj.FirstName),
                    new SqlParameter("@LastName", userObj.LastName),
                    new SqlParameter("@PhoneNumber", userObj.PhoneNumber),
                    new SqlParameter("@Address", userObj.Address),
                    new SqlParameter("@City", userObj.City),
                    new SqlParameter("@State", userObj.State),
                    new SqlParameter("@PostalCode", userObj.PostalCode),
                    new SqlParameter("@Country", userObj.Country)
                };

                var result = context.Database
                    .ExecuteSqlRaw("EXEC sp_UpdateUserDetails @UserID, @FirstName, @LastName, @PhoneNumber, " +
                                  "@Address, @City, @State, @PostalCode, @Country", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion


        #region GetProductById - Returns a product by ID
        public Product GetProductById(int productId)
        {
            Product product = null;
            try
            {
                product = context.Products.Find(productId);
                if (product != null)
                {
                    // Get product images
                    product.ProductImages = context.ProductImages
                        .Where(img => img.ProductId == productId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return product;
        }
        #endregion

        #region SaveProduct - Saves a product for a user
        public bool SaveProduct(int userId, int productId)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ProductID", productId)
                };

                var result = context.Database
                    .ExecuteSqlRaw("EXEC sp_SaveProduct @UserID, @ProductID", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region RemoveSavedProduct - Removes a saved product for a user
        public int RemoveSavedProduct(int userId, int productId)
        {
            int result;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ProductID", productId)
                };

                result = context.Database
                    .ExecuteSqlRaw("EXEC sp_RemoveSavedProduct @UserID, @ProductID", parameters);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }
        #endregion

        #region GetSavedProducts - Gets all saved products for a user
        public List<Product> GetSavedProducts(int userId)
        {
            List<Product> products = new List<Product>();
            try
            {
                //var parameter = new SqlParameter("@UserID", userId);

                products = (from p in context.Products
                            join sp in context.SavedProducts
                           on p.ProductId equals sp.ProductId
                            where sp.UserId == userId
                            select p).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #region CreateOrder - Creates a new order
        public bool CreateOrder(Order orderObj)
        {
            bool status = false;
            try
            {
                context.Orders.Add(orderObj);
                context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion


        #region AddOrderItem - Adds an item to an order
        public bool AddOrderItem(OrderItem orderItemObj)
        {
            bool status = false;
            try
            {
                context.OrderItems.Add(orderItemObj);
                context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion


        #region GetOrderHistory - Gets order history for a user
        public List<Order> GetOrderHistory(int userId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                var parameter = new SqlParameter("@UserID", userId);

                // Get orders
                orders = context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                // Get order items for each order
                foreach (var order in orders)
                {
                    order.OrderItems = context.OrderItems
                        .Where(oi => oi.OrderId == order.OrderId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                orders = new List<Order>();
            }
            return orders;
        }
        #endregion

        #region GetAllCategories
        public List<Category> GetAllCategories()
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Category> categories = new List<Category>();
            try
            {
                categories = (from category in context.Categories select category).ToList();
            }
            catch (Exception ex)
            {
                categories = new List<Category>();
                Console.WriteLine(ex.Message);
            }
            return categories;
        }
        #endregion

        #region GetProductImages - Gets all images for a product
        public List<ProductImage> GetProductImages(int productId)
        {
            List<ProductImage> images = new List<ProductImage>();
            try
            {

                images = context.ProductImages.Where(p => p.ProductId == productId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                images = new List<ProductImage>();
            }
            return images;
        }
        #endregion

        #region GetProductsCount - Gets count of products based on filters
        public int GetProductsCount(int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            int count = 0;
            try
            {
                var query = context.Products.AsQueryable();
                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }
                count = query.Count();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                count = 0;
            }
            return count;
        }
        #endregion

        #region AddProduct - Adds a new product
        public int AddProduct(Product productObj)
        {
            int productId = 0;
            try
            {
                context.Products.Add(productObj);
                context.SaveChanges();
                productId = productObj.ProductId;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                productId = 0;
            }
            return productId;
        }
        #endregion

        #region DeleteProduct - Deletes a product
        public bool DeleteProduct(int productId)
        {
            bool status = false;
            try
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                    status = true;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region GetProductsByCategory
        public List<Product> GetProductsByCategory(int categoryId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Product> products = new List<Product>();
            try
            {
                products = (from product in context.Products
                            where product.CategoryId == categoryId
                            select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Product>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region GetProductsByPriceRange
        public List<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Product> products = new List<Product>();
            try
            {
                products = (from product in context.Products
                            where product.Price >= minPrice && product.Price <= maxPrice
                            select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Product>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region UpdateProduct
        public bool UpdateProduct(Product product)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var existingProduct = context.Products.Find(product.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ModelUrl = product.ModelUrl;
                    existingProduct.ThumbnailUrl = product.ThumbnailUrl;
                    existingProduct.Quantity = product.Quantity;

                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

    }
}
