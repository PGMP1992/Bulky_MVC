using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string search)
        {
            //IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
            IEnumerable<Product> products = _unitOfWork.Product.GetSortedByName();
            ViewBag.Message = "";

            if (!String.IsNullOrEmpty(search))
            {
                var products1 = _unitOfWork.Product.GetByName(search);
                if (products1.Count > 0)
                {
                    return View(products1);
                }
                ViewBag.Message = "There are not books with that name!";
            }
            return View(products);
        }

        // Have to pass to Index view as asp-route-productId="@product.Id" otherwise it passes product as Null 
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Get UserId
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            // Check if UserId or product exists 
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.
                            Get(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);
            
            if( cartFromDb != null)
            {
                // Existing Cart
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                // Add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                // Start Session
                HttpContext.Session.SetInt32(SD.SessionCart, 
                                            _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Cart Updated";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
