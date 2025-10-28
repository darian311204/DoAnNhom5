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
    }
}
