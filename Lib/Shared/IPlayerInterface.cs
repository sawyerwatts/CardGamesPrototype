namespace CardGamesPrototype.Lib.Shared;

public interface IPlayerInterface<TCard>
    where TCard : Card
{
    Task GiveCards(Cards<TCard> cards, CancellationToken cancellationToken);

    Task ClearHand(CancellationToken cancellationToken);

    Task<int> PromptForIndexOfCardToPlay(Cards<TCard> cards, CancellationToken cancellationToken);

    Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<TCard> cards, CancellationToken cancellationToken);
}
