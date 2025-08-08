using CardGamesPrototype.Lib.Common;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(Player player) : Player
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];
}
