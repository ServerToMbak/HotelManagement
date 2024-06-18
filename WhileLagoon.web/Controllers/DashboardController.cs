using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
           

            return Json(await _dashboardService.GetTotalBookingRadialChartData());

        }
        [HttpGet]

        public async Task<IActionResult> GetRevenueUserChartData()
        {
            

            return Json(await _dashboardService.GetRevenueUserChartData());
        }
        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {



            return Json(await _dashboardService.GetRegisteredUserChartData());
        }

        public  async Task<IActionResult> GetMemberAndBookingLineChartData()
        {

            return Json(await _dashboardService.GetMemberAndBookingLineChartData());
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingPieChartData()
        {

            return Json(await _dashboardService.GetBookingPieChartData());
        }


    }
}
