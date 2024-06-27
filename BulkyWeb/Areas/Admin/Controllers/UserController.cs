﻿using Bulky.DataAccess;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        private readonly ApplicationDBContext _db;
        //private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManage(string userId)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            RoleManagementVM RoleVM = new RoleManagementVM
            {
                ApplicationUser = _db.ApplicationUsers.Include(c => c.Company).FirstOrDefault(u => u.Id == userId),
                RoleList =_db.Roles.Select(i => new SelectListItem { 
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };
            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            
            return View(RoleVM);
        }


        // --------------------------------------------------

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u =>u.Company).ToList();
            // Using AspNetUserRoles and AspNetRoles tables
            // - excluding the AspNet from the table name for all Identity tables works  
            
            var userRoles = _db.UserRoles.ToList(); // Mapping table for users and Roles 
            var roles = _db.Roles.ToList(); 

            foreach (var user in objUserList) {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null) {
                    user.Company = new Company() {
                        Name = ""
                    };
                }
            }
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now) // User is Locked 
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
