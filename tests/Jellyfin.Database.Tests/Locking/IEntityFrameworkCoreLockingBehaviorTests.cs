using System;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Locking;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.Locking;

public class IEntityFrameworkCoreLockingBehaviorTests
{
    [Theory]
    [InlineData(typeof(NoLockBehavior))]
    [InlineData(typeof(OptimisticLockBehavior))]
    [InlineData(typeof(PessimisticLockBehavior))]
    public void KnownLockingBehaviors_ImplementInterface(Type behaviorType)
    {
        Assert.True(typeof(IEntityFrameworkCoreLockingBehavior).IsAssignableFrom(behaviorType));
    }

    [Fact]
    public void Interface_DeclaresExpectedContract()
    {
        var type = typeof(IEntityFrameworkCoreLockingBehavior);

        var initialise = type.GetMethod(nameof(IEntityFrameworkCoreLockingBehavior.Initialise));
        Assert.NotNull(initialise);
        Assert.Equal([typeof(DbContextOptionsBuilder)], Array.ConvertAll(initialise.GetParameters(), p => p.ParameterType));

        var onSaveChanges = type.GetMethod(nameof(IEntityFrameworkCoreLockingBehavior.OnSaveChanges));
        Assert.NotNull(onSaveChanges);
        Assert.Equal([typeof(JellyfinDbContext), typeof(Action)], Array.ConvertAll(onSaveChanges.GetParameters(), p => p.ParameterType));

        var onSaveChangesAsync = type.GetMethod(nameof(IEntityFrameworkCoreLockingBehavior.OnSaveChangesAsync));
        Assert.NotNull(onSaveChangesAsync);
        Assert.Equal(typeof(Task), onSaveChangesAsync.ReturnType);
        Assert.Equal([typeof(JellyfinDbContext), typeof(Func<Task>)], Array.ConvertAll(onSaveChangesAsync.GetParameters(), p => p.ParameterType));
    }
}
