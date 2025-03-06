using Microsoft.EntityFrameworkCore;
using Server.Persistence;

namespace Server.Tests;

public static class Helper
{
    public static ServerContext GetContext<T>(string dataBaseName)
    {
        var options = new DbContextOptionsBuilder<ServerContext>()
            .UseInMemoryDatabase(dataBaseName)
            .Options;
        return new ServerContext(options);
    }
}