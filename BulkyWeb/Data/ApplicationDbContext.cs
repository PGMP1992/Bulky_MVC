using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Data
{
    public class ApplicationDBContext :DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :base (options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Sci-Fi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Horror", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Comics", DisplayOrder = 5 },
                new Category { Id = 6, Name = "Romance", DisplayOrder = 6 }
            );
        }

    }
}
