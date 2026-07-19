using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jellyfin.Server.Tests.Filters;

internal static class OperationFilterTestHelpers
{
    internal static OperationFilterContext CreateContext(params object[] endpointMetadata)
    {
        var apiDescription = new ApiDescription
        {
            ActionDescriptor = new ActionDescriptor
            {
                EndpointMetadata = new List<object>(endpointMetadata)
            }
        };

        return new OperationFilterContext(
            apiDescription,
            Mock.Of<ISchemaGenerator>(),
            new SchemaRepository(),
            new OpenApiDocument(),
            typeof(OperationFilterTestHelpers).GetMethod(nameof(CreateContext), BindingFlags.NonPublic | BindingFlags.Static)!);
    }
}
