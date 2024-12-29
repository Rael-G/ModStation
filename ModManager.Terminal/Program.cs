using Spectre.Console;
using ModManager;

class Program
{
    static Manager manager = new Manager(new List<Game>());

    static void Main(string[] args)
    {
        // Carregar jogos salvos
        manager.Games = Persistance.Load();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Escolha uma opção:[/]")
                    .AddChoices("Gerenciar Jogos", "Sair"));

            if (choice == "Gerenciar Jogos")
            {
                ManageGames();
            }
            else
            {
                // Salvar dados antes de sair
                Persistance.Save(manager.Games);
                break;
            }
        }
    }

    static void ManageGames()
    {
        while (true)
        {
            var choices = manager.Games.Select(g => g.Name).ToList();
            choices.Add("Adicionar Jogo");
            choices.Add("Voltar");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Escolha um jogo para gerenciar ou adicione um novo:[/]")
                    .AddChoices(choices));

            if (choice == "Adicionar Jogo")
            {
                AddGame();
            }
            else if (choice == "Voltar")
            {
                break;
            }
            else
            {
                var game = manager.Games.First(g => g.Name == choice);
                ManageMods(game);
            }
        }
    }

    static void AddGame()
    {
        AnsiConsole.MarkupLine("[green]Insira o caminho do jogo:[/]");
        var gamePath = ReadLine.Read();

        if (Directory.Exists(gamePath))
        {
            manager.AddGame(gamePath);
            Persistance.Save(manager.Games);
            AnsiConsole.MarkupLine("[green]Jogo adicionado com sucesso![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Caminho inválido![/]");
        }
    }

    static void ManageMods(Game game)
    {
        while (true)
        {
            var modChoices = game.Mods.Select(m => m.Name + (m.IsEnable ? " (Ativado)" : " (Desativado)")).ToList();
            modChoices.Add("Instalar Mod");
            modChoices.Add("Voltar");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]Gerenciar mods para o jogo: [blue]{game.Name}[/][/]")
                    .AddChoices(modChoices));

            if (choice == "Instalar Mod")
            {
                InstallMod(game);
            }
            else if (choice == "Voltar")
            {
                break;
            }
            else
            {
                var modName = choice.Split(" (")[0];
                var mod = game.Mods.First(m => m.Name == modName);
                ManageModActions(mod);
            }
        }
    }

    static void InstallMod(Game game)
    {
        AnsiConsole.MarkupLine("[green]Insira o caminho do arquivo ZIP do mod:[/]");
        var zipPath = ReadLine.Read();

        if (File.Exists(zipPath))
        {
            game.InstallMod(zipPath);
            Persistance.Save(manager.Games);
            AnsiConsole.MarkupLine("[green]Mod instalado e ativado com sucesso![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Arquivo ZIP não encontrado![/]");
        }
    }

    static void ManageModActions(Mod mod)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]O que deseja fazer com o mod: [blue]{mod.Name}[/]?[/]")
                    .AddChoices(mod.IsEnable ? "Desativar Mod" : "Ativar Mod", "Atualizar Mod", "Desinstalar Mod", "Voltar"));

            if (choice == "Ativar Mod")
            {
                mod.Enable();
                Persistance.Save(manager.Games);
            }
            else if (choice == "Desativar Mod")
            {
                mod.Disable();
                Persistance.Save(manager.Games);
            }
            else if (choice == "Atualizar Mod")
            {
                AnsiConsole.MarkupLine("[green]Insira o caminho do arquivo ZIP do mod:[/]");
                var zipPath = ReadLine.Read();
                try
                {
                    mod.Game.UpdateMod(mod, zipPath);
                    Persistance.Save(manager.Games);
                    AnsiConsole.MarkupLine("[green]Mod atualizado com sucesso![/]");
                }
                catch(UnauthorizedAccessException)
                {
                    AnsiConsole.MarkupLine("[red]Acesso negado! Tente executar como administrador.[/]");
                    throw;
                }
                break;
            }
            else if (choice == "Desinstalar Mod")
            {
                try
                {
                    mod.Game.UninstallMod(mod);
                    Persistance.Save(manager.Games);
                    AnsiConsole.MarkupLine("[green]Mod desinstalado com sucesso![/]");
                }
                catch(UnauthorizedAccessException)
                {
                    AnsiConsole.MarkupLine("[red]Acesso negado! Tente executar como administrador.[/]");
                    throw;
                }
                break;
            }
            else if (choice == "Voltar")
            {
                break;
            }
        }
    }
}
