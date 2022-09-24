using System.ComponentModel.DataAnnotations;

namespace AutoresApi.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
