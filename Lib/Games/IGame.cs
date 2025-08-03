namespace CardGamesPrototype.Lib.Games;

public interface IGame
{
    Task Play(CancellationToken cancellationToken);
}
