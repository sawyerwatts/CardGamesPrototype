namespace CardGamesPrototype.Lib.Common;

public static class Decks
{
    public static Cards Standard52 => new Cards(
    [
        new Card(AceOfHearts.Instance),
        new Card(TwoOfHearts.Instance),
        new Card(ThreeOfHearts.Instance),
        new Card(FourOfHearts.Instance),
        new Card(FiveOfHearts.Instance),
        new Card(SixOfHearts.Instance),
        new Card(SevenOfHearts.Instance),
        new Card(EightOfHearts.Instance),
        new Card(NineOfHearts.Instance),
        new Card(TenOfHearts.Instance),
        new Card(JackOfHearts.Instance),
        new Card(QueenOfHearts.Instance),
        new Card(KingOfHearts.Instance),

        new Card(AceOfSpades.Instance),
        new Card(TwoOfSpades.Instance),
        new Card(ThreeOfSpades.Instance),
        new Card(FourOfSpades.Instance),
        new Card(FiveOfSpades.Instance),
        new Card(SixOfSpades.Instance),
        new Card(SevenOfSpades.Instance),
        new Card(EightOfSpades.Instance),
        new Card(NineOfSpades.Instance),
        new Card(TenOfSpades.Instance),
        new Card(JackOfSpades.Instance),
        new Card(QueenOfSpades.Instance),
        new Card(KingOfSpades.Instance),

        new Card(AceOfDiamonds.Instance),
        new Card(TwoOfDiamonds.Instance),
        new Card(ThreeOfDiamonds.Instance),
        new Card(FourOfDiamonds.Instance),
        new Card(FiveOfDiamonds.Instance),
        new Card(SixOfDiamonds.Instance),
        new Card(SevenOfDiamonds.Instance),
        new Card(EightOfDiamonds.Instance),
        new Card(NineOfDiamonds.Instance),
        new Card(TenOfDiamonds.Instance),
        new Card(JackOfDiamonds.Instance),
        new Card(QueenOfDiamonds.Instance),
        new Card(KingOfDiamonds.Instance),

        new Card(AceOfClubs.Instance),
        new Card(TwoOfClubs.Instance),
        new Card(ThreeOfClubs.Instance),
        new Card(FourOfClubs.Instance),
        new Card(FiveOfClubs.Instance),
        new Card(SixOfClubs.Instance),
        new Card(SevenOfClubs.Instance),
        new Card(EightOfClubs.Instance),
        new Card(NineOfClubs.Instance),
        new Card(TenOfClubs.Instance),
        new Card(JackOfClubs.Instance),
        new Card(QueenOfClubs.Instance),
        new Card(KingOfClubs.Instance),
    ]);

    public static Cards Standard54 => new Cards(
    [
        new Card(Joker0.Instance),
        new Card(Joker1.Instance),
    ]);
}
