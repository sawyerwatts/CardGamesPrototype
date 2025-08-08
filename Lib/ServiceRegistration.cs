using CardGamesPrototype.Lib.Games;
using CardGamesPrototype.Lib.Games.Hearts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardGamesPrototype.Lib;

public static class ServiceRegistration
{
    public static void RegisterCardGameServices(this IHostApplicationBuilder builder)
    {
        builder.RegisterCommonServices();
        builder.RegisterHeartsGame();
    }

    private static void RegisterCommonServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDealer, Dealer>();
        builder.Services.AddSingleton<Dealer.IRng, Dealer.Rng>();
    }

    private static void RegisterHeartsGame(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<HeartsGame.Factory>();
    }
}
