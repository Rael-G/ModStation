using ModManager.Core.Services;
using Spectre.Console;

namespace ModManager.Terminal;

public class MenuService
{
    private readonly Manager _manager = new(InjectorService.GamesRepository.GetAll().ToList());
    private readonly GameManagerService _gameManager;
    private readonly ModManagerService _modManager;

    public MenuService()
    {
        _gameManager = new GameManagerService(_manager);
        _modManager = new ModManagerService(_manager);
    }

    public void Run()
    {
        while (true)
        {
            var choices = _gameManager.GetGameChoices();
            choices.Add("Add Game");
            choices.Add("Exit");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Manage your games[/]")
                    .AddChoices(choices));

            if (choice == "Add Game")
            {
                _gameManager.AddGame();
            }
            else if (choice == "Exit")
            {
                _manager.Save();
                Environment.Exit(0);
            }
            else
            {
                var game = _gameManager.GetGameByName(choice);
                _modManager.ManageMods(game);
            }
        }
    }
}