using Spectre.Console;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;
using ModStation.Core.Interfaces;

namespace ModManager.Terminal;

public class GameManagerService
{
    private readonly IGameService _gameService;

    private List<Game> _games;

    public GameManagerService(IGameService gameService)
    {
        _gameService = gameService;
        _games = [];
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        _games = [.. await _gameService.GetAllAsync()];
    }

    public List<string> GetGameChoices()
    {
        return _games.Select(g => $"[green]{g.Name}[/]").ToList();
    }

    public Game GetGameByName(string name)
    {
        return _games.Last(g => name.Contains(g.Name));
    }

    public async Task AddGame()
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
            await _gameService.CreateAsync(gamePath, name);

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[green]{name} added successfully![/]");
        }
        catch (DuplicatedEntityException e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
        }
    }
}
