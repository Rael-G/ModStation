using ModManager.Core.Entities;
using Spectre.Console;

namespace ModManager.Terminal;

public class OrderMenu(Game game, Manager manager)
{
    private readonly Game _game = game ?? throw new ArgumentNullException(nameof(game));
    private readonly Manager _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    private List<Mod> _orderedMods = new List<Mod>();
    private int _currentIndex = 0;

    public void Show()
    {
        if (!_game.Mods.Any())
        {
            AnsiConsole.MarkupLine("[red]No mods available to adjust the order.[/]");
            return;
        }

        _orderedMods = _game.Mods.OrderBy(m => m.Order).ToList();

        while (true)
        {
            RenderMenu();

            var key = Console.ReadKey(true).Key;
            HandleKeyPress(key);

            if (key == ConsoleKey.Enter)
            {
                AdjustOrder();
            }
            else if (key == ConsoleKey.Escape)
            {
                AnsiConsole.Clear();
                return;
            }
        }
    }

    private void RenderMenu()
    {
        AnsiConsole.Clear();
        var table = new Table()
            .Border(TableBorder.Minimal)
            .AddColumns("Order", "Name");

        foreach (var mod in _orderedMods.Select((m, index) => new { Mod = m, Index = index }))
        {
            string orderText = (mod.Mod.Order + 1).ToString();
            string modName = mod.Mod.Name;

            if (mod.Index == _currentIndex)
            {
                table.AddRow($"[blue]{orderText}[/]", $"[blue]{modName}[/]");
            }
            else
            {
                table.AddRow(orderText, modName);
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\n[gray]Use the ↑ ↓ arrow keys to adjust the position, Enter to confirm and Escape do finish.[/]");
    }

    private void HandleKeyPress(ConsoleKey key)
    {
        if (key == ConsoleKey.UpArrow && _currentIndex > 0)
        {
            _currentIndex--;
        }
        else if (key == ConsoleKey.DownArrow && _currentIndex < _orderedMods.Count - 1)
        {
            _currentIndex++;
        }
    }

    private void AdjustOrder()
    {
        while (true)
        {
            RenderOrderAdjustmentMenu();

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && _currentIndex > 0)
            {
                SwapMods(_currentIndex, --_currentIndex);
            }
            else if (key == ConsoleKey.DownArrow && _currentIndex < _orderedMods.Count - 1)
            {
                SwapMods(_currentIndex, ++_currentIndex);
            }
            else if (key == ConsoleKey.Enter)
            {
                ApplyOrderChanges();
                break;
            }
        }
    }

    private void RenderOrderAdjustmentMenu()
    {
        AnsiConsole.Clear();
        var table = new Table()
            .Border(TableBorder.Minimal)
            .AddColumns("Order", "Name");

        foreach (var mod in _orderedMods.Select((m, index) => new { Mod = m, Index = index }))
        {
            string orderText = (mod.Mod.Order + 1).ToString();
            string modName = mod.Mod.Name;

            if (mod.Index == _currentIndex)
            {
                table.AddRow($"[green]{orderText}[/]", $"[green]{modName}[/]");
            }
            else
            {
                table.AddRow(orderText, modName);
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\n[gray]Use the ↑ ↓ arrow keys to adjust the position and press Enter to confirm the new position.[/]");
    }

    private void SwapMods(int index1, int index2)
    {
        (_orderedMods[index2], _orderedMods[index1]) = (_orderedMods[index1], _orderedMods[index2]);
    }

    private void ApplyOrderChanges()
    {
        AnsiConsole.Status().Start("Adjusting mod order...", ctx =>
        {
            var modToMove = _orderedMods[_currentIndex];
            _game.SwapOrder(modToMove, _currentIndex);
            _manager.Save();
        });

        AnsiConsole.MarkupLine($"[green]The order of [blue]{_orderedMods[_currentIndex].Name}[/] was adjusted to position {_currentIndex + 1}![/]");
    }
}

