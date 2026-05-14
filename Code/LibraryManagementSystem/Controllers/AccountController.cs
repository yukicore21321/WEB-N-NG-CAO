using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AccountController(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IMemoryCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                    if (result.Succeeded)
                    {
                        if (user.MustChangePassword)
                        {
                            TempData["UserId"] = user.Id;
                            // Gửi OTP để đổi mật khẩu lần đầu
                            var otp = new Random().Next(100000, 999999).ToString();
                            _cache.Set($"OTP_ChangePwd_{user.Id}", otp, TimeSpan.FromMinutes(10));
                            await _emailService.SendEmailAsync(user.Email!, "Xác thực đổi mật khẩu lần đầu", otp, user.FullName);
                            
                            return RedirectToAction("VerifyOTPChangePassword");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }

        [HttpGet]
        public IActionResult VerifyOTPChangePassword()
        {
            if (TempData["UserId"] == null) return RedirectToAction("Login");
            TempData.Keep("UserId");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTPChangePassword(string otp)
        {
            var userId = TempData["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");

            if (_cache.TryGetValue($"OTP_ChangePwd_{userId}", out string? cachedOtp) && cachedOtp == otp)
            {
                TempData["UserId"] = userId;
                TempData["Verified"] = true;
                return RedirectToAction("ForceChangePassword");
            }

            ViewBag.Error = "Mã OTP không chính xác.";
            TempData.Keep("UserId");
            return View();
        }

        [HttpGet]
        public IActionResult ForceChangePassword()
        {
            if (TempData["UserId"] == null || TempData["Verified"] == null) return RedirectToAction("Login");
            TempData.Keep("UserId");
            TempData.Keep("Verified");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForceChangePassword(string newPassword, string confirmPassword)
        {
            var userId = TempData["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                TempData.Keep("UserId");
                TempData.Keep("Verified");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    user.MustChangePassword = false;
                    user.LastPasswordChange = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    // ĐỒNG BỘ MẬT KHẨU SANG BẢNG EMPLOYEES
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == user.UserName);
                    if (employee != null)
                    {
                        employee.Password = newPassword; // Lưu mật khẩu mới vào bảng Employee
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Admin");
                }
            }

            ViewBag.Error = "Đã có lỗi xảy ra.";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user != null)
            {
                var otp = new Random().Next(100000, 999999).ToString();
                _cache.Set($"OTP_{user.Email}", otp, TimeSpan.FromMinutes(10));
                
                await _emailService.SendEmailAsync(user.Email!, "Mã xác thực đặt lại mật khẩu", otp, user.FullName);
                
                TempData["Email"] = user.Email;
                TempData["Flow"] = "ForgotPassword";
                return RedirectToAction("VerifyOTP");
            }
            
            ViewBag.Error = "Không tìm thấy người dùng.";
            return View();
        }

        [HttpGet]
        public IActionResult VerifyOTP()
        {
            if (TempData["Email"] == null) return RedirectToAction("Login");
            TempData.Keep("Email");
            TempData.Keep("Flow");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var email = TempData["Email"]?.ToString();
            var flow = TempData["Flow"]?.ToString();
            
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            if (_cache.TryGetValue($"OTP_{email}", out string? cachedOtp) && cachedOtp == otp)
            {
                _cache.Remove($"OTP_{email}");
                
                if (flow == "Register")
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Login");
                    }
                }
                else if (flow == "ForgotPassword")
                {
                    TempData["Email"] = email;
                    return RedirectToAction("ResetPassword");
                }
            }

            ViewBag.Error = "Mã OTP không chính xác hoặc đã hết hạn.";
            TempData.Keep("Email");
            TempData.Keep("Flow");
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["Email"] == null) return RedirectToAction("Login");
            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                TempData.Keep("Email");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }

            ViewBag.Error = "Đã có lỗi xảy ra.";
            return View();
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Register(string name, string email, string username, string role)
        {
            // Kiểm tra phân quyền tạo account
            if (User.IsInRole("Staff") && role != "User")
            {
                ViewBag.Error = "Nhân viên chỉ có thể tạo tài khoản cho Khách hàng.";
                return View();
            }

            var user = new ApplicationUser { 
                UserName = username, 
                Email = email, 
                FullName = name,
                MustChangePassword = (role != "User") // Khách hàng ko cần đổi pass, nhân viên thì có
            };

            // Mật khẩu mặc định tạm thời, user sẽ đổi sau
            var tempPass = "Temp@123";
            var result = await _userManager.CreateAsync(user, tempPass);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);

                var otp = new Random().Next(100000, 999999).ToString();
                _cache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(10));
                
                await _emailService.SendEmailAsync(email, "Mã xác thực tài khoản mới", otp, name);
                
                TempData["Email"] = email;
                TempData["Flow"] = "Register";
                return RedirectToAction("VerifyOTP");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
