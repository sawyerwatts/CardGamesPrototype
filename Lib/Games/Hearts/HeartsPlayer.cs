using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(string name, IPlayerPortal<HeartsCard> playerPortal, ILogger<HeartsPlayer> logger)
    : Player<HeartsCard>(name, playerPortal, logger)
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];

    public class Factory(ILogger<HeartsPlayer> logger)
    {
        public HeartsPlayer Make(string name, IPlayerPortal<HeartsCard> playerPortal) => new HeartsPlayer(name, playerPortal, logger);
    }
}
