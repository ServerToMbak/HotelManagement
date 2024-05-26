using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.web.Models;
using WhiteLagoon.web.VİewModel;

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
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailable = false;
                }
            }

            return View(homeVM);
        }


        [HttpPost]
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {


            var VillaList = _unitOfWork.Villa.GetAll(inculdeProperties: "VillaAmenity").ToList();
            var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
            u.Status == SD.StatusCheckedIn).ToList();


            foreach (var villa in VillaList)
            {
                int roomAvailable = SD.VillaRoomsAvailableCount(villa.Id, villaNumberList, checkInDate, nights, bookedVillas);

                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            HomeVM homeVM = new HomeVM
            {
                VillaList = VillaList,
                CheckInDate =checkInDate,
                Nights = nights

            };

            return PartialView("_VillaList", homeVM);
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
