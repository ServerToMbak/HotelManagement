using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }   

        [Authorize]
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            Booking booking = new()
            {
                VillaId = villaId,
                Villa = _unitOfWork.Villa.Get(u => u.Id == villaId, inculdeProperties: "VillaAmenity"),
                CheckInDate = checkInDate,
                CheckOutDate = checkInDate.AddDays(nights),
                Nights = nights,
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber,
                UserId = userId,
                
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;

            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
            u.Status == SD.StatusCheckedIn).ToList();


            
                int roomAvailable = SD.VillaRoomsAvailableCount(villa.Id, villaNumberList, booking.CheckInDate, booking.Nights, bookedVillas);
                
            if(roomAvailable == 0)
            {
                TempData["error"] = "The room has been sold out";

                return RedirectToAction(nameof(FinalizeBooking),
                    new
                    {
                        villaId = booking.VillaId,
                        checkInDatet = booking.CheckInDate,
                        nights = booking.Nights,
                    });
            }

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

            var domain =  Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate.ToString("yyyy-MM-dd")}&nights={booking.Nights}"

            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                         
                    }
                },
                Quantity = 1,
            });
     
            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Booking.UpdateStritePaymentId(booking.Id, session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            
            return new StatusCodeResult(303);

        }

        [Authorize]
        public  IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId,inculdeProperties: "User,Villa");
            
            if(bookingFromDb.Status  == SD.StatusPending)
            {
                //this is a pending order, we need to confirm that if payment was succesfull
                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripeSessionId);

                if(session.PaymentStatus =="paid")
                {
                    _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved,0);
                    _unitOfWork.Booking.UpdateStritePaymentId(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save(); 
                }
            }
            
            return View(bookingId);
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;

            if(User.IsInRole(SD.Role_Admin))
            {
                objBookings = _unitOfWork.Booking.GetAll(inculdeProperties:"User,Villa");


            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _unitOfWork
                    .Booking.GetAll(u => u.UserId == userId,inculdeProperties:"User,Villa");
            }
            if(!string.IsNullOrEmpty(status)) 
            {
                objBookings = objBookings.Where(u => u.Status.ToLower().Equals(status.ToLower())); 
            }
            return  Json (new { data = objBookings });
        }


        [Authorize]
        public IActionResult BookingDetails(int bookingId) 
        {
            Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, inculdeProperties: "User,Villa");

            if(bookingFromDb.VillaNumber ==0 && bookingFromDb.Status == SD.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaNumbersByVilla(bookingFromDb.VillaId);

                bookingFromDb.VillaNumbers = _unitOfWork.VillaNumber.GetAll(u => u.VillaId == bookingFromDb.VillaId
                && availableVillaNumber.Any(x=> x==u.Villa_Number)).ToList();

            }

            return View(bookingFromDb);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new {bookingId = booking.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking Completed Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCanceled, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking Cancelled Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        private List<int> AssignAvailableVillaNumbersByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll(U => U.VillaId == villaId);

            var checkedInVilla = _unitOfWork.Booking.GetAll(u=>u.VillaId == villaId &&  u.Status == SD.StatusCheckedIn)
                .Select(u=> u.VillaNumber);

            foreach (var villaNumber in villaNumbers)
            {
                if(!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }

            return availableVillaNumbers;

        }
        #endregion
    }
}
