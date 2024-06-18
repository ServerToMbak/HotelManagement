using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }

        public IActionResult Index()
        {
            var villaNumbes = _villaNumberService.GetAllVillaNumbers();
            return View(villaNumbes);
        }

        public IActionResult Create() 
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };

            return View(villaNumberVM);
        }
          
        

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            bool isvillanumberexist = _villaNumberService.CheckVillaNumberExists(obj.VillaNumber.Villa_Number);

        
            //ModelState.Remove("Villa"); we can use that to not take the instance Villa in VillaNumber Mod

            if(ModelState.IsValid && !isvillanumberexist)
            {
                _villaNumberService.CreateVillaNumber(obj.VillaNumber);
                TempData["success"] = "The villa Number has been created successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            if (isvillanumberexist)
            {
                TempData["error"] = "the villa number  already exists";
            }
            obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),

            });

            return View(obj);  
         
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };
            if(villaNumberVM.VillaNumber == null) 
            { return RedirectToAction("Error", "Home"); }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            if (ModelState.IsValid)
            {
                _villaNumberService.UpdateVillaNumber(villaNumberVM.VillaNumber);
                TempData["success"] = "The villa Number has been updated successfully";
                return RedirectToAction(nameof(Index) /*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }

            villaNumberVM.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),

            });

            return View(villaNumberVM);
        }


        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                VillaNumber = _villaNumberService.GetVillaNumberById(villaNumberId)
            };
            if (villaNumberVM.VillaNumber == null)
            { return RedirectToAction("Error", "Home"); }

            return View(villaNumberVM);
        }


        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            VillaNumber? objFromDb = _villaNumberService.GetVillaNumberById(obj.VillaNumber.Villa_Number);
            if (objFromDb is not null)
            {
                _villaNumberService.DeleteVillaNumber(objFromDb.Villa_Number);
                TempData["success"] = "The villa number has been deleted successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            TempData["error"] = "The villa colud not be deleted";

            return View();
        }
    }

}
