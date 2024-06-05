using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfwork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfwork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfwork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        
        // Using ProductVM
        public IActionResult Upsert(int? id)  // Update and Insert
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0) // Create
            {
                return View(productVM);
            }
            else // Update 
            {
                productVM.Product = _unitOfwork.Product.Get(u => u.Id ==id);
                return View( productVM );
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file) // Using ProductVM
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if( file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);    
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    
                    // if there is a Image saved 
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // Delete old Image
                        var oldImagePath = 
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var filesStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filesStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                
                if( productVM.Product.Id == 0) //Add
                {
                    _unitOfwork.Product.Add(productVM.Product);
                }
                else // Update
                {
                    _unitOfwork.Product.Update(productVM.Product);
                }
               
                _unitOfwork.Save();
                TempData["success"] = "Product Created sucessfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        /* Replaced Edit above in Upsert ------------------------------ 
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
        */

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

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfwork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        
        #endregion
    }
}
