using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Frontend.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApiService _apiService;

        public ProductsController(ApiService apiService) => _apiService = apiService;

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.IsAuthenticated = _apiService.IsAuthenticated();

            List<Models.Product> products;
            if (!string.IsNullOrEmpty(search))
                products = await _apiService.SearchProductsAsync(search) ?? new List<Models.Product>();
            else if (categoryId.HasValue)
                products = (await _apiService.GetProductsAsync())?.Where(p => p.CategoryID == categoryId).ToList() ?? new List<Models.Product>();
            else
                products = await _apiService.GetProductsAsync() ?? new List<Models.Product>();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _apiService.GetProductAsync(id);
            if (product == null) return NotFound();

            ViewBag.Reviews = await _apiService.GetProductReviewsAsync(id) ?? new List<Models.Review>();
            ViewBag.IsLoggedIn = _apiService.IsAuthenticated();
            ViewBag.IsAuthenticated = _apiService.IsAuthenticated();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
        {
            if (!_apiService.IsAuthenticated())
            {
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập để đánh giá sản phẩm." });
            }

            if (request.Rating < 1 || request.Rating > 5)
            {
                return BadRequest(new { success = false, message = "Điểm đánh giá phải từ 1 đến 5 sao." });
            }

            var success = await _apiService.AddReviewAsync(request.ProductId, request.Rating, request.Comment ?? string.Empty);
            if (!success)
            {
                return BadRequest(new { success = false, message = "Không thể gửi đánh giá. Vui lòng thử lại." });
            }

            return Ok(new { success = true });
        }
    }

    public class ReviewRequest
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
