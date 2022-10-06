using AutoMapper.Configuration;
using AutoresApi.Validaciones;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace AutoresApi.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaTests
    {
        [TestMethod]
        public void PrimeraLetraMinusculaDevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayuscula();
            var value = "ignacio";
            var valContext = new ValidationContext(new { Nombre = value });
            //Ejecucion
            var result = primeraLetraMayuscula.GetValidationResult(value, valContext);
            // Varificar
            Assert.AreEqual("La primera letra debe ser mayuscula", result.ErrorMessage);
        }
    }
}