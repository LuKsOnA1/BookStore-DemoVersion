using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // By this code we get data from Database and save it in objCategoryList...
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        // To Create categories we need 2 Action: 1 - for View and 1 - to post...
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Here we check and make server side validation...
            if (obj.Name != null && obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order Can Not Match The Name!!!");
            }

            // Here we check if information from fields are valid and if it is than we create new 
            // Category...
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            // Than just return page again...
            return View();
        }
        public IActionResult Edit(int? id)
        {
            // Here we check Id is valid or not becouse we need Id ( Primary key ) to make Change in Database...
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Here we are trying to find Data in Database and store in variable based on Id...
            // This method works only with primary key...
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            // We check if there is some kind of problem about Data from Database...
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            // Than just return category based on id...
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                // Now we just use power of Entity Framework and Update category in Database...
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Here we are using same method to find category which is chosen by user...
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            // Help of Entity Framework again we remove and save changes in 2 lines and after delete we redirect 
            // to index page of category controller...
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
