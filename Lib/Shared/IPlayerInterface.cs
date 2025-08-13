namespace CardGamesPrototype.Lib.Shared;

public interface IPlayerInterface<TCard>
    where TCard : Card
{
    Task<int> PromptForIndexOfCardToPlay(Cards<TCard> cards, CancellationToken cancellationToken);

    Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<TCard> cards, CancellationToken cancellationToken);
}
