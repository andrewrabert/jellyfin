using System.Collections.Generic;
using Jellyfin.Api.Attributes;
using Jellyfin.Server.Filters;
using Microsoft.OpenApi;
using Xunit;

namespace Jellyfin.Server.Tests.Filters;

public class FileResponseFilterTests
{
    [Fact]
    public void Apply_ProducesFileAttribute_ReplacesSuccessContentWithBinaryTypes()
    {
        var operation = CreateOperationWithSuccessResponse();
        var context = OperationFilterTestHelpers.CreateContext(new ProducesFileAttribute("video/mp4", "audio/mp3"));

        new FileResponseFilter().Apply(operation, context);

        var content = operation.Responses?["200"].Content;
        Assert.NotNull(content);
        Assert.Equal(2, content.Count);
        foreach (var contentType in new[] { "video/mp4", "audio/mp3" })
        {
            var schema = Assert.IsType<OpenApiSchema>(content[contentType].Schema);
            Assert.Equal(JsonSchemaType.String, schema.Type);
            Assert.Equal("binary", schema.Format);
        }
    }

    [Fact]
    public void Apply_NoProducesFileAttribute_LeavesResponseUntouched()
    {
        var operation = CreateOperationWithSuccessResponse();
        var context = OperationFilterTestHelpers.CreateContext();

        new FileResponseFilter().Apply(operation, context);

        var content = operation.Responses?["200"].Content;
        Assert.NotNull(content);
        var (contentType, mediaType) = Assert.Single(content);
        Assert.Equal("application/json", contentType);
        Assert.NotNull(mediaType);
    }

    private static OpenApiOperation CreateOperationWithSuccessResponse()
        => new OpenApiOperation
        {
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType()
                    }
                }
            }
        };
}
