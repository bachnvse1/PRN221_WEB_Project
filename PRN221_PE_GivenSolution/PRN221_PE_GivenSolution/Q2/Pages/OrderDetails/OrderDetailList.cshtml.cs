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
            var userEmail = HttpContext.Session.GetString("UserEmail"); 
            var isAdmin = userEmail == "admin@example.com"; 

            if (isAdmin)
            {
            
                OrderDetails = context.OrderDetails.Include(od => od.Order).ThenInclude(o => o.Member).ToList();
            }
            else
            {
              
                OrderDetails = context.OrderDetails
                    .Where(od => od.Order.Member.Email == userEmail)  
                    .Include(od => od.Order)  
                    .ThenInclude(o => o.Member) 
                    .ToList();
            }
        }
    }
}
