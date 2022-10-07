using AutoresApi.Controllers.V1;
using AutoresApi.Tests.PruebasUnitarias.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AutoresApi.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin()
        {
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();
            //ejecucion
            var result = await rootController.Get();
            //verificacion
            Assert.AreEqual(3, result.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin()
        {
            //preparacio
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();
            //ejecucion
            var result = await rootController.Get();
            //verificacion
            Assert.AreEqual(2, result.Value.Count());
        }

        public async Task SiUsuarioNoEsAdminUsuandoMoq()
        {
            //preparacio\
            //Configure Mock IAutherizationService
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
             It.IsAny<ClaimsPrincipal>(),
             It.IsAny<object>(),
             It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));
            // Configure Mock IUrlHelper
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(String.Empty);
            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockUrlHelper.Object;
            //ejecucion
            var result = await rootController.Get();
            //verificacion
            Assert.AreEqual(2, result.Value.Count());
        }


    }
}
