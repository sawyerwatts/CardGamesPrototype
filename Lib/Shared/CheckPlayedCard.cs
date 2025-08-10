namespace CardGamesPrototype.Lib.Shared;

public static class CheckPlayedCard
{
    public static bool EnsureSuitIsFollowedIfPossible<TCard>(Suit suitToFollow, Cards<TCard> playerHand,
        int iPlayerCardToPlay, Suit? bypassSuit = null)
        where TCard : Card
    {
        TCard cardToPlay = playerHand[iPlayerCardToPlay];
        if (cardToPlay.Value.Suit == suitToFollow)
            return true;

        if (bypassSuit is not null && cardToPlay.Value.Suit == bypassSuit)
            return true;

        bool canFollowSuit = playerHand.Any(card => card.Value.Suit == suitToFollow);
        return !canFollowSuit;
    }
}
