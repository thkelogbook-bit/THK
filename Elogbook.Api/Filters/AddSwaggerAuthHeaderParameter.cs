using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace Elogbook.Api
{
    /// <summary>
    /// Filter to add the authorization to the header
    /// </summary>
    public class AddSwaggerAuthHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// Implement the Apply method
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                type = "string",
                required = false,
                description = "You must call the login api to get the JWT and put it here."
            });
        }
    }
}