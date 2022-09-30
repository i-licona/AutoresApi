using System.Threading.Tasks;
using AutoresApi.DTOs;
using AutoresApi.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoresApi.Utilities
{
    public class HATEOASAuthorFilterAttribute:HATEOASFilterAttribute
    {
        public GeneradorEnlaces generadorEnlaces { get; }

        public HATEOASAuthorFilterAttribute(
            GeneradorEnlaces generadorEnlaces
        )
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var autorDTO = result.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = result.Value as List<AutorDTO> ?? throw new Exception("Se esperaba una instancia de AutorDTO o Listado de AutorDTO");
                
                autoresDTO.ForEach(async item => await generadorEnlaces.GenerarEnlaces(item));  

                result.Value = autoresDTO;
            }
            else
            {
                await generadorEnlaces.GenerarEnlaces(autorDTO);
            }

            //var modelo = result.Value as AutorDTO ?? throw new ArgumentNullException("Se esperaba una instancia de autor DTO");

            //await this.generadorEnlaces.GenerarEnlaces(modelo);
            
            await next();

        }
    }
}
