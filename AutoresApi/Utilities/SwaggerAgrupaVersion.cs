using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AutoresApi.Utilities
{
    public class SwaggerAgrupaVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace;
            var versionApi = nameSpaceController.Split(".").Last().ToLower();
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
