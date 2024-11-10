using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Q2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Q2.Pages.Members
{
    public class MemberListModel : PageModel
    {
        private readonly DBContext _context;

        // List of members for display
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Role> Roles { get; set; } = new List<Role>();

        // Constructor to initialize DBContext
        public MemberListModel()
        {
            _context = new DBContext();
        }

        // OnGet method to fetch members
        public void OnGet()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");  // Lấy email người dùng từ session
            var isAdmin = userEmail == "admin@example.com";  // Kiểm tra xem người dùng có phải là admin không

            // Nếu người dùng là admin, lấy tất cả thành viên và vai trò
            if (isAdmin)
            {
                Members = _context.Members.Include(m => m.Role).ToList();  // Lấy tất cả thành viên và vai trò
                Roles = _context.Roles.ToList();  // Lấy tất cả vai trò
            }
            else
            {
                // Nếu không phải admin, chỉ lấy thông tin của người đăng nhập
                Members = _context.Members
                    .Where(m => m.Email == userEmail)  // Chỉ lấy thông tin của người dùng đang đăng nhập
                    .Include(m => m.Role)
                    .ToList();
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(
            string email,
            string companyName,
            string city,
            string country,
            int roleId,
            string password)  // Removed image
        {
            // Check if the required fields are missing
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country) || roleId == 0 || string.IsNullOrEmpty(password))
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            // Check if the email already exists in the database
            var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (existingMember != null)
            {
                return new JsonResult(new { success = false, message = "Email đã tồn tại. Vui lòng chọn email khác." });
            }

            // Create a new Member object and assign values
            var newMember = new Member
            {
                Email = email,
                CompanyName = companyName,
                City = city,
                Country = country,
                RoleId = roleId,
                Password = password  // You may want to hash the password before saving
            };

            // Add the new member to the database
            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            // Return a success response
            return new JsonResult(new { success = true, message = "Thêm thành viên thành công!" });
        }



        // Handle deleting a member
        public async Task<IActionResult> OnPostDeleteAsync(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
