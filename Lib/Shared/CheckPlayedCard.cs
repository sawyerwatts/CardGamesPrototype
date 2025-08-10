namespace CardGamesPrototype.Lib.Shared;

public static class CheckPlayedCard
{
    public static bool EnsureTrickSuitIsFollowedIfPossible<TCard>(Cards<TCard> trick, Cards<TCard> playerHand,
        int iPlayerCardToPlay, Suit? bypassSuit = null)
        where TCard : Card
    {
        if (trick.Count == 0)
            return true;
        Suit suitToFollow = trick[0].Value.Suit;

        TCard cardToPlay = playerHand[iPlayerCardToPlay];
        if (cardToPlay.Value.Suit == suitToFollow)
            return true;

        if (bypassSuit is not null && cardToPlay.Value.Suit == bypassSuit)
            return true;

        bool canFollowSuit = playerHand.Any(card => card.Value.Suit == suitToFollow);
        return !canFollowSuit;
    }
}
