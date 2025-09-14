using CardGamesPrototype.Lib.Games.Hearts;

using Spectre.Console;

namespace CardGamesPrototype.Cli;

// TODO: have an IGameFactory that has a key prop, then can inject all and run dynamically

public class Driver(
    CliPlayerInterfaceFactory cliPlayerInterfaceFactory,
    ILogger<Driver> logger,
    IServiceProvider services)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string playerName = AnsiConsole.Prompt(new TextPrompt<string>("What is your name?"));

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

            CliPlayerPortal<HeartsCard> playerPortal = cliPlayerInterfaceFactory.Make<HeartsCard>();
            HeartsPlayer.Factory playerFactory = services.GetRequiredService<HeartsPlayer.Factory>();

            HeartsGame game = services.GetRequiredService<HeartsGame.Factory>().Make(
            [
                playerFactory.Make(playerName, playerPortal),
                playerFactory.Make("AI 0", services.GetRequiredService<HeartsAi>()),
                playerFactory.Make("AI 1", services.GetRequiredService<HeartsAi>()),
                playerFactory.Make("AI 2", services.GetRequiredService<HeartsAi>()),
            ]);

            await game.Play(cancellationToken);
        }
    }
}
