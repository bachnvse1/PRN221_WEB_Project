using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Q2.Model;  // Thêm namespace của Model của bạn

namespace Q2.Pages.Members
{
    public class MemberListModel : PageModel
    {
        private readonly DBContext _context;

        // Thêm thuộc tính để lưu danh sách thành viên
        public List<Member> Members { get; set; } = new List<Member>();

        // Constructor nhận DbContext
        public MemberListModel()
        {
            _context = new DBContext();
        }

        // Phương thức OnGet để lấy dữ liệu thành viên
        public void OnGet()
        {
            // Lấy tất cả thành viên từ cơ sở dữ liệu (bạn có thể thay đổi câu lệnh này theo yêu cầu)
            Members = _context.Members.Include(m => m.Role).ToList();
        }

        public async Task<IActionResult> OnGetEditAsync(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound();
            }

            // Chuyển hướng tới trang Edit
            return RedirectToPage("/Member/Edit", new { memberId = member.MemberId });
        }

        // Action xử lý xóa (Delete)
        public async Task<IActionResult> OnPostDeleteAsync(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }

            // Sau khi xóa, quay lại danh sách thành viên
            return RedirectToPage();
        }
    }
}
