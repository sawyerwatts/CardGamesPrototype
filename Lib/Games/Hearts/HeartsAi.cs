using System.Security.Cryptography;

namespace CardGamesPrototype.Lib.Games.Hearts;

public class HeartsAi : IPlayerInterface<HeartsCard>
{
    public Task GiveCards(Cards<HeartsCard> cards, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ClearHand(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<int> PromptForIndexOfCardToPlay(Cards<HeartsCard> cards, CancellationToken cancellationToken)
    {
        return Task.FromResult(RandomNumberGenerator.GetInt32(
            fromInclusive: 0,
            toExclusive: cards.Count));
    }

    public Task<List<int>> PromptForIndexesOfCardsToPlay(Cards<HeartsCard> cards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
