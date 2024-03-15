using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;    
        }

        public IActionResult Index()
        {
            var amenitites = _unitOfWork.Amenity.GetAll(inculdeProperties: "Villa");
            return View(amenitites);
        }

        public IActionResult Create() 
        {
            AmenityVM amenitiyVM = new AmenityVM
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };

            return View(amenitiyVM);
        }
          
        

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {

            if(ModelState.IsValid )
            {
                _unitOfWork.Amenity.Add(obj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been created successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
           
            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),

            });

            return View(obj);  
         
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenitiyVM = new AmenityVM
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                Amenity = _unitOfWork.Amenity.Get(u=> u.Id == amenityId)
            };
            if(amenitiyVM.Amenity == null) 
            { return RedirectToAction("Error", "Home"); }
            return View(amenitiyVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenitiyVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(amenitiyVM.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been updated successfully";
                return RedirectToAction(nameof(Index) /*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }

            amenitiyVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),

            });

            return View(amenitiyVM);
        }


        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenitiyVM = new AmenityVM
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (amenitiyVM.Amenity == null)
            { return RedirectToAction("Error", "Home"); }

            return View(amenitiyVM);
        }


        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            Amenity? objFromDb = _unitOfWork.Amenity.Get(u=> u.Id == obj.Amenity.Id);
            if (objFromDb is not null)
            {
                _unitOfWork.Amenity.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "The amenity has been deleted successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            TempData["error"] = "The amenity colud not be deleted";

            return View();
        }
    }

}
