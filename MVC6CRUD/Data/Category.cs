using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC6CRUD.Data
{
    public class Category :BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }
    }
}
