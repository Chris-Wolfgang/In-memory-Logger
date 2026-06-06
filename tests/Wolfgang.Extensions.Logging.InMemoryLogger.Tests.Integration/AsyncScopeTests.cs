using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Integration;

/// <summary>
/// End-to-end exercises of the AsyncLocal-backed scope stack across real
/// async/await boundaries. The unit tests cover the synchronous BeginScope +
/// Dispose contract; these tests pin that scopes flow correctly through
/// awaited continuations on real captured execution contexts.
/// </summary>
public class AsyncScopeTests
{
    [Fact]
    public async Task BeginScope_when_used_across_await_keeps_the_scope_visible()
    {
        var sut = new InMemoryLogger("async.scope.basic", LogLevel.Trace);

        using (sut.BeginScope("outer-scope"))
        {
            await Task.Delay(1);

            // After awaiting, the scope must still be observable to the
            // logger. The unit-test surface asserts the in-call state; this
            // asserts the AsyncLocal continuation flow.
            Assert.Contains("outer-scope", sut.Scopes);
        }

        // Scope removed after Dispose.
        Assert.Empty(sut.Scopes);
    }


    [Fact]
    public async Task BeginScope_when_used_across_concurrent_awaits_keeps_each_branch_isolated()
    {
        var sut = new InMemoryLogger("async.scope.isolated", LogLevel.Trace);

        // Two parallel async branches each open their own scope. They must
        // not see each other's scope state.
        var taskA = Task.Run(async () =>
        {
            using (sut.BeginScope("branch-A"))
            {
                await Task.Delay(5);
                return sut.Scopes.ToArray();
            }
        });

        var taskB = Task.Run(async () =>
        {
            using (sut.BeginScope("branch-B"))
            {
                await Task.Delay(5);
                return sut.Scopes.ToArray();
            }
        });

        var resultA = await taskA;
        var resultB = await taskB;

        Assert.Contains("branch-A", resultA);
        Assert.DoesNotContain("branch-B", resultA);

        Assert.Contains("branch-B", resultB);
        Assert.DoesNotContain("branch-A", resultB);
    }


    [Fact]
    public async Task BeginScope_when_nested_visible_in_inner_scope_only_outer_after_inner_disposes()
    {
        var sut = new InMemoryLogger("async.scope.nested", LogLevel.Trace);

        using (sut.BeginScope("outer"))
        {
            using (sut.BeginScope("inner"))
            {
                await Task.Yield();

                Assert.Contains("outer", sut.Scopes);
                Assert.Contains("inner", sut.Scopes);
            }

            await Task.Yield();

            // After the inner scope disposes, only the outer scope remains.
            Assert.Contains("outer", sut.Scopes);
            Assert.DoesNotContain("inner", sut.Scopes);
        }

        Assert.Empty(sut.Scopes);
    }


    [Fact]
    public async Task Log_when_called_inside_a_scope_after_await_records_the_entry()
    {
        var sut = new InMemoryLogger("async.scope.logged", LogLevel.Trace);

        using (sut.BeginScope("op-42"))
        {
            await Task.Yield();
            sut.Log
            (
                LogLevel.Information,
                new EventId(0),
                "after-await",
                exception: null,
                formatter: static (s, _) => s
            );
        }

        Assert.Single(sut.LogEntries);
        Assert.Equal("after-await", sut.LogEntries[0].Formatter(sut.LogEntries[0].State, sut.LogEntries[0].Exception));
    }
}
