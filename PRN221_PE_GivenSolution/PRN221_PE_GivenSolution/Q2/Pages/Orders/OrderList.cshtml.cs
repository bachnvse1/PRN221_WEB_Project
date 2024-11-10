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
          
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var isAdmin = userEmail == "admin@example.com"; 

            if (isAdmin)
            {
             
                Orders = dbContext.Orders.Include(o => o.Member).ToList();
            }
            else
            {
                
                var userId = HttpContext.Session.GetInt32("UserId");
                Orders = dbContext.Orders.Where(o => o.MemberId == userId).ToList();
            }
        }

        public IActionResult OnPostDelete(int orderId)
        {
           
            var order = dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }

           
            dbContext.Orders.Remove(order);
            dbContext.SaveChanges();

           
            return RedirectToAction("/Order/List");
        }
    }
}
