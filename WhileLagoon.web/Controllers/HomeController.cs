using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.web.Models;
using WhiteLagoon.web.VÝewModel;

namespace WhiteLagoon.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM homeVm = new HomeVM
            {
                VillaList = _unitOfWork.Villa.GetAll(inculdeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVm);
        }
        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.VillaList = _unitOfWork.Villa.GetAll(inculdeProperties: "VillaAmenity");
            foreach (var villa in homeVM.VillaList)
            {
                if(villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }
             
            return View(homeVM);
        } 


        [HttpPost]
        public IActionResult GetVillasByDate(int nights,DateOnly checkInDate)
        {
            Thread.Sleep(1000);
            var VillaList = _unitOfWork.Villa.GetAll(inculdeProperties: "VillaAmenity").ToList();


            foreach (var villa in VillaList)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }

            HomeVM homeVM = new HomeVM
            {
                VillaList = VillaList,
                CheckInDate = checkInDate,
                Nights = nights

            };

            return PartialView("_VillaList",homeVM);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
