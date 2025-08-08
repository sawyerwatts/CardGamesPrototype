namespace CardGamesPrototype.Lib.Common;

public abstract class Player
{
    public IReadOnlyList<CardValue> PeakCards => _cards.AsReadOnly();

    private List<CardValue> _cards = [];

    public abstract Task SetHand<T>(Cards<T> hand, CancellationToken cancellationToken)
        where T : Card;

    public abstract Task GiveCards<T>(Cards<T> cards, CancellationToken cancellationToken)
        where T : Card;

    public abstract Task<CardValue> RemoveCard<T>(RemoveCardSpec spec, CancellationToken cancellationToken)
        where T : Card;

    public sealed record RemoveCardSpec(
        CardValue? PlayThisCard,
        Suit? SuitToFollow);

    public abstract Task<Cards<T>> RemoveCards<T>(RemoveCardsSpec spec, CancellationToken cancellationToken)
        where T : Card;

    public sealed record RemoveCardsSpec(
        int? MinCardsToRemove,
        int? MaxCardsToRemove);
}
