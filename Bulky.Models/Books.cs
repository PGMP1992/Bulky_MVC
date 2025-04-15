using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Books
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Author { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        public string? Cover { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? Pages { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublishDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string ISBN { get; set; }

        public bool Complete { get; set; } = false;

        [Required]
        public int UserId { get; set; }

        // Navigation property
        //[ValidateNever]
        //public User User { get; set; }
    }



}
