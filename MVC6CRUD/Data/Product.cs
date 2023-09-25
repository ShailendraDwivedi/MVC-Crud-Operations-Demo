using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC6CRUD.Data
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        [MaxLength(50)]
        public string Color { get; set; }
        public int Price { get; set; }
        [Required]
        [MaxLength(250)]
        public string? Image { get; set; }
        [ForeignKey("Categories")]
        public long CategoryId { get; set; }
        
        public virtual Category Categories { get; set; }
    }
}
