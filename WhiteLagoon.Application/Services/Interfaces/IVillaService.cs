using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IVillaService
    {
        IEnumerable<Villa> GetAllVillas();
        Villa GetVillaById(int id);
        void CreateVilla(Villa villa);  
        void UpdateVilla(Villa villa);  
        bool DeleteVilla(int id);

        IEnumerable<Villa> GetVilasAvailabilityByDate(int nights, DateOnly checkInDate);

        bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate);
    }
}
