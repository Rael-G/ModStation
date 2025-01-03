using ModManager.Core.Entities;
using ModManager.Core.Exceptions;
using ModManager.Core.Services;

namespace ModManager;

public class Manager(List<Game> games)
{
    public List<Game> Games { get; set; } = games;

    public Game AddGame(string gamePath)
    {
        if (!Games.Any(g => g.GamePath == gamePath))
        {
            var game = new Game(gamePath);
            InjectorService.GamesRepository.Create(game);
            Games.Add(game);
            return game;
        }
        else
        {
            throw new DuplicatedEntity("This game is already registered.");
        }
    }

    public void RemoveGame(Game game)
    {
        game.RemoveGame();
        Games.Remove(game);
    }

    public void Save()
    {
        foreach (var game in Games)
        {
            InjectorService.GamesRepository.Update(game);
        }
    }
}
