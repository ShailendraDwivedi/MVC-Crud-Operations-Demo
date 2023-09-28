using Microsoft.AspNetCore.Mvc;
using MVC6CRUD.Data;
using MVC6CRUD.Models;
using MVC6CRUD.Repository;

namespace MVC6CRUD.Controllers
{
    /// <summary>
    /// Category Controller
    /// </summary>
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();
            ViewData["CurrentFilter"] = searchString;
            var model = _categoryRepository.GetAll().Where(c => c.CategoryName.ToLower().StartsWith(searchString) || searchString == "").Select(s => new CategoryViewModel
            {
                CategoryId = s.Id,
                CategoryName = s.CategoryName
            });
            switch (sortOrder)
            {
                case "name_desc":
                    model = model.OrderByDescending(s => s.CategoryName);
                    break;
                default:
                    model = model.OrderBy(s => s.CategoryName);
                    break;
            }
            int pageSize = 3;
            return View(PaginatedList<CategoryViewModel>.Create(model.AsQueryable(), pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            CategoryViewModel model = new CategoryViewModel();
            return PartialView("_AddCategory", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = new Category
                    {
                        CategoryName = model.CategoryName,
                        AddedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    _categoryRepository.Insert(category);
                    TempData["successMessage"] = "Category Created Successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["errorMessage"] = "Model state is invalid!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public IActionResult EditCategory(int categoryId)
        {
            CategoryViewModel model = new CategoryViewModel();

            Category category = _categoryRepository.GetById(categoryId);
            {
                model.CategoryId = category.Id;
                model.CategoryName = category.CategoryName;
                model.AddedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            return PartialView("_EditCategory", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(int categoryId, CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = _categoryRepository.GetById(categoryId);
                    {
                        category.Id = categoryId;
                        category.CategoryName = model.CategoryName;
                        category.AddedDate = model.AddedDate;
                        category.ModifiedDate = DateTime.UtcNow;
                    }
                    _categoryRepository.Update(category);
                    TempData["successMessage"] = "Category Updated Successfully!";
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    TempData["errorMessage"] = "Model state is invalid!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Delete(int categoryId)
        {
            CategoryViewModel model = new CategoryViewModel();

            Category category = _categoryRepository.GetById(categoryId);
            {
                model.CategoryId = category.Id;
                model.CategoryName = category.CategoryName;
                model.AddedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            return PartialView("_DeleteCategory", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                Category category = _categoryRepository.GetById(categoryId);
                _categoryRepository.Delete(category);
                TempData["successMessage"] = "Category Deleted Successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
