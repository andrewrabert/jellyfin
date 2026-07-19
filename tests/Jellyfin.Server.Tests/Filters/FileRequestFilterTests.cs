using Jellyfin.Api.Attributes;
using Jellyfin.Server.Filters;
using Microsoft.OpenApi;
using Xunit;

namespace Jellyfin.Server.Tests.Filters;

public class FileRequestFilterTests
{
    [Fact]
    public void Apply_AcceptsFileAttribute_SetsBinaryRequestBody()
    {
        var operation = new OpenApiOperation();
        var context = OperationFilterTestHelpers.CreateContext(new AcceptsFileAttribute("image/png", "image/jpeg"));

        new FileRequestFilter().Apply(operation, context);

        Assert.NotNull(operation.RequestBody);
        var content = operation.RequestBody.Content;
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        foreach (var contentType in new[] { "image/png", "image/jpeg" })
        {
            var schema = Assert.IsType<OpenApiSchema>(content[contentType].Schema);
            Assert.Equal(JsonSchemaType.String, schema.Type);
            Assert.Equal("binary", schema.Format);
        }
    }

    [Fact]
    public void Apply_NoAcceptsFileAttribute_LeavesRequestBodyUnset()
    {
        var operation = new OpenApiOperation();
        var context = OperationFilterTestHelpers.CreateContext();

        new FileRequestFilter().Apply(operation, context);

        Assert.Null(operation.RequestBody);
    }
}
