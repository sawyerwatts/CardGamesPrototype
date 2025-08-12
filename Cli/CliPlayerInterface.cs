using CardGamesPrototype.Lib.Shared;

using Spectre.Console;

namespace CardGamesPrototype.Cli;

public class CliPlayerInterface<TCard>(ILogger<CliPlayerInterface<TCard>> logger) : IPlayerInterface<TCard>
    where TCard : Card
{
    public Task DisplayCards(Cards<TCard> cards, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task ClearHand(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<int> PromptForIndexOfCardToPlay(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        TCard cardToPlay = AnsiConsole.Prompt(
            new SelectionPrompt<TCard>()
                .Title("Which card do you want to play?")
#pragma warning disable CA1861
                .AddChoices(cards.ToArray())
#pragma warning restore CA1861
        );
        int iCardToPlay = cards.FindIndex(card => card.Equals(card));
        logger.LogInformation("Playing card {CardToPlay} at index {IndexCardToPlay}", cardToPlay, iCardToPlay);
        return Task.FromResult(iCardToPlay);
    }

    public Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class CliPlayerInterfaceFactory(IServiceProvider services)
{
    public CliPlayerInterface<TCard> Make<TCard>()
        where TCard : Card
    {
        var logger = services.GetRequiredService<ILogger<CliPlayerInterface<TCard>>>();
        return new CliPlayerInterface<TCard>(logger);
    }
}
