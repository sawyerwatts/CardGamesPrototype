using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Shared;

// TODO: the divide b/w Player and IPlayerInterface is a lil awkward, esp around the hand ownership.

// TODO: attaching error messages to the validation(s) when they eval to false would be slick (that
//      might just be the spec pattern)
//      impl one of the validation types?

/// <summary>
/// This class exists to contain the state for an individual player within a game, as well as
/// validating the inputs returned by <see name="IPlayerInterface"/>.
/// </summary>
/// <typeparam name="TCard"></typeparam>
public class Player<TCard>(string name, IPlayerInterface<TCard> playerInterface, ILogger<Player<TCard>> logger)
    where TCard : Card
{
    public string Name => name;

    public Cards<TCard> Hand { get; set; } = [];

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
            while (iCardToPlay < 0 || iCardToPlay >= Hand.Count)
                iCardToPlay = await playerInterface.PromptForIndexOfCardToPlay(Hand, cancellationToken);

            validCardToPlay = validateChosenCard(Hand, iCardToPlay);
        }

        TCard cardToPlay = Hand[iCardToPlay];
        Hand.RemoveAt(iCardToPlay);
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
