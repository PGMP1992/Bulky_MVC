using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfwork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfwork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfwork.Product.GetAll().ToList();
            return View(objProductList);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfwork.Category.GetAll()
                  .Select(u => new SelectListItem
                  {
                    Text = u.Name,
                    Value = u.Id.ToString()
                  });

            //ViewBag.CategoryList = CategoryList;
            ViewData["CategoryList"] = CategoryList;
                return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
                if (ModelState.IsValid)
                {
                    _unitOfwork.Product.Add(obj);
                    _unitOfwork.Save();
                    TempData["success"] = "Product Created sucessfully";
                    return RedirectToAction("Index", "Product");
                }
                return View();
            }

            public IActionResult Edit(int? id)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                Product? productFromDb = _unitOfwork.Product.Get(c => c.Id == id);

                if (productFromDb == null)
                {
                    return NotFound();
                }

                return View(productFromDb);
            }

            [HttpPost]
            public IActionResult Edit(Product obj)
            {
                if (ModelState.IsValid)
                {
                    _unitOfwork.Product.Update(obj);
                    _unitOfwork.Save();
                    TempData["success"] = "Product Edited sucessfully";
                    return RedirectToAction("Index", "Product");
                }
                return View();
            }

            public IActionResult Delete(int? id)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                Product? productFromDb = _unitOfwork.Product.Get(c => c.Id == id);

                if (productFromDb == null)
                {
                    return NotFound();
                }

                return View(productFromDb);
            }

            [HttpPost, ActionName("Delete")]
            public IActionResult DeletePost(int? id)
            {
                Product? obj = _unitOfwork.Product.Get(c => c.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                _unitOfwork.Product.Remove(obj);
                _unitOfwork.Save();
                TempData["success"] = "Product Deleted sucessfully";
                return RedirectToAction("Index", "Product");
            }
        }
}
