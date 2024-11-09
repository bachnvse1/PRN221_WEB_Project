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
            OrderDetails = context.OrderDetails.ToList();
        }
    }
}
