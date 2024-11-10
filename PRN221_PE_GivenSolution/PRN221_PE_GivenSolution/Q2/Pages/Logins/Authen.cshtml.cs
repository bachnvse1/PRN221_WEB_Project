using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            // Xóa hết session khi đăng xuất
            HttpContext.Session.Clear();

        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            // Kiểm tra thông tin đăng nhập
            var member = _context.Members
                .FirstOrDefault(x => x.Email == Email && x.Password == Password); // Mật khẩu cần được mã hóa (hash)

            if (member == null)
            {
                // Trả về lỗi nếu không tìm thấy thành viên
                return new JsonResult(new { success = false, message = "Email hoặc mật khẩu không đúng." });
            }
            else
            {
                // Đăng nhập thành công, lưu thông tin vào session
                HttpContext.Session.SetString("UserEmail", member.Email);
                HttpContext.Session.SetString("UserId", member.MemberId.ToString()); // Nếu cần lưu ID người dùng

                // Trả về kết quả thành công
                return new JsonResult(new { success = true });
            }
        }
    }
}
