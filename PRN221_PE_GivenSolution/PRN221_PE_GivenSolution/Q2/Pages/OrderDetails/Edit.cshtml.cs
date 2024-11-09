using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;

namespace Q2.Pages.OrderDetails
{
    public class EditModel : PageModel
    {
        private readonly DBContext _context;

        public EditModel()
        {
            _context = new DBContext();
        }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; }

        [BindProperty]
        public int ProductId { get; set; }

        public List<Product> Products { get; set; }

        public IActionResult OnGet(int orderId, int productId)
        {
            // Retrieve order detail by orderId and productId
            OrderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);

            if (OrderDetail == null)
            {
                return NotFound();
            }

            // Retrieve list of products
            Products = _context.Products.ToList();

            return Page();
        }

        public IActionResult OnPostEditOrderDetailAsync()
        {
            // Retrieve form values if BindProperty is not used for all fields
            var orderId = int.Parse(Request.Form["OrderId"]);
            var productId = int.Parse(Request.Form["ProductId"]);
            var unitPrice = decimal.Parse(Request.Form["UnitPrice"]);
            var quantity = int.Parse(Request.Form["Quantity"]);
            var discount = decimal.Parse(Request.Form["Discount"]);

            var orderDetailFromDb = _context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);
            if (orderDetailFromDb == null)
            {
                return NotFound();
            }

            // Update order detail fields
            orderDetailFromDb.UnitPrice = unitPrice;
            orderDetailFromDb.Quantity = quantity;
            orderDetailFromDb.Discount = discount;

            // Save changes to database
            _context.SaveChanges();

            // Redirect to the order detail list page
            return RedirectToPage("/OrderDetails/OrderDetailList");
        }
    }
}
