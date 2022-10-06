using Microsoft.EntityFrameworkCore;

namespace AutoresApi.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametroPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();

            httpContext.Response.Headers.Add("CantidadTotalRegistros", cantidad.ToString());

        }
    }
}
