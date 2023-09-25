namespace MVC6CRUD.Models
{
    public class CategoryListViewModel
    {
        public IQueryable<CategoryViewModel> CategoryListViewModels { get; set; }
        public string SearchStrings { get; set; }
        public string CatograyNameShortOrder { get; set; }
        public string OrderBy { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
