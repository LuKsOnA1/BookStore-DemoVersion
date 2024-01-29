﻿using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Models.ViewModels;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
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
            // By this code we get data from Database and save it in objProductList...
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {      
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
               .GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //Create
				return View(productVM);
			}
            else
            {
                // Update 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
     
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile file)
        {

            // Here we check if information from fields are valid and if it is than we create new 
            // Category...
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }          
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
					_unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Category created successfully";

                }
                else
                {
					_unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Category updated successfully";

                }

                _unitOfWork.Save();
                //TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
				return View(productVM);
			}
        }


        #region API CALLS


        [HttpGet]
        public IActionResult getAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
		public IActionResult Delete(int? id)
		{
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = 
                Path.Combine(_webHostEnvironment.WebRootPath, 
                productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });

		}

		#endregion
	}
}