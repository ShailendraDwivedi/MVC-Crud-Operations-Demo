using Microsoft.AspNetCore.Mvc.Rendering;
using MVC6CRUD.Data;
using System.ComponentModel.DataAnnotations;

namespace MVC6CRUD.Models
{
    public class ProductViewModel
    {
        public long Id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }
        [Required]
        [Display(Name = "Product Description")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Product Color")]
        public string? Color { get; set; }
        [Required]
        [Display(Name = "Product Price")]
        public int Price { get; set; }
        [Required]
        [Display(Name = "Product Image")]
        public string? ProductImage { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public int CategoryId { get; set; }
        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }
        [Display(Name = "Added Date")]
        public DateTime AddedDate { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
        public IEnumerable<SelectListItem>? Category { get; set; }     
        
    }
}
