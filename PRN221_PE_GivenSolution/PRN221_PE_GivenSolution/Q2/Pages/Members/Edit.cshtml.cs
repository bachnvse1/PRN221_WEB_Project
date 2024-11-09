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

        [BindProperty]
        public IFormFile Image { get; set; }  // Thêm thuộc tính cho ảnh tải lên

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

        public IActionResult OnPostEditMemberAsync()
        {
            // Get values from the form without using BindProperty
            var memberId = int.Parse(Request.Form["MemberId"]);
            var email = Request.Form["Email"].ToString();
            var companyName = Request.Form["CompanyName"];
            var city = Request.Form["City"];
            var country = Request.Form["Country"];
            var roleId = int.Parse(Request.Form["RoleId"]);
            var password = Request.Form["Password"];
            var image = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;

            var memberFromDb = _context.Members.FirstOrDefault(m => m.MemberId == memberId);
            if (memberFromDb == null)
            {
                return NotFound();
            }

            // Check if the email already exists in another member, unless it’s the same as the current member's email
            var existingMember = _context.Members.FirstOrDefault(m => m.Email == email && m.MemberId != memberId);
            if (existingMember != null)
            {
                return new JsonResult(new { success = false, message = "Email đã tồn tại. Vui lòng chọn email khác." });
            }

            // Update member details
            memberFromDb.Email = email;
            memberFromDb.CompanyName = companyName;
            memberFromDb.City = city;
            memberFromDb.Country = country;
            memberFromDb.RoleId = roleId;

            // Update password if provided
            if (!string.IsNullOrEmpty(password))
            {
                memberFromDb.Password = password; // Update password if not empty
            }

            // Handle image update if new image is provided
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyToAsync(memoryStream).Wait();
                    memberFromDb.Image = memoryStream.ToArray();  // Save image as byte array
                }
            }

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to member list page after update
            return RedirectToPage("/Members/MemberList");
        }

    }
}
