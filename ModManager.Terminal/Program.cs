using Spectre.Console;
using ModManager;
using ModManager.Core.Entities;
using ModManager.Core.Services;
using ModManager.Core.Exceptions;

class Program
{
    static readonly Manager manager = new([]);

    static void Main(string[] args)
    {
        Console.Clear();

        if (!Directory.Exists(InjectorService.DataDirectory))
        {
            Directory.CreateDirectory(InjectorService.DataDirectory);
        }

        manager.Games = InjectorService.GamesRepository.GetAll().ToList();

        ManageGames();

    }

    static void ManageGames()
    {
        while (true)
        {
            var choices = manager.Games.Select(g => $"[green]{g.Name}[/]").ToList();
            choices.Add("Add Game");
            choices.Add("Back");
            choices.Add("Exit");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]ModManager[/]")
                    .AddChoices(choices));

            if (choice == "Add Game")
            {
                AddGame();
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                EndApplication();
            }
            else
            {
                var game = manager.Games.First(g => choice.Contains(g.Name));
                ManageMods(game);
            }
        }
    }

    static void AddGame()
    {
        AnsiConsole.MarkupLine("[green]Enter the game path:[/]");
        var gamePath = ReadLine.Read();

        if (Directory.Exists(gamePath))
        {
            Game game = null!;
            try
            {
                AnsiConsole.Status().Start("Adding Game ...", ctx =>
                {
                    game = manager.AddGame(gamePath);
                });
            }
            catch (DuplicatedEntity e)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[red]" + e.Message + "[/]");
                return;
            }

            var newName = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                    .DefaultValue(game.Name)
                    .AllowEmpty()
                    .PromptStyle("cyan"));

            game.Name = newName;

            manager.Save();
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[green]Game added successfully![/]");
        }
        else
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]Invalid path![/]");
        }
    }

    static void ManageMods(Game game)
    {
        AnsiConsole.Clear();
        while (true)
        {
            var modChoices = game.Mods.Select(m => (m.IsEnable ? "[green]" : "[red]") + m.Name + (m.IsEnable ? " (Enabled)" : " (Disabled)") + "[/]").ToList();
            modChoices.Add("Install Mod");
            modChoices.Add("Adjust Order");
            modChoices.Add("Remove Game");
            modChoices.Add("Back");
            modChoices.Add("Exit");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]Manage mods for the game: [blue]{game.Name}[/][/]") 
                    .AddChoices(modChoices));

            if (choice == "Install Mod")
            {
                InstallMod(game);
            }
            else if (choice == "Adjust Order")
            {
                SwapOrderMenu(game);
            }
            else if (choice == "Remove Game")
            {
                RemoveGame(game);
                return;
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                EndApplication();
            }
            else
            {
                var modName = choice.Split(" (")[0];
                var mod = game.Mods.First(m => modName.Contains(m.Name));
                ManageModActions(mod);
            }

            AnsiConsole.Clear();
        }
    }

    static void InstallMod(Game game)
    {
        AnsiConsole.MarkupLine("[green]Enter the mod path:[/]");
        var modPath = ReadLine.Read();

        Mod mod = null!;

        if (File.Exists(modPath) || Directory.Exists(modPath))
        {
            AnsiConsole.Status().Start("Installing Mod ...", ctx =>
            {
                mod = game.InstallMod(modPath);
                manager.Save();
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[green]Mod installed and enabled successfully![/]");
            });
        }
        else
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]File not found![/]");
        }

        var newName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Do you want to keep this name?[/]")
                .DefaultValue(mod.Name)
                .AllowEmpty()
                .PromptStyle("cyan"));

        mod.Name = newName;
        manager.Save();
        AnsiConsole.Clear();
    }

