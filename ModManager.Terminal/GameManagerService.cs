using Spectre.Console;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;

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

        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                .DefaultValue(Path.GetFileName(gamePath))
                .AllowEmpty()
                .PromptStyle("cyan"));

        try
        {
            _manager.AddGame(gamePath, name);
            _manager.Save();

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[green]{name} added successfully![/]");
        }
        catch (DuplicatedEntity e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
        }
    }
}
