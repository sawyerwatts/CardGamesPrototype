namespace CardGamesPrototype.Lib.Common;

public class Player
{
    public IReadOnlyList<CardValue> PeakCards => _cards.AsReadOnly();

    private List<CardValue> _cards = [];

    public virtual Task SetHand<T>(Cards<T> hand, CancellationToken cancellationToken)
        where T : Card
    {
        throw new NotImplementedException();
    }

    public virtual Task GiveCards<T>(Cards<T> cards, CancellationToken cancellationToken)
        where T : Card
    {
        throw new NotImplementedException();
    }

    public virtual Task<CardValue> RemoveCard<T>(RemoveCardSpec spec,
        CancellationToken cancellationToken)
        where T : Card
    {
        throw new NotImplementedException();
    }

    public sealed record RemoveCardSpec(
        CardValue? PlayThisCard,
        Suit? SuitToFollow);

    public virtual Task<Cards<T>> RemoveCards<T>(RemoveCardsSpec spec,
        CancellationToken cancellationToken)
        where T : Card
    {
        throw new NotImplementedException();
    }

    public sealed record RemoveCardsSpec(
        int? MinCardsToRemove,
        int? MaxCardsToRemove);
}
