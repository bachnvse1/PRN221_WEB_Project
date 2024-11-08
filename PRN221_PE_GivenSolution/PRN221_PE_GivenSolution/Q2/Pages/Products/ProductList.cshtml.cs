/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Q2.Hubs;
using Q2.Model;
using System.Text.Json;

namespace Q2.Pages
{
    public class ProductListModel : PageModel
    {
        private readonly DBContext dbcontext;
        private readonly IHubContext<ProductsHub> hubContext;

        public ProductListModel(IHubContext<ProductsHub> hubContext)
        {
            this.dbcontext = new DBContext();
            this.hubContext = hubContext;
        }

        public List<Product> Products = new List<Product>();

        public List<Category> Categories = new List<Category>();

        public int categoryIdSelected { get; set; }

        public void OnGet(int categoryId = 0)
        {
            Categories = dbcontext.Categories.ToList();
            if(categoryId == 0)
            {
                Products = dbcontext.Products.ToList();
            } else
            {
                categoryIdSelected = categoryId;
                Products = dbcontext.Products.Where(x=>x.CategoryId == categoryId).ToList();
            }
        }

        public IActionResult OnGetAddToCart(int productId, int categoryId)
        {
            List<OrderDetail> orders = new List<OrderDetail>();
            if(HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
            } else orders = new List<OrderDetail>();

            OrderDetail order = orders.FirstOrDefault(x=>x.ProductId == productId);
            if(order != null)
            {
                order.Quantity++;
            } else
            {
                order = new OrderDetail();
                order.ProductId = productId;
                order.Quantity = 1;
                orders.Add(order);

            }

            HttpContext.Session.SetString("cart", JsonSerializer.Serialize(orders));
            categoryIdSelected = categoryId;
            return RedirectToPage("/Carts/Cart");
        }

        public IActionResult OnGetDeleteProduct(int productId, int categoryId)
        {
            Product product = dbcontext.Products.Include(x => x.OrderDetails).FirstOrDefault(x => x.ProductId == productId);
            if(product != null)
            {
                List<OrderDetail> orders = new List<OrderDetail>();
                if (HttpContext.Session.GetString("cart") != null)
                {
                    string data = HttpContext.Session.GetString("cart");
                    orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                }
                else orders = new List<OrderDetail>();

                OrderDetail order = orders.FirstOrDefault(x => x.ProductId == productId);
                if (order != null)
                {
                    orders.Remove(order);
                    HttpContext.Session.SetString("cart", JsonSerializer.Serialize(orders));
                }

                dbcontext.OrderDetails.RemoveRange(product.OrderDetails);
                dbcontext.Products.Remove(product);
                dbcontext.SaveChanges();
                hubContext.Clients.All.SendAsync("ProductDeleted", productId);
            }
           
            return RedirectToPage("", new {categoryId = categoryId});
        }
    }
}
*/