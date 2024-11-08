/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;
using System.Text.Json;

namespace Q2.Pages
{
    public class CartListModel : PageModel
    {
        private readonly DBContext dbcontext;
        public List<OrderDetail> orders = new List<OrderDetail>();
        public List<Product> products = new List<Product>();

        public CartListModel()
        {
            this.dbcontext = new DBContext();
        }
        public void OnGet()
        {
            
            if (HttpContext.Session.GetString("cart") != null)
            {
                string data = HttpContext.Session.GetString("cart");
                orders = JsonSerializer.Deserialize<List<OrderDetail>>(data);
                products = dbcontext.Products.ToList();
            }
        }
    }
}
*/