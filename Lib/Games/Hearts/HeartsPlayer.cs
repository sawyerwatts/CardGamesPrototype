using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(Player player) : Player
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];

    public override Task SetHand<T>(Cards<T> hand, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task GiveCards<T>(Cards<T> cards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<CardValue> RemoveCard<T>(RemoveCardSpec spec, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<Cards<T>> RemoveCards<T>(RemoveCardsSpec spec, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
