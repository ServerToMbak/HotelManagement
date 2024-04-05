using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.web.VİewModel
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RemmberMe { get; set; }

        public string? RedirectUrl { get; set; }
    }
}
