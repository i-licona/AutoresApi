using Microsoft.AspNetCore.Identity;

namespace AutoresApi.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int LibroId { get; set; }
        public Libro libro { get; set; }
    }
}
