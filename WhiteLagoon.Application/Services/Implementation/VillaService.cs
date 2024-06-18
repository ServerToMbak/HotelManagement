using Microsoft.AspNetCore.Hosting;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public void CreateVilla(Villa obj)
        {
            if (obj.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");
                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    obj.Image.CopyTo(fileStream);

                obj.ImageUrl = @"\images\VillaImage\" + fileName;


            }
            else
            {
                obj.ImageUrl = "https://placehold.co/600x400";
            }
            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Save();
        }

        public bool DeleteVilla(int id)
        {
            try
            {
                Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == id);
                if (objFromDb is not null)
                {


                    var oldImagePth = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePth))
                    {
                        System.IO.File.Delete(oldImagePth);
                    }

                    _unitOfWork.Villa.Remove(objFromDb);
                    _unitOfWork.Save();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public IEnumerable<Villa> GetAllVillas()
        {
            return _unitOfWork.Villa.GetAll(inculdeProperties: "VillaAmenity");
        }

        public IEnumerable<Villa> GetVilasAvailabilityByDate(int nights, DateOnly checkInDate)
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

            return VillaList;
        }

        public Villa GetVillaById(int id)
        {
           return _unitOfWork.Villa.Get(u => u.Id == id, inculdeProperties: "VillaAmenity");
        }

        public bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate)
        {
            var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
            u.Status == SD.StatusCheckedIn).ToList();



            int roomAvailable = SD.VillaRoomsAvailableCount(villaId, villaNumberList, checkInDate, nights, bookedVillas);

            return  roomAvailable > 0 ;   
        }

        public void UpdateVilla(Villa villa)
        {

            if (villa.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                if (!string.IsNullOrEmpty(villa.ImageUrl))
                {
                    var oldImagePth = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePth))
                    {
                        System.IO.File.Delete(oldImagePth);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    villa.Image.CopyTo(fileStream);

                villa.ImageUrl = @"\images\VillaImage\" + fileName;
            }

            _unitOfWork.Villa.Update(villa);
            _unitOfWork.Save();
        }
    }
}
