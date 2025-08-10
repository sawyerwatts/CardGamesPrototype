using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CardGamesPrototype.Lib.Games.Hearts;

public static class HeartsServiceRegistration
{
    public static void RegisterHeartsGame(this IHostApplicationBuilder builder, string gamesSectionName)
    {
        builder.Services.AddSingleton<HeartsGame.Factory>();
        builder.Services.AddOptions<HeartsGame.Options>()
            .BindConfiguration($"{gamesSectionName}:Hearts")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddTransient<HeartsAi>();
    }
}
