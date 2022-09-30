using System;
using System.Security.Claims;

namespace AutoresApi.DTOs
{
    public class RespuestaAutenticacion
    {
        public string Token { get; set; }
        public System.DateTime Expiracion { get; set; }
        public IList<Claim>? Rol { get; set; }
    }
}
