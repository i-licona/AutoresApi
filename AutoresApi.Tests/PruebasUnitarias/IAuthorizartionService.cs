namespace AutoresApi.Tests.PruebasUnitarias
{
    internal interface IAuthorizartionService
    {
        void AuthorizeAsync(System.Security.Claims.ClaimsPrincipal claimsPrincipal, object v, IEnumerable<Microsoft.AspNetCore.Authorization.IAuthorizationRequirement> enumerable);
    }
}