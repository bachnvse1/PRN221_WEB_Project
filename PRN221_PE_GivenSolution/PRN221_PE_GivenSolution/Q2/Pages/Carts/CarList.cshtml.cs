using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Q2.Model;
using System.Text.Json;

namespace Q2.Pages.Carts
{
    public class CartListModel : PageModel
    {
        private readonly DBContext dbcontext;
        public List<OrderDetail> orders = new List<OrderDetail>();
        public List<Product> products = new List<Product>();

        [BindProperty]
        public int ProductId { get; set; }
        [BindProperty]
        public int QuantityChange { get; set; }

        public CartListModel()
        {
            this.dbcontext = new DBContext();
        }

        public bool ShowCheckoutForm { get; set; } = false;

        public void OnGet()
        {
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                products = dbcontext.Products.ToList();
            }
        }

        public IActionResult OnPost(int productId, int quantityChange)
        {
            List<OrderDetail> orders = new List<OrderDetail>();
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
            }

            var order = orders.FirstOrDefault(o => o.ProductId == productId);

            if (order != null)
            {
                order.Quantity += quantityChange;

                if (order.Quantity < 1)
                {
                    order.Quantity = 1;
                }
            }
            else
            {
                order = new OrderDetail
                {
                    ProductId = productId,
                    Quantity = 1
                };
                orders.Add(order);
            }

            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(orders));

            return RedirectToPage("/Carts/CarList");
        }

        public IActionResult OnPostCheckout()
        {
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                products = dbcontext.Products.ToList();
            }
            ShowCheckoutForm = true;
            return Page();
        }

        public IActionResult OnPostCloseCheckout()
        {
            ShowCheckoutForm = false;
            return RedirectToPage("");
        }

        public IActionResult OnPostSubmitOrder(DateTime RequiredDate, DateTime ShippedDate)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var order = new Order
            {
                MemberId = userId.Value,
                OrderDate = DateTime.Now,
                RequiredDate = RequiredDate,
                ShippedDate = ShippedDate,
                Freight = 0
            };

            string data = HttpContext.Session.GetString("cart");

            var orderDetails = orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);

            if (orderDetails == null || !orderDetails.Any())
            {
                return RedirectToPage("/Cart");
            }

            foreach (var orderDetail in orderDetails)
            {
                var product = dbcontext.Products.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity,
                        UnitPrice = (decimal)product.UnitPrice,
                        Discount = orderDetail.Discount
                    });
                }
            }

            dbcontext.Orders.Add(order);
            dbcontext.SaveChanges();

            HttpContext.Session.Remove("cart");

            return RedirectToAction("/Home");
        }

    }
}
