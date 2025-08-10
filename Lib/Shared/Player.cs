using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Shared;

// TODO: the divide b/w Player and IPlayerInterface is a lil awkward, esp around the hand ownership.

// TODO: a player name would be cool too

// TODO: attaching error messages to the validation(s) when they eval to false would be slick (that
//      might just be the spec pattern)
//      impl one of the validation types?

/// <summary>
/// This class exists to contain the state for an individual player within a game, as well as
/// validating the inputs returned by <see name="IPlayerInterface"/>.
/// </summary>
/// <typeparam name="TCard"></typeparam>
public class Player<TCard>(IPlayerInterface<TCard> playerInterface, ILogger<Player<TCard>> logger)
    where TCard : Card
{
    public IEnumerable<CardValue> PeakHand => _hand.Select(card => card.Value);

    private Cards<TCard> _hand = [];

    public async Task GiveCards(Cards<TCard> cards, CancellationToken cancellationToken)
    {
        logger.LogInformation("Giving cards to player");
        _hand.AddRange(cards);
        await playerInterface.GiveCards(cards, cancellationToken);
        logger.LogInformation("Gave cards to player");
    }

    public async Task<Cards<TCard>> ClearHand(CancellationToken cancellationToken)
    {
        logger.LogInformation("Clearing player's hand");
        Cards<TCard> hand = _hand;
        _hand = [];
        await playerInterface.ClearHand(cancellationToken);
        logger.LogInformation("Cleared player's hand");
        return hand;
    }

    /// <summary>
    /// </summary>
    /// <param name="validateChosenCard">
    /// This will take the current player hand and the pre-validated in-range index of the card to
    /// play, and return true iff it is valid to play that card.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TCard> PlayCard(Func<Cards<TCard>, int, bool> validateChosenCard, CancellationToken cancellationToken)
    {
        bool validCardToPlay = false;
        int iCardToPlay = -1;
        while (!validCardToPlay)
        {
            iCardToPlay = -1;
            while (iCardToPlay < 0 || iCardToPlay >= _hand.Count)
                iCardToPlay = await playerInterface.PromptForIndexOfCardToPlay(_hand, cancellationToken);

            validCardToPlay = validateChosenCard(_hand, iCardToPlay);
        }

        TCard cardToPlay = _hand[iCardToPlay];
        _hand.RemoveAt(iCardToPlay);
        return cardToPlay;
    }

    /// <summary>
    /// </summary>
    /// <param name="validateChosenCards">
    /// This will take the current player hand and the pre-validated in-range and unique indexes of
    /// the cards to play, and return true iff it is valid to play those cards.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Cards<TCard>> PlayCards(Func<Cards<TCard>, List<int>, bool> validateChosenCards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
