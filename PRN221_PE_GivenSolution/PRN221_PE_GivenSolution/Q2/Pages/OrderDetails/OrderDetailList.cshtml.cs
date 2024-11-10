using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Q2.Model;

namespace Q2.Pages.OrderDetails
{
    public class OrderDetailListModel : PageModel
    {
        private readonly DBContext context;

        public OrderDetailListModel()
        {
            this.context = new DBContext();
        }

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public void OnGet()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");  // Lấy email từ session
            var isAdmin = userEmail == "admin@example.com";  // Kiểm tra xem người dùng có phải là admin không

            if (isAdmin)
            {
                // Nếu là admin, lấy tất cả các chi tiết đơn hàng
                OrderDetails = context.OrderDetails.Include(od => od.Order).ThenInclude(o => o.Member).ToList();
            }
            else
            {
                // Nếu không phải admin, lấy chi tiết đơn hàng của người dùng đó
                OrderDetails = context.OrderDetails
                    .Where(od => od.Order.Member.Email == userEmail)  // Lọc theo email của khách hàng
                    .Include(od => od.Order)  // Lấy thông tin đơn hàng
                    .ThenInclude(o => o.Member)  // Lấy thông tin khách hàng
                    .ToList();
            }
        }
    }
}
