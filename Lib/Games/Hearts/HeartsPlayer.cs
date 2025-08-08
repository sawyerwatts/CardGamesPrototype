namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(Player<HeartsCard> player) : Player<HeartsCard>
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];
}
