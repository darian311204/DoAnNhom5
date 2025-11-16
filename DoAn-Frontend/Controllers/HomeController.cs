using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService) => _apiService = apiService;

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            ViewBag.IsAuthenticated = _apiService.IsAuthenticated();
            var products = await _apiService.GetProductsAsync();
            return View(products?.ToList() ?? new List<Models.Product>());
        }
    }
}