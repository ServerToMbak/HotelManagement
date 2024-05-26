﻿using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.web.VİewModel;

namespace WhiteLagoon.web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Year == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new DateTime(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
            || u.Status == SD.StatusCanceled);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate
           && u.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate
           && u.BookingDate <= currentMonthStartDate);

            return Json(GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));

        }
        [HttpGet]

        public async Task<IActionResult> GetRevenueUserChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
            || u.Status == SD.StatusCanceled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();


            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate
                      && u.BookingDate <= DateTime.Now).Sum(u=>u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate
           && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);




            return Json(GetRadialChartDataModel(totalRevenue,countByCurrentMonth, countByPreviousMonth));
        }
        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate
           && u.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate
           && u.CreatedAt <= currentMonthStartDate);




            return Json(GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
        }

        public  async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            u.BookingDate <= DateTime.Now)
                .GroupBy(b => b.BookingDate.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewBookingCount = u.Count(),
                });


            var customerData = _unitOfWork.ApplicationUser.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) &&
           u.CreatedAt <= DateTime.Now)
               .GroupBy(b => b.CreatedAt.Date)
               .Select(u => new
               {
                   DateTime = u.Key,
                   NewCustomerCount = u.Count(),
               });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                 (booking, customer) => new
                 {
                     booking.DateTime,
                     booking.NewBookingCount,
                     NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                 });

                 var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                 (customer, booking) => new
                 {
                     customer.DateTime,
                     NewBookingCount = bookingData.Select(x => x.NewBookingCount).FirstOrDefault(),
                     customer.NewCustomerCount,

                 });
            
            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM//dd/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data = newBookingData
                },
                 new ChartData
                {
                    Name = "New Members",
                    Data = newCustomerData
                },
            };



            LineChartVM lineChartVM = new LineChartVM
            {
                Categories = categories,
                Series = chartDataList,
            };

           
            return Json(lineChartVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && 
            (u.Status != SD.StatusPending || u.Status == SD.StatusCanceled));
            var customerWithOneBooking = totalBookings.GroupBy(u => u.UserId).Where(x => x.Count() == 1)
                .Select(x => x.Key).ToList();

            int bookingsByNewCıstomer = customerWithOneBooking.Count();
            int bookignsByReturnCustomer = totalBookings.Count() - bookingsByNewCıstomer;


            PieChart pieChart = new PieChart
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCıstomer, bookignsByReturnCustomer },
            };

            return Json(pieChart);

        }

        private static RadialBarChartVM GetRadialChartDataModel  (int totalCOunt,   double currentMonthCount, double prevMonthCount)
        {

            RadialBarChartVM radialBarChartVM = new RadialBarChartVM();

            int increaseDecreaseRatio = 100;

            if (prevMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32
                    ((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
            }
            radialBarChartVM.TotalCount = totalCOunt;
            radialBarChartVM.CountCurrentMounth = Convert.ToInt32(currentMonthCount);
            radialBarChartVM.HasRatioIncreased = currentMonthCount > prevMonthCount;
            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

            return radialBarChartVM;
        }

    }
}