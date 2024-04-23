using System.ComponentModel.DataAnnotations;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.web.VİewModel
{
    public class HomeVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly? CheckOutDae { get; set; }
        public int Nights { get; set; }
    }
}
