namespace CardGamesPrototype.Lib.Common;

// TODO: mv to own file
// TODO: having an AI implementation would be slick
//      would be cool if it counted cards too
public interface IPlayerInterface
{
}

// TODO: attaching error messages to the validation(s) when they eval to false would be slick (that
//      might just be the spec pattern)
//      impl one of the validation types?

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

    /// <summary>
    /// </summary>
    /// <param name="validateChosenCard">
    /// This will take the current player hand and the pre-validated in-range index of the card to
    /// play, and return true iff it is valid to play that card.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<TCard> PlayCard(Func<Cards<TCard>, int, bool> validateChosenCard, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// </summary>
    /// <param name="validateChosenCards">
    /// This will take the current player hand and the pre-validated in-range and unique indexes of
    /// the cards to play, and return true iff it is valid to play those cards.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Cards<TCard>> PlayCards(Func<Cards<TCard>, List<int>, bool> validateChosenCards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
