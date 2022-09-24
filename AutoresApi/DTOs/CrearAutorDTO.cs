using System.ComponentModel.DataAnnotations;

namespace AutoresApi.DTOs
{
    public class CrearAutorDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 20, ErrorMessage = "El campo {0} no debe ser mayor a {1} carácteres")]
        public string Nombre { get; set; }
    }
}
