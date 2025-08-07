using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed record HeartsPlayer(Player Player)
{
    public int Score { get; set; } = 0;
    public List<HeartsCards> TricksTakenThisRound { get; set; } = [];
}
