using System;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Locking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Locking;

public class OptimisticLockBehaviorTests
{
    private readonly OptimisticLockBehavior _behavior = new(NullLogger<OptimisticLockBehavior>.Instance);

    private static InvalidOperationException CreateLockedException()
        => new("Write failed.", new InvalidOperationException("SQLite Error 5: 'database is locked'."));

    [Fact]
    public void OnSaveChanges_InvokesCallbackOnce()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () => invocations++);

        Assert.Equal(1, invocations);
    }

    [Fact]
    public void OnSaveChanges_DatabaseLocked_Retries()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () =>
        {
            invocations++;
            if (invocations < 3)
            {
                throw CreateLockedException();
            }
        });

        Assert.Equal(3, invocations);
    }

    [Fact]
    public void OnSaveChanges_UnrelatedException_DoesNotRetry()
    {
        var invocations = 0;

        _behavior.OnSaveChanges(null!, () =>
        {
            invocations++;
            throw new InvalidOperationException("Unrelated failure.");
        });

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
    public async Task OnSaveChangesAsync_DatabaseLocked_Retries()
    {
        var invocations = 0;

        await _behavior.OnSaveChangesAsync(null!, () =>
        {
            invocations++;
            if (invocations < 3)
            {
                throw CreateLockedException();
            }

            return Task.CompletedTask;
        });

        Assert.Equal(3, invocations);
    }

    [Fact]
    public async Task OnSaveChangesAsync_UnrelatedException_DoesNotRetry()
    {
        var invocations = 0;

        await _behavior.OnSaveChangesAsync(null!, () =>
        {
            invocations++;
            throw new InvalidOperationException("Unrelated failure.");
        });

        Assert.Equal(1, invocations);
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
