using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.web.Controllers
{
    
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                ModelState.AddModelError("name", "The description cannot exactly match the name"); // it associates the error to the description

                //ModelState.AddModelError("name", "The description cannot exactly match the name");
            }
            if(ModelState.IsValid)
            {
                if(obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    obj.Image.CopyTo(fileStream);

                    obj.ImageUrl = @"\images\VillaImage\" + fileName;


                }
                else
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }
                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been created successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
          
            return View(obj);  
         
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(p => p.Id == villaId);

            if(obj == null)
            {
                return RedirectToAction("Error","Home");
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {

            if (ModelState.IsValid)
            {

                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldImagePth = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.Trim('\\'));

                        if (System.IO.File.Exists(oldImagePth))
                        {
                            System.IO.File.Delete(oldImagePth);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    obj.Image.CopyTo(fileStream);

                    obj.ImageUrl = @"\images\VillaImage\" + fileName;
                }

                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been updated successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            return View();
        }


        public IActionResult Delete(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(p => p.Id == villaId);

            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(obj);
        }


        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFromDb = _unitOfWork.Villa.Get(u=> u.Id == obj.Id);
            if (objFromDb is not null)
            {

 
                    var oldImagePth = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePth))
                    {
                        System.IO.File.Delete(oldImagePth);
                    }
              
                _unitOfWork.Villa.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been deleted successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            TempData["error"] = "The villa colud not be deleted";

            return View();
        }
    }

}
