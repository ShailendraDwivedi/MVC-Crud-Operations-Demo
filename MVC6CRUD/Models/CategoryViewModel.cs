using System.ComponentModel.DataAnnotations;

namespace MVC6CRUD.Models
{
    public class CategoryViewModel
    {
        [Display(Name = "Category Id")]
        public Int64 CategoryId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Display(Name = "Added Date")]
        public DateTime AddedDate { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
    }
}
