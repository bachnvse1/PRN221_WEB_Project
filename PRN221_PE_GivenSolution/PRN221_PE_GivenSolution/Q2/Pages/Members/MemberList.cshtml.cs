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

        public MemberListModel()
        {
            _context = new DBContext();
        }

        // OnGet method to fetch members
        public void OnGet()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail"); 
            var isAdmin = userEmail == "admin@example.com";  

        
            if (isAdmin)
            {
                Members = _context.Members.Include(m => m.Role).ToList(); 
                Roles = _context.Roles.ToList();  
            }
            else
            {
               
                Members = _context.Members
                    .Where(m => m.Email == userEmail) 
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
            string password) 
        {
           
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country) || roleId == 0 || string.IsNullOrEmpty(password))
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

           
            var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (existingMember != null)
            {
                return new JsonResult(new { success = false, message = "Email đã tồn tại. Vui lòng chọn email khác." });
            }

            
            var newMember = new Member
            {
                Email = email,
                CompanyName = companyName,
                City = city,
                Country = country,
                RoleId = roleId,
                Password = password  
            };

            
            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            
            return new JsonResult(new { success = true, message = "Thêm thành viên thành công!" });
        }



      
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
