namespace CardGamesPrototype.Lib.Shared;

// TODO: the HeartsAi suggests that PromptForIndexesOfCardsToPlay (if not both) methods should

public interface IPlayerInterface<TCard>
    where TCard : Card
{
    Task<int> PromptForIndexOfCardToPlay(Cards<TCard> cards, CancellationToken cancellationToken);

    Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<TCard> cards, CancellationToken cancellationToken);
}
