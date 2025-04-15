using Stripe;

namespace Bulky.Models
{
    public class GoogleAPi
    {
        // - --------------------------------------------------------------------------------
        // - Open Library API response structure
        //public string? Title { get; set; }
        //public string? Author { get; set; }
        //public string? Genre { get; set; }
        //public int Pages { get; set; }
        //public string? PublishDate { get; set; }
        //public string? Cover { get; set; }
        // Add Description

        public string? Title { get; set; }
        public string? Author { get; set; } 
        public string? Description { get; set; } 
        public int Pages { get; set; }
        public string? PublishDate { get; set; }
        public string? Category { get; set; }
        public string? Price { get; set; }
    }
}
