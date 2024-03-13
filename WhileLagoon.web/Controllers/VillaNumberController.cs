using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;    
        }

        public IActionResult Index()
        {
            var villaNumbes = _unitOfWork.VillaNumber.GetAll(inculdeProperties: "Villa");
            return View(villaNumbes);
        }

        public IActionResult Create() 
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            bool isvillanumberexist = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

        
            //ModelState.Remove("Villa"); we can use that to not take the instance Villa in VillaNumber Mod

            if(ModelState.IsValid && !isvillanumberexist)
            {
                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been created successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            if (isvillanumberexist)
            {
                TempData["error"] = "the villa number  already exists";
            }
            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                VillaNumber = _unitOfWork.VillaNumber.Get(u=> u.Villa_Number == villaNumberId)
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
                _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa Number has been updated successfully";
                return RedirectToAction(nameof(Index) /*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }

            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),


                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };
            if (villaNumberVM.VillaNumber == null)
            { return RedirectToAction("Error", "Home"); }

            return View(villaNumberVM);
        }


        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get(u=> u.Villa_Number == obj.VillaNumber.Villa_Number);
            if (objFromDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been deleted successfully";
                return RedirectToAction(nameof(Index)/*,"Villa"*/); // Action Name and then Control Name(Control name is not required)
            }
            TempData["error"] = "The villa colud not be deleted";

            return View();
        }
    }

}
