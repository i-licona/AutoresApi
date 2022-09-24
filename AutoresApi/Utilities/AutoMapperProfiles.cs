using AutoMapper;
using AutoresApi.DTOs;
using AutoresApi.Models;

namespace AutoresApi.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearAutorDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>().ForMember(x => x.Libros, options => options.MapFrom(MapAutorDTOLibros));
            CreateMap<LibroCrearDTO, Libro>().ForMember(x => x.AutoresLibros, options => options.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>().ForMember(libroDTO => libroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));

            CreateMap<LibroPatchDTO, Libro>().ReverseMap();
            CreateMap<ComentarioCrear, Comentario>(); 
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var result = new List<LibroDTO>();

            if (autor.AutoresLibros == null)
            {
                return result;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                result.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return result;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var result = new List<AutorDTO>();

            if (libro.AutoresLibros == null)
            {
                return result;
            }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                result.Add(new AutorDTO() {
                    Id = autorLibro.AutorId, Nombre = autorLibro.Autor.Nombre
                });
            }

            return result;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCrearDTO libroCrearDTO, Libro libro)
        {
            var result = new List<AutorLibro>();
            if (libroCrearDTO.AutoresIds == null)
            {
                return result;
            }

            foreach (var autorId in libroCrearDTO.AutoresIds)
            {
                result.Add(new AutorLibro() { AutorId = autorId });
            }
            return result;
        }
    }
}
