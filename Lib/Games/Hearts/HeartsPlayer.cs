namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(IPlayerInterface playerInterface) : Player<HeartsCard>(playerInterface)
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];
}
