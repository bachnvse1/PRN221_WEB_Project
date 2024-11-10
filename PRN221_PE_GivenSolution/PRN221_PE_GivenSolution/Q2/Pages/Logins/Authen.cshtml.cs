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
            HttpContext.Session.Clear();

        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var member = _context.Members
                .FirstOrDefault(x => x.Email == Email && x.Password == Password);

            if (member == null)
            {
                return new JsonResult(new { success = false, message = "Email hoặc mật khẩu không đúng." });
            }
            else
            {

                HttpContext.Session.SetString("UserEmail", member.Email);
                HttpContext.Session.SetInt32("UserId", member.MemberId); 


                return new JsonResult(new { success = true });
            }
        }
    }
}
