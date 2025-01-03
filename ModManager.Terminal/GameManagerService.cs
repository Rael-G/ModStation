using Spectre.Console;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;
using ModManager.Core.Services;

namespace ModManager.Terminal;

public class GameManagerService(Manager manager)
{
    private readonly Manager _manager = manager;

    public List<string> GetGameChoices()
    {
        return _manager.Games.Select(g => $"[green]{g.Name}[/]").ToList();
    }

    public Game GetGameByName(string name)
    {
        return _manager.Games.First(g => name.Contains(g.Name));
    }

    public void AddGame()
    {
        AnsiConsole.MarkupLine("[green]Enter the game path:[/]");
        var gamePath = ReadLine.Read();

        if (!Directory.Exists(gamePath))
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]Invalid path![/]");
            return;
        }

        try
        {
            var game = _manager.AddGame(gamePath);

            game.Name = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                    .DefaultValue(game.Name)
                    .AllowEmpty()
                    .PromptStyle("cyan"));

            _manager.Save();
            AnsiConsole.MarkupLine("[green]Game added successfully![/]");
        }
        catch (DuplicatedEntity e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
        }
    }

    public void Save()
    {
        _manager.Save();
    }
}
