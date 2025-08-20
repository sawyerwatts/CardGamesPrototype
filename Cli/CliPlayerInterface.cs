using CardGamesPrototype.Lib.Shared;

using Spectre.Console;

namespace CardGamesPrototype.Cli;

public class CliPlayerInterface<TCard>(ILogger<CliPlayerInterface<TCard>> logger) : IPlayerInterface<TCard>
    where TCard : Card
{
    public async Task<int> PromptForIndexOfCardToPlay(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        TCard cardToPlay = await AnsiConsole.PromptAsync(
            new SelectionPrompt<TCard>()
                .Title("Which card do you want to play?")
                .PageSize(1024)
#pragma warning disable CA1861
                .AddChoices(cards.ToArray()),
#pragma warning restore CA1861
            cancellationToken);
        // BUG: this always returns 0??
        int iCardToPlay = cards.FindIndex(card => card.Equals(card));
        logger.LogInformation("Playing card {CardToPlay} at index {IndexCardToPlay}", cardToPlay, iCardToPlay);
        return iCardToPlay;
    }

    public async Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        List<TCard> cardsToPlay = await AnsiConsole.PromptAsync(
            new MultiSelectionPrompt<TCard>()
                .Title("Which card do you want to play?")
                .PageSize(1024)
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                    "[green]<enter>[/] to accept)[/]")
#pragma warning disable CA1861
                .AddChoices(cards.ToArray()),
#pragma warning restore CA1861
            cancellationToken);

        List<int> iCardsToPlay = cards
            .Select((card, iCard) => (Card: card, Index: iCard))
            .Where(x => cardsToPlay.Contains(x.Card))
            .Select(x => x.Index)
            .ToList();

        foreach (int iCardToPlay in iCardsToPlay)
            logger.LogInformation("Playing card {CardToPlay} at index {IndexCardToPlay}", cards[iCardToPlay], iCardToPlay);

        return iCardsToPlay;
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
