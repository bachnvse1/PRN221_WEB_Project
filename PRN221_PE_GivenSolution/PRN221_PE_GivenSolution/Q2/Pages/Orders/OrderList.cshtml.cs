using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            // Retrieve list of orders from the database
            Orders = dbContext.Orders.ToList();
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
