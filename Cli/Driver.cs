using CardGamesPrototype.Lib.Games.Hearts;

using Spectre.Console;

namespace CardGamesPrototype.Cli;

public class Driver(
    CliPlayerInterfaceFactory cliPlayerInterfaceFactory,
    ILogger<Driver> logger,
    IServiceProvider services)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string gameName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What game do you want to play?")
#pragma warning disable CA1861
                    .AddChoices(new []
                    {
                        "Hearts",
                    })
#pragma warning restore CA1861
            );
            logger.LogInformation("Initializing {Game}", gameName);

            CliPlayerInterface<HeartsCard> playerInterface = cliPlayerInterfaceFactory.Make<HeartsCard>();
            HeartsPlayer.Factory playerFactory = services.GetRequiredService<HeartsPlayer.Factory>();

            HeartsGame game = services.GetRequiredService<HeartsGame.Factory>().Make(
            [
                playerFactory.Make(playerInterface),
                playerFactory.Make(services.GetRequiredService<HeartsAi>()),
                playerFactory.Make(services.GetRequiredService<HeartsAi>()),
                playerFactory.Make(services.GetRequiredService<HeartsAi>()),
            ]);

            await game.Play(cancellationToken);
        }
    }
}
