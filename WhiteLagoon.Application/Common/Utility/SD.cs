﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCanceled = "Canceled";
        public const string StatusRefunded = "Refunded";


        public static int VillaRoomsAvailableCount(int villaId, List<VillaNumber> villaNumberList, DateOnly checkInDate,
            int nights, List<Booking> bookings)
        {
            List<int> bookingInDate = new();
            var rommsInVilla = villaNumberList.Where(x => x.VillaId == villaId).Count();
            int finalAvailableRommForAllNights = int.MaxValue; 
            for(int i=0; i<nights; i++)
            {
                var villabooks = bookings.Where(u => u.CheckInDate <= checkInDate.AddDays(i)
                && u.CheckOutDate > checkInDate.AddDays(i)  && u.VillaId == villaId);

                foreach(var booking in villabooks)
                {
                    if(!bookingInDate.Contains(booking.Id))
                    {
                        bookingInDate.Add(booking.Id);
                    }
                }

                var totalAvailableRooms = rommsInVilla - bookingInDate.Count;

                if(totalAvailableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if(finalAvailableRommForAllNights > totalAvailableRooms )
                    {
                        finalAvailableRommForAllNights = totalAvailableRooms;
                    }
                }
                
            }
            return finalAvailableRommForAllNights;
        }
    }
}
