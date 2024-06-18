using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IAmenityService
    {
        IEnumerable<Amenity> GetAllAmenities();
        Amenity GetAmenityById(int amenityId);
        void Update(Amenity amenity);
        void Create(Amenity amenity);
        void Delete(Amenity amenity);
    }
}
