using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {

        private readonly IUnitOfWork _unitOfWork;
        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(Amenity amenity)
        {
            _unitOfWork.Amenity.Add(amenity);
            _unitOfWork.Save();
        }

        public void Delete(Amenity amenity)
        {
            _unitOfWork.Amenity.Remove(amenity);
            _unitOfWork.Save();
        }

        public IEnumerable<Amenity> GetAllAmenities()
        {
            return _unitOfWork.Amenity.GetAll(inculdeProperties: "Villa");
        }

        public Amenity GetAmenityById(int amenityId)
        {
            return _unitOfWork.Amenity.Get(u => u.Id == amenityId);
        }

        public void Update(Amenity amenity)
        {

            _unitOfWork.Amenity.Update(amenity);
            _unitOfWork.Save();
        }
    }
}
