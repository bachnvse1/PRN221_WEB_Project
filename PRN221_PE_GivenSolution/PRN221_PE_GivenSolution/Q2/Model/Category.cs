namespace Q2.Model
{
    public partial class Category
    {
        public int categoryId { get; set; }
        public string categoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
