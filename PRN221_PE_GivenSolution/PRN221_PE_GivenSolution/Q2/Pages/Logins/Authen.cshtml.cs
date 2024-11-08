using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;
using Microsoft.AspNetCore.Http;

namespace Q2.Pages.Logins
{
    public class AuthenModel : PageModel
    {
        private readonly DBContext _context;

        public AuthenModel()
        {
            _context = new DBContext();
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var member = _context.Members.FirstOrDefault(x => x.Email == Email && x.Password == Password);
            if (member == null)
            {
                // Trả về lỗi nếu không tìm thấy thành viên
                return new JsonResult(new { success = false, message = "Email hoặc mật khẩu không đúng." });
            }
            else
            {
                // Đăng nhập thành công, bạn có thể lưu thông tin người dùng vào session hoặc cookie
                HttpContext.Session.SetString("UserEmail", member.Email);

                // Trả về kết quả thành công
                return new JsonResult(new { success = true });
            }
        }
    }
}
