using BulkyWeb.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _db;

        public Category Category { get; set; }

        public DeleteModel(ApplicationDBContext db)
        {
            _db = db;
        }

        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = _db.Categories.FirstOrDefault(c => c.Id == id);
            }
        }

        public IActionResult OnPost()
        {
            Category? cat = _db.Categories.Find(Category.Id);
            if (cat == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(cat);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted sucessfully";
            return RedirectToPage("Index");
        }
    }
}
