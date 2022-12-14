using System.ComponentModel.DataAnnotations;

namespace AutoresApi.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo libro es requerido")]
        [StringLength(maximumLength: 35, ErrorMessage = "El campo no puede ser mayor a 35 carácteres")]
        public string Titulo { get; set; }
        public System.DateTime FechaPublicacion { get; set; }
        //public List<ComentarioDTO> Comentarios { get; set; }

    }
}
