using Spectre.Console;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;
using ModStation.Core.Interfaces;

namespace ModManager.Terminal;

public class ModManagerService(IModService modService, IGameService gameService)
{
    private readonly IModService _modService = modService;
    private readonly IGameService _gameService = gameService;

    public async Task ManageMods(Game game)
    {
        static string ModOption(Mod mod)
        {
            return $"{(mod.IsEnabled ? "[green]" : "[red]")}{mod.Name}{(mod.IsEnabled ? " (Enabled)" : " (Disabled)")}[/]";
        }

        while (true)
        {
            var modChoices = game.Mods
                .Select(ModOption)
                .ToList();
            modChoices.Add("Install Mod");
            modChoices.Add("Remove Game");
            modChoices.Add("Change Order");
            modChoices.Add("Back");
            modChoices.Add("Exit");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]Manage mods for: [blue]{game.Name}[/][/]")
                    .AddChoices(modChoices));

            if (choice == "Install Mod")
            {
                await InstallMod(game);
            }
            else if (choice == "Remove Game")
            {
                await RemoveGame(game);
                break;
            }
            else if (choice == "Change Order")
            {
                new OrderMenu(game, _modService).Show();
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                Environment.Exit(0);
            }
            else
            {
                var mod = game.Mods
                    .First(m => choice == ModOption(m));
                await ManageModActions(mod);
            }
        }
    }

    private async Task InstallMod(Game game)
    {
        AnsiConsole.MarkupLine("[green]Enter the mod path:[/]");
        var modPath = ReadLine.Read();

        if (!File.Exists(modPath) && !Directory.Exists(modPath))
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]File not found![/]");
            return;
        }

        var modName = Path.GetFileNameWithoutExtension(modPath);
        AnsiConsole.Clear();
        modName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                .DefaultValue(modName)
                .AllowEmpty()
                .PromptStyle("cyan"));

        try
        {
            await _modService.CreateAsync(modName, modPath, game);
        }
        catch(DuplicatedEntityException e)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
            return;
        }

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[green]{modName} installed![/]");
    }

    private async Task ManageModActions(Mod mod)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]Manage mod: [blue]{mod.Name}[/][/]")
                    .AddChoices(
                        mod.IsEnabled ? "Disable Mod" : "Enable Mod",
                        "Uninstall Mod",
                        "Back",
                        "Exit"
                        )
                    );

            if (choice == "Enable Mod")
            {
                await _modService.EnableAsync(mod);
            }
            else if (choice == "Disable Mod")
            {
                await _modService.DisableAsync(mod);
            }
            else if (choice == "Uninstall Mod")
            {
                await _modService.DeleteAsync(mod);
                break;
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                Environment.Exit(0);
            }
        }
    }

    private async Task RemoveGame(Game game)
    {
        var confirm = AnsiConsole.Confirm("[red]This action will remove all mods from [blue]" + game.Name + "[/] and restore the original files. Do you want to continue?[/]", false);
        if (confirm)
        {
            AnsiConsole.Clear();
            await AnsiConsole.Status().Start($"Removing {game.Name} ...", async ctx =>
            {
                await _gameService.DeleteAsync(game);
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[green]{game.Name} has been removed![/]");
            });
        }
    }
}