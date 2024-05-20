using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PomeloCli.Tests;

public class TypeExtensionsTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TypeExtensionsTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test2()
    {
        var c = typeof(Command<>);
        var types = new[] { typeof(A), typeof(B), typeof(B1), typeof(C), typeof(C1) };
        foreach (var t in types)
        {
            _testOutputHelper.WriteLine("t {0}, IsSubclassOfRawGeneric {1}", t.Name, t.IsSubclassOfRawGeneric(c));
        }
    }

    class A : ICommand
    {
        public Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }

    class B : Command
    { }

    class B1 : B
    { }

    class C : Command<B>
    { }

    class C1 : C
    { }
}