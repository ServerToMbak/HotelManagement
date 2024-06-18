using Microsoft.AspNetCore.Hosting;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VillaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckVillaNumberExists(int villa_Number)
        {
            return _unitOfWork.VillaNumber.Any(u => u.Villa_Number == villa_Number);  
        }

        public void CreateVillaNumber(VillaNumber obj)
        {
            
            _unitOfWork.VillaNumber.Add(obj);
            _unitOfWork.Save();
        }

        public bool DeleteVillaNumber(int id)
        {
            try
            {
                VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == id);
                if (objFromDb is not null)
                {
                    _unitOfWork.VillaNumber.Remove(objFromDb);
                    _unitOfWork.Save();
                    return true;
                }   
                return false;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public IEnumerable<VillaNumber> GetAllVillaNumbers()
        {
            return _unitOfWork.VillaNumber.GetAll(inculdeProperties: "Villa");
        }

        public VillaNumber GetVillaNumberById(int id)
        {
           return _unitOfWork.VillaNumber.Get(u => u.Villa_Number == id, inculdeProperties: "Villa");
        }

        public void UpdateVillaNumber(VillaNumber villaNumber)
        {
            _unitOfWork.VillaNumber.Update(villaNumber);
            _unitOfWork.Save();
        }
    }
}
