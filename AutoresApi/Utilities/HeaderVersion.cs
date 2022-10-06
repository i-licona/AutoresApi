using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace AutoresApi.Utilities
{
    public class HeaderVersion : Attribute, IActionConstraint
    {
        private readonly string header;
        private readonly string value;

        public int Order => 0;

        public HeaderVersion(string header, string value)
        {
            this.header = header;
            this.value = value;
        }

        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(header))
            {
                return false;
            }

            return string.Equals(headers[header], value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
