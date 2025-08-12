using Microsoft.Extensions.Logging;

namespace CardGamesPrototype.Lib.Games.Hearts;

public sealed class HeartsPlayer(IPlayerInterface<HeartsCard> playerInterface, ILogger<HeartsPlayer> logger)
    : Player<HeartsCard>(playerInterface, logger)
{
    public int Score { get; set; } = 0;
    public List<Cards<HeartsCard>> TricksTakenThisRound { get; set; } = [];

    public class Factory(ILogger<HeartsPlayer> logger)
    {
        public HeartsPlayer Make(IPlayerInterface<HeartsCard> playerInterface) => new HeartsPlayer(playerInterface, logger);
    }
}
