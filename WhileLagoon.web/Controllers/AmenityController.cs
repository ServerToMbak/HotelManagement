using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IAmenityService _amenityService;
        private readonly IVillaService _villaService;
        public AmenityController(IAmenityService amenityService, IVillaService villaService)
        {
            _amenityService = amenityService;
            _villaService = villaService;
        }

        public IActionResult Index()
        {
            var amenitites = _amenityService.GetAllAmenities();
            return View(amenitites);
        }

        public IActionResult Create() 
        {
            AmenityVM amenitiyVM = new AmenityVM
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
               _amenityService.Create(obj.Amenity);
                TempData["success"] = "The amenity has been created successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
           
            obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                Amenity =  _amenityService.GetAmenityById(amenityId)
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
                _amenityService.Update(amenitiyVM.Amenity);
                TempData["success"] = "The amenity has been updated successfully";
                return RedirectToAction(nameof(Index) /*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }

            amenitiyVM.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                Amenity = _amenityService.GetAmenityById(amenityId)
            };
            if (amenitiyVM.Amenity == null)
            { return RedirectToAction("Error", "Home"); }

            return View(amenitiyVM);
        }


        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            Amenity? objFromDb = _amenityService.GetAmenityById(obj.Amenity.Id);
            if (objFromDb is not null)
            {
                _amenityService.Delete(objFromDb);
                TempData["success"] = "The amenity has been deleted successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            TempData["error"] = "The amenity colud not be deleted";

            return View();
        }
    }

}
