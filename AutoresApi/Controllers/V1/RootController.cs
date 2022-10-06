using AutoresApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoresApi.Controllers.V1
{

    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        public RootController(IAuthorizationService authorizationService)
        {
            AuthorizationService = authorizationService;
        }

        public IAuthorizationService AuthorizationService { get; }

        [HttpGet(Name = "getRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            var esAdmin = await AuthorizationService.AuthorizeAsync(User, "Es admin");

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link(routeName: "getRoot", new { }), descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link(routeName: "getAuthors", new { }), descripcion: "get-authors", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link(routeName: "createAuthor", new { }), descripcion: "post-authors", metodo: "POST"));
            }


            return datosHateoas;
        }
    }
}
