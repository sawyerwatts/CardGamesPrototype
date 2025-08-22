namespace CardGamesPrototype.Lib.Games.Hearts;

public static class CheckPlayedHeartsCard
{
    public static bool AreHeartsArePlayedOnlyAfterBeingBroken(bool hasHeartsBeenBroken, Cards<HeartsCard> hand, int iCardToPlay) =>
        hand[iCardToPlay].Value.Suit is not Suit.Hearts || hasHeartsBeenBroken;
}
