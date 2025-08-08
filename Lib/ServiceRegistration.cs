using CardGamesPrototype.Lib.Games.Hearts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardGamesPrototype.Lib;

public static class ServiceRegistration
{
    private const string GamesConfigPrefix = "Games";

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
        builder.Services.AddOptions<HeartsGame.Options>()
            .BindConfiguration($"{GamesConfigPrefix}:Hearts")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
