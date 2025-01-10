using System.Threading.Tasks;
using ModStation.Core.Interfaces;
using Spectre.Console;

namespace ModManager.Terminal;

public class MenuService
{
    private readonly GameManagerService _gameManager;
    private readonly ModManagerService _modManager;

    public MenuService(IGameService gameService, IModService modService)
    {
        _gameManager = new GameManagerService(gameService);
        _modManager = new ModManagerService(modService, gameService);
    }

    public async Task Run()
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
                await _gameManager.AddGame();
            }
            else if (choice == "Exit")
            {
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