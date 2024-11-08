using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Q2.Model;
using System.Linq;

namespace Q2.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly DBContext _context;

        public IndexModel()
        {
            _context = new DBContext();
        }

        // Các thuộc tính sẽ lưu kết quả
        public string SearchQuery { get; set; }
        public string SelectedCategory { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<string> Categories { get; set; } = new List<string>();

        public void OnGet(string searchQuery, string selectedCategory)
        {
            // Lưu các giá trị từ URL hoặc form vào biến
            SearchQuery = searchQuery;
            SelectedCategory = selectedCategory;

            // Lấy danh sách các danh mục từ bảng Categories
            Categories = _context.Categories
                .Select(c => c.categoryName) // Lấy tên danh mục (hoặc theo cấu trúc bảng của bạn)
                .ToList();

            // Truy vấn sản phẩm dựa trên searchQuery và selectedCategory
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.ProductName.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                var category = _context.Categories.FirstOrDefault(c => c.categoryName == SelectedCategory);
                if (category != null)
                {
                    int categoryId = category.categoryId;
                    query = query.Where(p => p.CategoryId == categoryId); // Lọc theo tên danh mục
                }
            }

            Products = query.ToList();
        }


        public IActionResult OnGetProductDetails(int id)
        {
            var product = _context.Products.Where(x => x.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return new JsonResult(product);
        }

        public IActionResult OnPostProductUpdate(int? productId, string productName, string productPrice, string productWeight, string productImage)
        {
            if (!productId.HasValue)
            {
                return BadRequest("Product ID is required");
            }

            var product = _context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            decimal unitPrice = decimal.Parse(productPrice);
            decimal weight = decimal.Parse(productWeight);

            // Cập nhật sản phẩm
            product.ProductName = productName;
            product.UnitPrice = unitPrice;
            product.Weight = weight;
            product.Image = productImage;

            // Lưu vào CSDL
            _context.SaveChanges();
            return RedirectToPage("");
        }
    }
}
