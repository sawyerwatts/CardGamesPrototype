using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Shared;

// TODO: the divide b/w Player and IPlayerPortal is a lil awkward, esp around the hand ownership.
//      the player portal doesn't know the hand state unless it's being prompted for a card to play
//      redesign
//          HeartsGame takes abstract PlayerPortal, and PlayerPortal is injected w/ PlayerState or whatev

// TODO: but what if we wanted to show the total points of all players?
//      then we'd need to allow for public vs private game state
//          some games will want to show how many cards others have, so hand.count would be public
//      could also have ultra private for tricks taken and card counting

// TODO: attaching error messages to the validation(s) when they eval to false would be slick (that
//      might just be the spec pattern)
//      impl one of the validation types?

/// <summary>
/// This class exists to contain the state for an individual player within a game, as well as
/// validating the inputs returned by <see name="IPlayerInterface"/>.
/// </summary>
/// <typeparam name="TCard"></typeparam>
public class Player<TCard>(string name, IPlayerPortal<TCard> playerPortal, ILogger<Player<TCard>> logger)
    where TCard : Card
{
    public string Name => name;

    public Cards<TCard> Hand { get; set; } = [];

    /// <param name="validateChosenCard">
    /// This will take the current player hand and the pre-validated in-range index of the card to
    /// play, and return true iff it is valid to play that card.
    /// </param>
    /// <param name="cancellationToken"></param>
    public async Task<TCard> PlayCard(Func<Cards<TCard>, int, bool> validateChosenCard, CancellationToken cancellationToken)
    {
        bool validCardToPlay = false;
        int iCardToPlay = -1;
        while (!validCardToPlay)
        {
            iCardToPlay = await playerPortal.PromptForIndexOfCardToPlay(Hand, cancellationToken);
            if (iCardToPlay < 0 || iCardToPlay >= Hand.Count)
                continue;

            validCardToPlay = validateChosenCard(Hand, iCardToPlay);
        }

        TCard cardToPlay = Hand[iCardToPlay];
        Hand.RemoveAt(iCardToPlay);
        return cardToPlay;
    }

    /// <param name="validateChosenCards">
    /// This will take the current player hand and the pre-validated in-range and unique indexes of
    /// the cards to play, and return true iff it is valid to play those cards.
    /// </param>
    /// <param name="cancellationToken"></param>
    public async Task<Cards<TCard>> PlayCards(Func<Cards<TCard>, List<int>, bool> validateChosenCards, CancellationToken cancellationToken)
    {
        bool validCardsToPlay = false;
        List<int> iCardsToPlay = [];
        while (!validCardsToPlay)
        {
            iCardsToPlay = await playerPortal.PromptForIndexesOfCardsToPlay(Hand, cancellationToken);
            if (iCardsToPlay.Count != iCardsToPlay.Distinct().Count())
                continue;

            if (iCardsToPlay.Any(iCardToPlay => iCardToPlay < 0 || iCardToPlay >= Hand.Count))
                continue;

            validCardsToPlay = validateChosenCards(Hand, iCardsToPlay);
        }

        Cards<TCard> cardsToPlay = new(capacity: iCardsToPlay.Count);
        foreach (int iCardToPlay in iCardsToPlay.OrderDescending())
        {
            cardsToPlay.Add(Hand[iCardToPlay]);
            Hand.RemoveAt(iCardToPlay);
        }

        return cardsToPlay;
    }
}