static void ManageModActions(Mod mod)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]What do you want to do with the mod: [blue]{mod.Name}[/]?[/]")
                    .AddChoices(mod.IsEnable ? "Disable Mod" : "Enable Mod", "Update Mod", "Uninstall Mod", "Back", "Exit"));

            if (choice == "Enable Mod")
            {
                AnsiConsole.Status().Start("Enabling Mod ...", ctx =>
                {
                    mod.Enable();
                    manager.Save();
                });
            }
            else if (choice == "Disable Mod")
            {
                AnsiConsole.Status().Start("Disabling Mod ...", ctx =>
                {
                    mod.Disable();
                    manager.Save();
                });
            }
            else if (choice == "Update Mod")
            {
                AnsiConsole.MarkupLine("[green]Enter the ZIP file path of the mod:[/]");
                var zipPath = ReadLine.Read();
                AnsiConsole.Status().Start("Updating Mod ...", ctx =>
                {
                    mod.Game.UpdateMod(mod, zipPath);
                    manager.Save();
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[green]Mod updated successfully![/]");
                });
                break;
            }
            else if (choice == "Uninstall Mod")
            {
                AnsiConsole.Status().Start("Uninstalling Mod ...", ctx =>
                {
                    mod.Game.UninstallMod(mod);
                    manager.Save();
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[green]Mod uninstalled successfully![/]");
                });
                break;
            }
            else if (choice == "Back")
            {
                break;
            }
            else if (choice == "Exit")
            {
                EndApplication();
            }
            AnsiConsole.Clear();
        }
    }

    static void RemoveGame(Game game)
    {
        AnsiConsole.Clear();
        var confirm = AnsiConsole.Confirm("[red]This action will remove all mods from [blue]" + game.Name + "[/]. Are you sure?[/]", false);
        if (confirm)
        {
            AnsiConsole.Status().Start($"Removing {game.Name} ...", ctx =>
            {
                manager.RemoveGame(game);
                manager.Save();
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[green]{game.Name} has been removed![/]");
            });
        }
    }

    static void SwapOrderMenu(Game game)
    {
        if (!game.Mods.Any())
        {
            AnsiConsole.MarkupLine("[red]No mods available to adjust the order.[/]");
            return;
        }

        int currentIndex = 0;
        var orderedMods = game.Mods.OrderBy(m => m.Order).ToList();

        while (true)
        {
            AnsiConsole.Clear();
            var table = new Table()
                .Border(TableBorder.Minimal)
                .AddColumns("Order", "Name");

            foreach (var m in orderedMods.Select((m, index) => new { m, index }))
            {
                string orderText = (m.index + 1).ToString();
                string modName = m.m.Name;

                if (m.index == currentIndex)
                {
                    table.AddRow($"[blue]{orderText}[/]", $"[blue]{modName}[/]");
                }
                else
                {
                    table.AddRow(orderText, modName);
                }
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("\n[gray]Use the ↑ ↓ arrow keys to adjust the position and press Enter to confirm.[/]");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && currentIndex > 0)
            {
                currentIndex--;
            }
            else if (key == ConsoleKey.DownArrow && currentIndex < orderedMods.Count - 1)
            {
                currentIndex++;
            }
            else if (key == ConsoleKey.Enter)
            {
                var mod = orderedMods[currentIndex];
                int selectedOrder = mod.Order;

                while (true)
                {
                    AnsiConsole.Clear();
                    var tableInMove = new Table()
                        .Border(TableBorder.Minimal)
                        .AddColumns("Order", "Name");

                    foreach (var m in orderedMods.Select((m, index) => new { m, index }))
                    {
                        string orderText = (m.index + 1).ToString();
                        string modName = m.m.Name;

                        if (m.index == currentIndex)
                        {
                            tableInMove.AddRow($"[green]{orderText}[/]", $"[green]{modName}[/]");
                        }
                        else
                        {
                            tableInMove.AddRow(orderText, modName);
                        }
                    }

                    AnsiConsole.Write(tableInMove);
                    AnsiConsole.MarkupLine("\n[gray]Use the ↑ ↓ arrow keys to adjust the position and press Enter to confirm the new position.[/]");

                    key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.UpArrow && currentIndex > 0)
                    {
                        var temp = orderedMods[currentIndex];
                        orderedMods[currentIndex] = orderedMods[currentIndex - 1];
                        orderedMods[currentIndex - 1] = temp;
                        currentIndex--;
                    }
                    else if (key == ConsoleKey.DownArrow && currentIndex < orderedMods.Count - 1)
                    {
                        var temp = orderedMods[currentIndex];
                        orderedMods[currentIndex] = orderedMods[currentIndex + 1];
                        orderedMods[currentIndex + 1] = temp;
                        currentIndex++;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        AnsiConsole.Status().Start("Adjusting mod order...", ctx =>
                        {
                            var modToMove = orderedMods[currentIndex];
                            game.SwapOrder(modToMove, currentIndex + 1); 
                            manager.Save();
                        });
                        AnsiConsole.MarkupLine($"[green]The order of mod [blue]{orderedMods[currentIndex].Name}[/] was successfully adjusted to position {currentIndex + 1}![/]");
                        return;
                    }
                }
            }
        }
    }

    static void EndApplication()
    {
        manager.Save();
        Environment.Exit(0);
    }

}
