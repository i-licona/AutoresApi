using System;

namespace AutoresApi.DTOs
{
    public class RespuestaAutenticacion
    {
        public string Token { get; set; }
        public System.DateTime Expiracion { get; set; }
    }
}
