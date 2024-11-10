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

            OrderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);

            if (OrderDetail == null)
            {
                return NotFound();
            }


            Products = _context.Products.ToList();

            return Page();
        }

        public IActionResult OnPostEditOrderDetailAsync()
        {

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


            orderDetailFromDb.UnitPrice = unitPrice;
            orderDetailFromDb.Quantity = quantity;
            orderDetailFromDb.Discount = discount;


            _context.SaveChanges();

            return RedirectToPage("/OrderDetails/OrderDetailList");
        }
    }
}
