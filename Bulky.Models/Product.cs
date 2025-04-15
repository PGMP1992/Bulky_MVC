using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ISBN { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }

        [MaxLength(50)]
        public string? Genre { get; set; }

        public int Pages { get; set; }
        
        public DateTime? PublishDate { get; set; }

        public bool Complete { get; set; } = false;

        [Required]
        [Display(Name = "Price 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "List for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "List for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        public int CategoryId {  get; set; }
        
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category {  get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
