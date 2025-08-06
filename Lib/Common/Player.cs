namespace CardGamesPrototype.Lib.Common;

public abstract class Player
{
    public IReadOnlyList<CardValue> PeakCards => _cards.AsReadOnly();

    private List<CardValue> _cards = [];

    public abstract Task SetHand(Cards hand, CancellationToken cancellationToken);

    public abstract Task GiveCards(Cards cards, CancellationToken cancellationToken);

    public abstract Task<CardValue> RemoveCard(RemoveCardSpec spec, CancellationToken cancellationToken);

    public sealed record RemoveCardSpec(
        CardValue? PlayThisCard,
        Suit? SuitToFollow);

    public abstract Task<Cards> RemoveCards(RemoveCardsSpec spec, CancellationToken cancellationToken);

    public sealed record RemoveCardsSpec(
        int? MinCardsToRemove,
        int? MaxCardsToRemove);
}
