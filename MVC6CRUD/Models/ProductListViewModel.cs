namespace MVC6CRUD.Models
{
    public class ProductListViewModel
    {
        public IQueryable<ProductViewModel> ProductViewModels { get; set; }
        public string SearchStrings { get; set; }
        public string ProductNameShortOrder { get; set; }
        public string CatograyNameShortOrder { get; set; }
        public string OrderBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
