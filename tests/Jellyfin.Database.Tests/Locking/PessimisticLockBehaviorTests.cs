using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Locking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Locking;

public class PessimisticLockBehaviorTests
{
    private readonly PessimisticLockBehavior _behavior = new(
        NullLogger<PessimisticLockBehavior>.Instance,
        NullLoggerFactory.Instance);

    [Fact]
    public void OnSaveChanges_InvokesCallbackOnce()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () => invocations++);

        Assert.Equal(1, invocations);
    }

    [Fact]
    public void OnSaveChanges_ReleasesLock_AllowingSubsequentSaves()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () => invocations++);
        _behavior.OnSaveChanges(null!, () => invocations++);

        Assert.Equal(2, invocations);
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
    public async Task OnSaveChangesAsync_ReleasesLock_AllowingSubsequentSaves()
    {
        var invocations = 0;

        Task SaveChanges()
        {
            invocations++;
            return Task.CompletedTask;
        }

        await _behavior.OnSaveChangesAsync(null!, SaveChanges);
        await _behavior.OnSaveChangesAsync(null!, SaveChanges);

        Assert.Equal(2, invocations);
    }

    [Fact]
    public void Initialise_AddsCommandAndTransactionInterceptors()
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        _behavior.Initialise(optionsBuilder);

        var interceptors = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()?.Interceptors?.ToList();
        Assert.NotNull(interceptors);
        Assert.Equal(2, interceptors.Count);
        Assert.Single(interceptors.OfType<IDbCommandInterceptor>());
        Assert.Single(interceptors.OfType<IDbTransactionInterceptor>());
    }
}
