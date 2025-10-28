using DoAn_Frontend.Models;
using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Frontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService) => _apiService = apiService;

        public IActionResult Login()
        {
            if (_apiService.IsAuthenticated()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin!";
                return View(dto);
            }

            try
            {
                var result = await _apiService.LoginAsync(dto);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Đăng nhập thành công! Chào mừng " + result.User.FullName + "!";
                    return result.User.Role == "Admin" 
                        ? RedirectToAction("Dashboard", "Admin") 
                        : RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại!";
            }
            return View(dto);
        }

        public IActionResult Register()
        {
            if (_apiService.IsAuthenticated()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin!";
                return View(dto);
            }

            try
            {
                var result = await _apiService.RegisterAsync(dto);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login", "Auth");
                }
                ViewBag.Error = "Đăng ký thất bại! Email có thể đã tồn tại hoặc thông tin không hợp lệ.";
            }
            catch (Exception ex)
            {
                // Parse error message from backend
                if (ex.Message.Contains("Email already exists") || ex.Message.Contains("already exists"))
                {
                    ViewBag.Error = "Email này đã được sử dụng! Vui lòng chọn email khác hoặc đăng nhập.";
                }
                else if (ex.Message.Contains("Email"))
                {
                    ViewBag.Error = ex.Message;
                }
                else
                {
                    ViewBag.Error = "Đăng ký thất bại! " + ex.Message;
                }
            }
            return View(dto);
        }

        public IActionResult Logout()
        {
            _apiService.Logout();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");
            return View(_apiService.GetCurrentUser());
        }
    }
}
