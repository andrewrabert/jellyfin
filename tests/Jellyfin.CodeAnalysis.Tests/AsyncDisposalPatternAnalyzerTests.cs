using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Jellyfin.CodeAnalysis.Tests;

public class AsyncDisposalPatternAnalyzerTests
{
    private const string AsyncResourceSource = """

        public class AsyncResource : System.IDisposable, System.IAsyncDisposable
        {
            public void Dispose()
            {
            }

            public System.Threading.Tasks.ValueTask DisposeAsync() => default;
        }

        public class DerivedAsyncResource : AsyncResource
        {
        }

        public class SyncResource : System.IDisposable
        {
            public void Dispose()
            {
            }
        }

        """;

    [Fact]
    public async Task SyncUsing_AwaitCreatedAsyncDisposable_ReportsDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    {|#0:using (var resource = await CreateAsync())
                    {
                    }|}
                }

                private static System.Threading.Tasks.Task<AsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new AsyncResource());
            }
            """;

        var expected = new DiagnosticResult(AsyncDisposalPatternAnalyzer.AsyncDisposableSyncDisposal)
            .WithLocation(0)
            .WithArguments("AsyncResource");

        await VerifyAsync(Source, expected);
    }

    [Fact]
    public async Task SyncUsing_AwaitCreatedDerivedAsyncDisposable_ReportsDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    {|#0:using (var resource = await CreateAsync())
                    {
                    }|}
                }

                private static System.Threading.Tasks.Task<DerivedAsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new DerivedAsyncResource());
            }
            """;

        var expected = new DiagnosticResult(AsyncDisposalPatternAnalyzer.AsyncDisposableSyncDisposal)
            .WithLocation(0)
            .WithArguments("DerivedAsyncResource");

        await VerifyAsync(Source, expected);
    }

    [Fact]
    public async Task SyncUsing_MultipleVariablesOneAwaitCreated_ReportsDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    {|#0:using (AsyncResource first = new AsyncResource(), second = await CreateAsync())
                    {
                    }|}
                }

                private static System.Threading.Tasks.Task<AsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new AsyncResource());
            }
            """;

        var expected = new DiagnosticResult(AsyncDisposalPatternAnalyzer.AsyncDisposableSyncDisposal)
            .WithLocation(0)
            .WithArguments("AsyncResource");

        await VerifyAsync(Source, expected);
    }

    [Fact]
    public async Task AwaitUsing_AwaitCreatedAsyncDisposable_NoDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    await using (var resource = await CreateAsync())
                    {
                    }
                }

                private static System.Threading.Tasks.Task<AsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new AsyncResource());
            }
            """;

        await VerifyAsync(Source);
    }

    [Fact]
    public async Task SyncUsing_ExpressionWithoutDeclaration_NoDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    using (await CreateAsync())
                    {
                    }
                }

                private static System.Threading.Tasks.Task<AsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new AsyncResource());
            }
            """;

        await VerifyAsync(Source);
    }

    [Fact]
    public async Task SyncUsing_SynchronouslyCreatedAsyncDisposable_NoDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public void Run()
                {
                    using (var resource = new AsyncResource())
                    {
                    }
                }
            }
            """;

        await VerifyAsync(Source);
    }

    [Fact]
    public async Task SyncUsing_AwaitCreatedSyncDisposable_NoDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    using (var resource = await CreateAsync())
                    {
                    }
                }

                private static System.Threading.Tasks.Task<SyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new SyncResource());
            }
            """;

        await VerifyAsync(Source);
    }

    [Fact]
    public async Task SyncUsing_AwaitCreatedFakeAsyncDisposable_NoDiagnostic()
    {
        const string Source = """
            namespace Fake
            {
                public interface IAsyncDisposable
                {
                }
            }

            public class FakeAsyncResource : Fake.IAsyncDisposable, System.IDisposable
            {
                public void Dispose()
                {
                }
            }

            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    using (var resource = await CreateAsync())
                    {
                    }
                }

                private static System.Threading.Tasks.Task<FakeAsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new FakeAsyncResource());
            }
            """;

        await VerifyAsync(Source);
    }

    [Fact]
    public async Task UsingDeclaration_AwaitCreatedAsyncDisposable_NoDiagnostic()
    {
        const string Source = AsyncResourceSource + """
            public class Test
            {
                public async System.Threading.Tasks.Task RunAsync()
                {
                    using var resource = await CreateAsync();
                }

                private static System.Threading.Tasks.Task<AsyncResource> CreateAsync()
                    => System.Threading.Tasks.Task.FromResult(new AsyncResource());
            }
            """;

        await VerifyAsync(Source);
    }

    private static Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<AsyncDisposalPatternAnalyzer, DefaultVerifier>
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync();
    }
}
