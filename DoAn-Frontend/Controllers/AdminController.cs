using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DoAn_Frontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AdminController> _logger;
        private readonly IReviewApiService _reviewApiService;
        private readonly ICategoryApiService _categoryApiService;
        // CHỈ GIỮ MỘT CONSTRUCTOR DUY NHẤT
        public AdminController(ApiService apiService, ILogger<AdminController> logger, IReviewApiService reviewApiService, ICategoryApiService categoryApiService)
        {
            _apiService = apiService;
            _logger = logger;
            _reviewApiService = reviewApiService;
            _categoryApiService = categoryApiService;
        }

        public IActionResult Dashboard()
        {
            _logger.LogInformation("Dashboard accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        public async Task<IActionResult> Products()
        {
            _logger.LogInformation("Products list accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            return View(await _apiService.GetProductsAsync() ?? new List<Models.Product>());
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            _logger.LogInformation("UploadImage called. File: {FileName}, Length: {Length}", file?.FileName, file?.Length);
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("UploadImage unauthorized access attempt.");
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadImage received empty file.");
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            var url = await _apiService.UploadImageAsync(file);
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("UploadImage failed in ApiService.UploadImageAsync.");
                return StatusCode(500, new { success = false, message = "Upload failed" });
            }

            _logger.LogInformation("UploadImage succeeded. Url={Url}", url);
            return Json(new { success = true, imageUrl = url });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Models.Product product)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });

            var success = await _apiService.CreateProductAsync(product);
            return Json(new { success, message = success ? "Product created successfully" : "Failed to create product" });
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });

            Console.WriteLine($"Admin GetProduct called for ID: {id}");

            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
            {
                Console.WriteLine($"Product {id} not found");
                return Json(new { success = false, message = "Product not found" });
            }

            Console.WriteLine($"Product {id} found: {product.ProductName}");
            return Json(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");

            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Models.Product product)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });

            var success = await _apiService.UpdateProductAsync(product);
            return Json(new { success, message = success ? "Product updated successfully" : "Failed to update product" });
        }

        public class DeleteProductRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("DeleteProduct unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("DeleteProduct called with Id={Id}", request.Id);
            var success = await _apiService.DeleteProductAsync(request.Id);
            _logger.LogInformation("DeleteProduct result for Id={Id}: {Success}", request.Id, success);
            return Json(new { success, message = success ? "Product deleted successfully" : "Failed to delete product" });
        }

        public async Task<IActionResult> Orders()
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View(await _apiService.GetAdminOrdersAsync() ?? new List<Models.Order>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("UpdateOrderStatus unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("UpdateOrderStatus called for OrderId={OrderId}, Status={Status}", request.OrderId, request.Status);
            var success = await _apiService.UpdateOrderStatusAsync(request.OrderId, request.Status, request.CancelReason);
            _logger.LogInformation("UpdateOrderStatus result for OrderId={OrderId}: {Success}", request.OrderId, success);
            return Json(new { success, message = success ? "Order status updated successfully" : "Failed to update order status" });
        }

        public class UpdateOrderStatusRequest
        {
            public int OrderId { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? CancelReason { get; set; }
        }

        // ==================== REVIEW MANAGEMENT ====================

        public async Task<IActionResult> Reviews()
        {
            _logger.LogInformation("Reviews list accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");

            var reviews = await _reviewApiService.GetAllReviewsAsync() ?? new List<Models.Review>();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReviewActive([FromBody] ToggleReviewRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("ToggleReviewActive unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("ToggleReviewActive called for ReviewId={ReviewId}", request.ReviewId);
            var success = await _reviewApiService.ToggleReviewActiveAsync(request.ReviewId);
            _logger.LogInformation("ToggleReviewActive result for ReviewId={ReviewId}: {Success}", request.ReviewId, success);

            return Json(new { success, message = success ? "Review status updated successfully" : "Failed to update review status" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview([FromBody] DeleteReviewRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("DeleteReview unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("DeleteReview called for ReviewId={ReviewId}", request.ReviewId);
            var success = await _reviewApiService.DeleteReviewAsync(request.ReviewId);
            _logger.LogInformation("DeleteReview result for ReviewId={ReviewId}: {Success}", request.ReviewId, success);

            return Json(new { success, message = success ? "Review deleted successfully" : "Failed to delete review" });
        }

        public class ToggleReviewRequest
        {
            public int ReviewId { get; set; }
        }

        public class DeleteReviewRequest
        {
            public int ReviewId { get; set; }
        }

        // Thêm vào AdminController.cs

        // ==================== CATEGORY MANAGEMENT ====================

        // ==================== CATEGORY MANAGEMENT ====================
        // Thêm vào AdminController.cs

        public async Task<IActionResult> Categories()
        {
            _logger.LogInformation("Categories list accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                var categories = await _categoryApiService.GetAllCategoriesAsync();

                if (categories == null)
                {
                    _logger.LogWarning("GetAllCategoriesAsync returned null");
                    categories = new List<Models.Category>();
                }

                _logger.LogInformation("Loaded {Count} categories for view", categories.Count);
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories view");
                return View(new List<Models.Category>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (!_apiService.IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            _logger.LogInformation("GetCategory called for ID: {Id}", id);

            try
            {
                var category = await _categoryApiService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category {Id} not found", id);
                    return Json(new { success = false, message = "Category not found" });
                }

                _logger.LogInformation("Category {Id} found: {Name}", id, category.CategoryName);

                // Trả về object với property names đúng chuẩn camelCase cho JavaScript
                return Json(new
                {
                    success = true,
                    categoryID = category.CategoryID,
                    categoryName = category.CategoryName,
                    description = category.Description,
                    isActive = category.IsActive,
                    productCount = category.ProductCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category {Id}", id);
                return Json(new { success = false, message = "An error occurred while loading category" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Models.Category category)
        {
            if (!_apiService.IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            // Validate input
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return Json(new { success = false, message = "Category name is required" });
            }

            if (category.CategoryName.Length > 100)
            {
                return Json(new { success = false, message = "Category name cannot exceed 100 characters" });
            }

            if (!string.IsNullOrEmpty(category.Description) && category.Description.Length > 500)
            {
                return Json(new { success = false, message = "Description cannot exceed 500 characters" });
            }

            _logger.LogInformation("CreateCategory called with name: {Name}", category.CategoryName);

            try
            {
                var success = await _categoryApiService.CreateCategoryAsync(category);

                return Json(new
                {
                    success,
                    message = success
                        ? "Category created successfully"
                        : "Failed to create category. Please check if the category name already exists."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return Json(new { success = false, message = "An error occurred while creating the category" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] Models.Category category)
        {
            if (!_apiService.IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            // Validate input
            if (category.CategoryID <= 0)
            {
                return Json(new { success = false, message = "Invalid category ID" });
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return Json(new { success = false, message = "Category name is required" });
            }

            if (category.CategoryName.Length > 100)
            {
                return Json(new { success = false, message = "Category name cannot exceed 100 characters" });
            }

            if (!string.IsNullOrEmpty(category.Description) && category.Description.Length > 500)
            {
                return Json(new { success = false, message = "Description cannot exceed 500 characters" });
            }

            _logger.LogInformation("UpdateCategory called for ID: {Id}", category.CategoryID);

            try
            {
                var success = await _categoryApiService.UpdateCategoryAsync(category);

                return Json(new
                {
                    success,
                    message = success
                        ? "Category updated successfully"
                        : "Failed to update category. Please check if the category name already exists."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", category.CategoryID);
                return Json(new { success = false, message = "An error occurred while updating the category" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteCategoryRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("DeleteCategory unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid category ID" });
            }

            _logger.LogInformation("DeleteCategory called with Id={Id}", request.Id);

            try
            {
                var success = await _categoryApiService.DeleteCategoryAsync(request.Id);

                _logger.LogInformation("DeleteCategory result for Id={Id}: {Success}", request.Id, success);

                return Json(new
                {
                    success,
                    message = success
                        ? "Category deleted successfully"
                        : "Failed to delete category. It may have associated products or doesn't exist."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", request.Id);
                return Json(new { success = false, message = "An error occurred while deleting the category" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCategoryStatus([FromBody] ToggleCategoryRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("ToggleCategoryStatus unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (request.CategoryId <= 0)
            {
                return Json(new { success = false, message = "Invalid category ID" });
            }

            _logger.LogInformation("ToggleCategoryStatus called for CategoryId={CategoryId}", request.CategoryId);

            try
            {
                var success = await _categoryApiService.ToggleCategoryStatusAsync(request.CategoryId);

                _logger.LogInformation("ToggleCategoryStatus result for CategoryId={CategoryId}: {Success}",
                    request.CategoryId, success);

                return Json(new
                {
                    success,
                    message = success
                        ? "Category status updated successfully"
                        : "Failed to update category status"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status {CategoryId}", request.CategoryId);
                return Json(new { success = false, message = "An error occurred while updating category status" });
            }
        }

        // Request models cho Category
        public class DeleteCategoryRequest
        {
            public int Id { get; set; }
        }

        public class ToggleCategoryRequest
        {
            public int CategoryId { get; set; }
        }
        [HttpGet]
        public IActionResult TestCategoryAuth()
        {
            var token = HttpContext.Session.GetString("Token");
            var user = _apiService.GetCurrentUser();

            return Json(new
            {
                hasToken = !string.IsNullOrEmpty(token),
                tokenLength = token?.Length ?? 0,
                isAuthenticated = !string.IsNullOrEmpty(token),
                isAdmin = user?.Role == "Admin",
                userRole = user?.Role,
                userName = user?.FullName
            });
        }
    }
}