using Bulky.DataAccess.Repository;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        
        // Using ProductVM
        public IActionResult Upsert(int? id)  // Update and Insert
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
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
                productVM.Product = _unitOfWork.Product.Get(u => u.Id ==id);
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
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else // Update
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Product Created sucessfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        /* Replaced Edit above in Upsert ------------------------------ 
         * and Deleted with API Call below 
        
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
        */

        // --------------------------------------------------

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
