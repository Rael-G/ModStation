using Spectre.Console;
using ModManager.Core.Entities;

namespace ModManager.Terminal;

public class ModManagerService(Manager manager)
{
    public readonly Manager _manager = manager;

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
                SwapOrderMenu(game);
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

        AnsiConsole.MarkupLine("[green]Mod installed and enabled successfully![/]");
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

    private void SwapOrderMenu(Game game)
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
                            _manager.Save();
                        });
                        AnsiConsole.MarkupLine($"[green]The order of mod [blue]{orderedMods[currentIndex].Name}[/] was successfully adjusted to position {currentIndex + 1}![/]");
                        return;
                    }
                }
            }
        }
    }
}