namespace CardGamesPrototype.Lib.Common;

// TODO: mv to own file
public interface IPlayerInterface
{
}

/// <summary>
/// This class exists to contain the state for an individual player within a game, as well as
/// validating the inputs returned by <see name="IPlayerInterface"/>.
/// </summary>
/// <typeparam name="TCard"></typeparam>
public class Player<TCard>(IPlayerInterface playerInterface)
    where TCard : Card
{
    public IEnumerable<CardValue> PeakHand => _hand.Select(card => card.Value);

    private Cards<TCard> _hand = [];

    public Task GiveCards(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        _hand.AddRange(cards);
        return Task.CompletedTask;
    }

    public Task<Cards<TCard>> ClearHand(CancellationToken cancellationToken)
    {
        Cards<TCard> hand = _hand;
        _hand = [];
        return Task.FromResult(hand);
    }

    public Task<TCard> PlayCard(Func<Cards<TCard>, int, bool> validateChosenCard, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Cards<TCard>> PlayCards(Func<Cards<TCard>, List<int>, bool> validateChosenCards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
