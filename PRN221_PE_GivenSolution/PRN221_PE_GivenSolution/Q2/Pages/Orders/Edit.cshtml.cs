using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;
using System.Linq;

namespace Q2.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly DBContext _context;

        public EditModel()
        {
            _context = new DBContext();
        }

        [BindProperty]
        public Order Order { get; set; }

        public IActionResult OnGet(int orderId)
        {
            // Retrieve order details by orderId
            Order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (Order == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orderFromDb = _context.Orders.FirstOrDefault(o => o.OrderId == Order.OrderId);
            if (orderFromDb == null)
            {
                return NotFound();
            }

            // Update order properties
            orderFromDb.MemberId = Order.MemberId;
            orderFromDb.OrderDate = Order.OrderDate;
            orderFromDb.RequiredDate = Order.RequiredDate;
            orderFromDb.ShippedDate = Order.ShippedDate;
            orderFromDb.Freight = Order.Freight;

            // Save changes to database
            _context.SaveChanges();

            // Redirect back to the order list page after saving
            return RedirectToAction("/Order/List");
        }
    }
}
