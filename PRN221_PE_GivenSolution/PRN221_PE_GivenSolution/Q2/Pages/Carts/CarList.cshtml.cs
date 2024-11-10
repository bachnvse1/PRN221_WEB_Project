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
        public int ProductId { get; set; } // ID sản phẩm cần thay đổi
        [BindProperty]
        public int QuantityChange { get; set; } // Thay đổi số lượng (1 hoặc -1)

        public CartListModel()
        {
            this.dbcontext = new DBContext();
        }

        public bool ShowCheckoutForm { get; set; } = false;

        public void OnGet()
        {
            // Kiểm tra xem giỏ hàng có tồn tại trong session không
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                products = dbcontext.Products.ToList();
            }
        }

        // Xử lý khi người dùng gửi form POST để thay đổi số lượng
        public IActionResult OnPost(int productId, int quantityChange)
        {
            // Deserialize the cart from session, if it exists
            List<OrderDetail> orders = new List<OrderDetail>();
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
            }

            // Find the product in the cart by ProductId
            var order = orders.FirstOrDefault(o => o.ProductId == productId);

            if (order != null)
            {
                // Change the quantity based on the value provided (positive or negative)
                order.Quantity += quantityChange;

                // Ensure that quantity is not less than 1
                if (order.Quantity < 1)
                {
                    order.Quantity = 1;
                }
            }

            // If the product is not in the cart, add it
            else
            {
                order = new OrderDetail
                {
                    ProductId = productId,
                    Quantity = 1 // Default to 1 if it's a new item
                };
                orders.Add(order);
            }

            // Save the updated cart back to session
            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(orders));

            // Redirect to the cart page to show the updated cart
            return RedirectToPage("/Carts/CarList");
        }

        public IActionResult OnPostCheckout()
        {
            // Kiểm tra xem giỏ hàng có tồn tại trong session không
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                products = dbcontext.Products.ToList();
            }
            // Show the checkout form
            ShowCheckoutForm = true;
            return Page();
        }

        public IActionResult OnPostCloseCheckout()
        {
            // Ẩn form Checkout
            ShowCheckoutForm = false;
            return RedirectToPage("");
        }

        public IActionResult OnPostSubmitOrder(DateTime RequiredDate, DateTime ShippedDate)
        {
            // Lấy userId từ session (ví dụ userId được lưu trong session với key "userId")
            int? userId = HttpContext.Session.GetInt32("userId");

            // Kiểm tra nếu userId không tồn tại hoặc không hợp lệ
            if (userId == null)
            {
                // Nếu không có userId trong session, có thể redirect về trang đăng nhập hoặc thông báo lỗi
                return RedirectToPage("/Login"); // Thay "/Login" bằng trang đăng nhập của bạn
            }

            // Tạo đơn hàng từ dữ liệu giỏ hàng
            var order = new Order
            {
                MemberId = userId.Value, // Lấy userId từ session
                OrderDate = DateTime.Now,
                RequiredDate = RequiredDate,
                ShippedDate = ShippedDate,
                Freight = 0 // Phí vận chuyển có thể được tính toán nếu cần
            };

            // Thêm chi tiết đơn hàng từ giỏ hàng
            foreach (var orderDetail in orders)
            {
                var product = products.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity,
                        //UnitPrice = product.UnitPrice
                    });
                }
            }

            // Lưu đơn hàng và chi tiết vào cơ sở dữ liệu
            dbcontext.Orders.Add(order);
            dbcontext.SaveChanges();

            // Xóa giỏ hàng trong session sau khi đặt hàng
            HttpContext.Session.Remove("cart");

            // Chuyển hướng tới trang xác nhận hoặc trang chủ
            return RedirectToPage("/Home");
        }
    }
}
