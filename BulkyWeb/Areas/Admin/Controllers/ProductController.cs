using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

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
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
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
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files) // Using ProductVM
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0) //Add
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Created sucessfully";
                }
                else // Update
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Updated sucessfully";
                }

                _unitOfWork.Save();

                //Manipulating IMageUrl field
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id);
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var filesStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(filesStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }
                        productVM.Product.ProductImages.Add(productImage);
                        
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                    
                }
                TempData["success"] = "Product Created/ Updated";
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
         * and Delete with API Call below 
        
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

            // Delete ImageUrl first 
            //string productPath = @"images\products\product-" + id;
            //string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            //if (Directory.Exists(finalPath))
            //{
            //    string[] filePaths = Directory.GetFiles(finalPath);
            //    foreach (string filePath in filePaths)
            //    {
            //        System.IO.File.Delete(filePath);
            //    }
            //    Directory.Delete(finalPath);
            //}

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
