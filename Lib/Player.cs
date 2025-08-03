namespace CardGamesPrototype.Lib;

// TODO: make a separate IPlayerInterface and leave this as purely concrete

// TODO: a player name would be cool too

public abstract class Player
{
    public abstract Task SetHand(List<Card> hand, CancellationToken cancellationToken);

    public abstract Task GiveCards(List<Card> cards, CancellationToken cancellationToken);

    public abstract Task<Card> RemoveCard(RemoveCardSpec spec, CancellationToken cancellationToken);

    public sealed record RemoveCardSpec(
        Card? PlayThisCard,
        Suit? SuitToFollow);

    public abstract Task<List<Card>> RemoveCards(RemoveCardsSpec spec, CancellationToken cancellationToken);

    public sealed record RemoveCardsSpec(
        int? MinCardsToRemove,
        int? MaxCardsToRemove);
}
