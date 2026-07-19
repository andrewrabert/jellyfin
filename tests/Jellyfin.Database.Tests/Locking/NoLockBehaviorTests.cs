using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Locking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Locking;

public class NoLockBehaviorTests
{
    private readonly NoLockBehavior _behavior = new(NullLogger<NoLockBehavior>.Instance);

    [Fact]
    public void OnSaveChanges_InvokesCallbackOnce()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () => invocations++);

        Assert.Equal(1, invocations);
    }

    [Fact]
    public async Task OnSaveChangesAsync_InvokesCallbackOnce()
    {
        var invocations = 0;

        await _behavior.OnSaveChangesAsync(null!, () =>
        {
            invocations++;
            return Task.CompletedTask;
        });

        Assert.Equal(1, invocations);
    }

    [Fact]
    public void Initialise_AddsNoInterceptors()
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        _behavior.Initialise(optionsBuilder);

        var coreExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>();
        Assert.True(coreExtension?.Interceptors is null || !coreExtension.Interceptors.Any());
    }
}
