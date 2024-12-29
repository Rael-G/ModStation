namespace ModManager;

public class Manager(List<Game> games)
{
    public List<Game> Games { get; set; } = games;

    public void AddGame(string gamePath)
    {
        if (!Games.Any(g => g.GamePath == gamePath))
        {
            var game = new Game(gamePath);
            Games.Add(game);
        }
        else
        {
            Console.WriteLine("This game is already registered");
        }
    }
}
