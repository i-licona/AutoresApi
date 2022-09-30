using AutoresApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AutoresApi.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IActionContextAccessor actionContext;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContext;

        public GeneradorEnlaces(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContext,
            IActionContextAccessor actionContext
        )
        {
            this.authorizationService = authorizationService;
            this.httpContext = httpContext;
            this.actionContext = actionContext;
        }

        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = this.httpContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(this.actionContext.ActionContext);
        }

        private async Task<bool> EsAdmin()
        {
            var httpContex = this.httpContext.HttpContext;
            var result = await this.authorizationService.AuthorizeAsync(httpContex.User, "Es admin");
            return result.Succeeded;
        }

        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();
            //route links
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("getAuthorById",
                new { id = autorDTO.Id }),
                descripcion: "get-author-by-id", metodo: "GET")
            );

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("updateAuthor",
                new { id = autorDTO.Id }),
                descripcion: "update-author", metodo: "PUT")
            );
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("deleteAuthor",
                    new { id = autorDTO.Id }),
                    descripcion: "delete-author", metodo: "DELETE")
                );
            }
        }
    }
}
