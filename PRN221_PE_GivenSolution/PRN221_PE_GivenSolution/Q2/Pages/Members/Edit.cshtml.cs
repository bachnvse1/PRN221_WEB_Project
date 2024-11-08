using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;

namespace Q2.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly DBContext _context;

        public EditModel()
        {
            _context = new DBContext();
        }

        [BindProperty]
        public Member Member { get; set; }

        public List<Role> Roles { get; set; }

        public IActionResult OnGet(int memberId)
        {
            // Lấy thông tin thành viên từ database
            Member = _context.Members.FirstOrDefault(m => m.MemberId == memberId);

            if (Member == null)
            {
                return NotFound();
            }

            // Lấy danh sách vai trò
            Roles = _context.Roles.ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var memberFromDb = _context.Members.FirstOrDefault(m => m.MemberId == Member.MemberId);
            if (memberFromDb == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin thành viên
            memberFromDb.Email = Member.Email;
            memberFromDb.CompanyName = Member.CompanyName;
            memberFromDb.City = Member.City;
            memberFromDb.Country = Member.Country;
            memberFromDb.RoleId = Member.RoleId;

            _context.SaveChanges();

            // Chuyển hướng về danh sách thành viên
            return RedirectToPage("/Member/MemberList");
        }
    }
}
