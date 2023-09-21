using Xunit;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;
using DocoptNet;
using NSubstitute;

namespace Chirp.CLI.UnitTest;

public class ChirpHandlerTest
{
    private readonly IStorageProvider<ChirpRecord> _storage = Substitute.For<IStorageProvider<ChirpRecord>>();
    private readonly IStorage<ChirpRecord> _providedStorage = Substitute.For<IStorage<ChirpRecord>>();
    private readonly IArgumentsProvider _argsProvider = Substitute.For<IArgumentsProvider>();
    private readonly IUserInterface _ui = Substitute.For<IUserInterface>();
    private readonly ChirpHandler _sut;

    public ChirpHandlerTest()
    {
        _sut = new ChirpHandler(_storage, _argsProvider,_ui);
    }

    [Fact]
    public void ChirpHandler_ProvidesArgument_ArgumentsReadAndExecuted()
    {
        _storage.Storage.Returns(_providedStorage);
        var argDict = new Dictionary<string, ArgValue>
        {
            { "read", ArgValue.True },
            { "cheep", ArgValue.False },
            { "--help", ArgValue.False }
        };
        _sut.HandleCustomArgs(argDict);
    }
}
