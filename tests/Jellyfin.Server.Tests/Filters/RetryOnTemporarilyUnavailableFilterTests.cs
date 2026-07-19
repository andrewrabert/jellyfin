using Jellyfin.Server.Filters;
using Microsoft.OpenApi;
using Xunit;

namespace Jellyfin.Server.Tests.Filters;

public class RetryOnTemporarilyUnavailableFilterTests
{
    [Fact]
    public void Apply_AddsServiceUnavailableResponse()
    {
        var operation = new OpenApiOperation { Responses = new OpenApiResponses() };
        var context = OperationFilterTestHelpers.CreateContext();

        new RetryOnTemporarilyUnavailableFilter().Apply(operation, context);

        var response = operation.Responses?["503"];
        Assert.NotNull(response);
        Assert.NotNull(response.Headers);
        Assert.True(response.Headers.ContainsKey("Retry-After"));
        Assert.True(response.Headers.ContainsKey("Message"));
        Assert.NotNull(response.Content);
        Assert.True(response.Content.ContainsKey("text/html"));
    }

    [Fact]
    public void Apply_ExistingServiceUnavailableResponse_IsNotOverwritten()
    {
        var existingResponse = new OpenApiResponse { Description = "Custom 503" };
        var operation = new OpenApiOperation
        {
            Responses = new OpenApiResponses { ["503"] = existingResponse }
        };
        var context = OperationFilterTestHelpers.CreateContext();

        new RetryOnTemporarilyUnavailableFilter().Apply(operation, context);

        Assert.Same(existingResponse, operation.Responses?["503"]);
    }

    [Fact]
    public void Apply_NullResponses_DoesNotThrow()
    {
        var operation = new OpenApiOperation { Responses = null };
        var context = OperationFilterTestHelpers.CreateContext();

        new RetryOnTemporarilyUnavailableFilter().Apply(operation, context);

        Assert.Null(operation.Responses);
    }
}
