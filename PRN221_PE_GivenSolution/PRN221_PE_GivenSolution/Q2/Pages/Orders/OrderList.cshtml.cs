using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Q2.Model;

namespace Q2.Pages.Orders
{
    public class OrderListModel : PageModel
    {
        private readonly DBContext dbContext;

        public OrderListModel()
        {
            dbContext = new DBContext();
        }

        public List<Order> Orders { get; set; } = new List<Order>();

        public void OnGet()
        {
            // Lấy email người dùng từ session
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var isAdmin = userEmail == "admin@example.com"; // Kiểm tra nếu là admin

            if (isAdmin)
            {
                // Nếu là admin, hiển thị tất cả các đơn hàng
                Orders = dbContext.Orders.Include(o => o.Member).ToList();
            }
            else
            {
                // Nếu không phải admin, chỉ hiển thị đơn hàng của người dùng hiện tại
                var userId = HttpContext.Session.GetString("UserId");
                Orders = dbContext.Orders.Where(o => o.MemberId.ToString() == userId).ToList();
            }
        }

        public IActionResult OnPostDelete(int orderId)
        {
            // Find order by orderId
            var order = dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Remove order from the database
            dbContext.Orders.Remove(order);
            dbContext.SaveChanges();

            // Refresh the page after deletion
            return RedirectToAction("/Order/List");
        }
    }
}
