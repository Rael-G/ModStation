using Spectre.Console;
using ModManager.Core.Entities;
using ModManager.Core.Exceptions;

namespace ModManager.Terminal;

public class ModManagerService(Manager manager)
{
    private readonly Manager _manager = manager;

    public void ManageMods(Game game)
    {
        static string ModOption(Mod mod)
        {
            return $"{(mod.IsEnable ? "[green]" : "[red]")}{mod.Name}{(mod.IsEnable ? " (Enabled)" : " (Disabled)")}[/]";
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
                var mod = game.Mods
                    .First(m => choice == ModOption(m));
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
            game.InstallMod(modPath, modName);
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
        var confirm = AnsiConsole.Confirm("[red]This action will remove all mods from [blue]" + game.Name + "[/] and restore the original files. Do you want to continue?[/]", false);
        if (confirm)
        {
            AnsiConsole.Clear();
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