using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDBContext _db;

        public ProductRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        // Have to Implement for ImageUrl 
        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Author = obj.Author;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.ProductImages = obj.ProductImages;
                objFromDb.Genre = obj.Genre;
                objFromDb.Pages = obj.Pages;
                objFromDb.PublishDate = obj.PublishDate;
                objFromDb.Complete = obj.Complete;
                //Cover = bookData["cover"]?.FirstOrDefault()?.ToString() //Added PM
            }
            //_db.Products.Update(obj);
        }

        public List<Product> GetByName(string name)
        {
            return _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Where(x => x.Title.ToLower().Contains(name.ToLower()))
                .ToList();
        }

        public List<Product> GetSortedByName()
        {
            return _db.Products
                .OrderBy(x => x.Title)
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .ToList();
        }
    }
}
