using CardGamesPrototype.Lib.Games.Hearts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardGamesPrototype.Lib;

public static class ServiceRegistration
{
    private const string GamesConfigPrefix = "Games";

    public static void RegisterCardGameServices(this IHostApplicationBuilder builder)
    {
        builder.RegisterSharedServices();
        builder.RegisterHeartsGame(GamesConfigPrefix);
    }

    private static void RegisterSharedServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDealer, Dealer>();
        builder.Services.AddSingleton<Dealer.IRng, Dealer.Rng>();
    }
}
