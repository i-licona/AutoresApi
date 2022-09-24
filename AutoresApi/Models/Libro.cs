using System.ComponentModel.DataAnnotations;

namespace AutoresApi.Models
{
    public class Libro
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo libro es requerido")]
        [StringLength(maximumLength:35, ErrorMessage = "El campo no puede ser mayor a 35 carácteres")]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
