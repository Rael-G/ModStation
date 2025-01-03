using Spectre.Console;
using ModManager.Core.Entities;

namespace ModManager.Terminal;

public class ModManagerService(Manager manager)
{
    private readonly Manager _manager = manager;

    public void ManageMods(Game game)
    {
        while (true)
        {
            var modChoices = game.Mods
                .Select(m => $"{(m.IsEnable ? "[green]" : "[red]")}{m.Name}{(m.IsEnable ? " (Enabled)" : " (Disabled)")}[/]")
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
                InstallMod(game);
            }
            else if (choice == "Remove Game")
            {
                RemoveGame(game);
                break;
            }
            else if (choice == "Change Order")
            {
                new OrderMenu(game, _manager).Show();
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                _manager.Save();
                Environment.Exit(0);
            }
            else
            {
                var modName = choice.Split(" (")[0];
                var mod = game.Mods.First(m => modName.Contains(m.Name));
                ManageModActions(mod);
            }
        }
    }

    private void InstallMod(Game game)
    {
        AnsiConsole.MarkupLine("[green]Enter the mod path:[/]");
        var modPath = ReadLine.Read();

        if (!File.Exists(modPath) && !Directory.Exists(modPath))
        {
            AnsiConsole.MarkupLine("[red]File not found![/]");
            return;
        }

        var modName = Path.GetFileNameWithoutExtension(modPath);
        modName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                .DefaultValue(modName)
                .AllowEmpty()
                .PromptStyle("cyan"));

        game.InstallMod(modPath, modName);

        AnsiConsole.MarkupLine($"[green]{modName} installed and enabled![/]");
    }

    private void ManageModActions(Mod mod)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]Manage mod: [blue]{mod.Name}[/][/]")
                    .AddChoices(
                        mod.IsEnable ? "Disable Mod" : "Enable Mod",
                        "Uninstall Mod",
                        "Back",
                        "Exit"
                        )
                    );

            if (choice == "Enable Mod")
            {
                mod.Enable();
            }
            else if (choice == "Disable Mod")
            {
                mod.Disable();
            }
            else if (choice == "Uninstall Mod")
            {
                mod.Game.UninstallMod(mod);
                break;
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                _manager.Save();
                Environment.Exit(0);
            }
        }
    }

    private void RemoveGame(Game game)
    {
        AnsiConsole.Clear();
        var confirm = AnsiConsole.Confirm("[red]This action will remove all mods from [blue]" + game.Name + "[/]. Are you sure?[/]", false);
        if (confirm)
        {
            AnsiConsole.Status().Start($"Removing {game.Name} ...", ctx =>
            {
                _manager.RemoveGame(game);
                _manager.Save();
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[green]{game.Name} has been removed![/]");
            });
        }
    }
}