using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(Player player) : Player
{
    public int Score { get; set; } = 0;
    public List<HeartsCards> TricksTakenThisRound { get; set; } = [];

    public override Task SetHand(Cards hand, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task GiveCards(Cards cards, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<CardValue> RemoveCard(RemoveCardSpec spec, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<Cards> RemoveCards(RemoveCardsSpec spec, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
