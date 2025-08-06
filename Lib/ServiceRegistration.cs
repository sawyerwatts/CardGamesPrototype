using CardGamesPrototype.Lib.Common;
using CardGamesPrototype.Lib.Games;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardGamesPrototype.Lib;

public static class ServiceRegistration
{
    public static void RegisterCardGameServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDealer, Dealer>();
        builder.Services.AddSingleton<HeartsGame.Factory>();
    }
}
