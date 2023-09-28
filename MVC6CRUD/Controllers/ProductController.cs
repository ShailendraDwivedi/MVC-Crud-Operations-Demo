using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MVC6CRUD.Data;
using MVC6CRUD.Models;
using MVC6CRUD.Repository;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;

namespace MVC6CRUD.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        public ProductController(IRepository<Category> categoryRepository, IRepository<Product> productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        [HttpGet]
        // GET: ProductController
        public IActionResult Index(string searchString = "", string sortOrder = "", string currentFilter = "", int pageNumber = 1)
        {
            var productData = new ProductListViewModel();
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();
            productData.SearchStrings = searchString;
            productData.ProductNameShortOrder = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            productData.CatograyNameShortOrder = sortOrder == "cat" ? "cat_desc" : "cat";
            var models = _productRepository.GetAll().Where(p => p.ProductName.ToLower().StartsWith(searchString) || searchString == "");
            var productLists = (from prduct in models
                                select new ProductViewModel
                                {
                                    Id = prduct.Id,
                                    ProductName = prduct.ProductName,
                                    Description = prduct.Description,
                                    Color = prduct.Color,
                                    Price = prduct.Price,
                                    ProductImage = prduct.Image,
                                    CategoryName = _categoryRepository.GetAll().Where(c => c.Id == prduct.CategoryId).Select(c => c.CategoryName).FirstOrDefault()
                                });
            switch (sortOrder)
            {
                case "name_desc":
                    productLists = productLists.OrderByDescending(s => s.ProductName);
                    break;
                case "cat":
                    productLists = productLists.OrderBy(s => s.CategoryName);
                    break;
                case "cat_desc":
                    productLists = productLists.OrderByDescending(s => s.CategoryName);
                    break;
                default:
                    productLists = productLists.OrderBy(s => s.ProductName);
                    break;
            }
            LoadCategory();
            int totalRecords = productLists.Count();
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            productLists = productLists.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            productData.ProductViewModels = productLists.AsQueryable();
            productData.CurrentPage = pageNumber;
            productData.TotalPages = totalPages;
            productData.PageSize = pageSize;
            productData.OrderBy = sortOrder;
            return View(productData);
        }

        // GET: ProductController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }
        [HttpGet]
        // GET: ProductController/AddProduct
        public IActionResult AddProduct()
        {
            ProductViewModel addProductViewModel = new ProductViewModel();
            //addProductViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
            //{
            //    Text = c.CategoryName,
            //    Value = c.Id.ToString()
            //});
            return PartialView("_AddProduct.cshtml", addProductViewModel);
        }
        [NonAction]
        private void LoadCategory()
        {
            var categories = _categoryRepository.GetAll().ToList();
            ViewBag.Category = new SelectList(categories, "Id", "CategoryName");
        }
        // POST: ProductController/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(ProductViewModel addProductViewModel)
        {
            try
            {
                //addProductViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
                //{
                //    Text = c.CategoryName,
                //    Value = c.Id.ToString()
                //});
                if (ModelState.IsValid)
                {
                    Product product = new Product()
                    {
                        ProductName = addProductViewModel.ProductName,
                        Description = addProductViewModel.Description,
                        Price = addProductViewModel.Price,
                        Color = addProductViewModel.Color,
                        Image = addProductViewModel.ProductImage,
                        CategoryId = Convert.ToInt64(addProductViewModel.CategoryId),                        
                        AddedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    _productRepository.Insert(product);
                    TempData["successMessage"] = "Product (" + product.ProductName + ") added successfully.";
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
        // GET: ProductController/EditProduct/5
        public IActionResult EditProduct(int id)
        {
            var productToEdit = _productRepository.GetById(id);

            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    ProductName = productToEdit.ProductName,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = (int)productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    ProductImage = productToEdit.Image,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
                    {
                        Text = c.CategoryName,
                        Value = c.Id.ToString()
                    }).ToList()

                };
                return PartialView("_EditProduct", productViewModel);
            }
            else
            {
                return PartialView("_EditProduct");
            }
        }

        // POST: ProductController/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(int id, ProductViewModel productViewModel)
        {
            productViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            });
            try
            {
                var product = new Product()
                {
                    Id = productViewModel.Id,
                    ProductName = productViewModel.ProductName,
                    Description = productViewModel.Description,
                    Price = productViewModel.Price,
                    Color = productViewModel.Color,
                    CategoryId = Convert.ToInt64(productViewModel.CategoryId),
                    Image = productViewModel.ProductImage
                };
                ModelState.Remove("Category");
                if (ModelState.IsValid)
                {
                    _productRepository.Update(product);
                    TempData["successMsg"] = "Product (" + product.ProductName + ") updated successfully !";
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

        // GET: ProductController/DeleteProduct/5
        public ActionResult Delete(int id)
        {
            var productToDelete = _productRepository.GetById(id);
            if (productToDelete != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToDelete.Id,
                    ProductName = productToDelete.ProductName,
                    Description = productToDelete.Description,
                    Price = productToDelete.Price,
                    CategoryId = (int)productToDelete.CategoryId,
                    Color = productToDelete.Color,
                    ProductImage = productToDelete.Image,
                    Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
                    {
                        Text = c.CategoryName,
                        Value = c.Id.ToString()
                    })
                };
                return View(productViewModel);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductController/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                Product product = _productRepository.GetById(id);
                _productRepository.Delete(product);
                TempData["successMessage"] = "Category Deleted Successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        // GET: ProductController/AddProduct
        public IActionResult Create()
        {
            ProductViewModel addProductViewModel = new ProductViewModel();
            addProductViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            });
            return View(addProductViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel addProductViewModel)
        {
            try
            {
                addProductViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
                {
                    Text = c.CategoryName,
                    Value = c.Id.ToString()
                });
                var product = new Product()
                {
                    ProductName = addProductViewModel.ProductName,
                    Description = addProductViewModel.Description,
                    Price = addProductViewModel.Price,
                    Color = addProductViewModel.Color,
                    CategoryId = Convert.ToInt64(addProductViewModel.CategoryId),
                    Image = addProductViewModel.ProductImage
                };
                ModelState.Remove("Category");
                if (ModelState.IsValid)
                {
                    _productRepository.Insert(product);
                    TempData["successMsg"] = "Product (" + product.ProductName + ") added successfully.";
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
        public IActionResult Edit(int id)
        {
            var productToEdit = _productRepository.GetById(id);
            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    ProductName = productToEdit.ProductName,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = (int)productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    ProductImage = productToEdit.Image,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
                    {
                        Text = c.CategoryName,
                        Value = c.Id.ToString()
                    }).ToList(),
                };
                return View(productViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductViewModel productViewModel)
        {
            productViewModel.Category = (IEnumerable<SelectListItem>)_categoryRepository.GetAll().Select(c => new SelectListItem()
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            });
            try
            {
                var product = new Product()
                {
                    Id = productViewModel.Id,
                    ProductName = productViewModel.ProductName,
                    Description = productViewModel.Description,
                    Price = productViewModel.Price,
                    Color = productViewModel.Color,
                    CategoryId = Convert.ToInt64(productViewModel.CategoryId),
                    Image = productViewModel.ProductImage
                };
                ModelState.Remove("Category");
                if (ModelState.IsValid)
                {
                    _productRepository.Update(product);
                    TempData["successMsg"] = "Product (" + product.ProductName + ") updated successfully !";
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
    }
}
