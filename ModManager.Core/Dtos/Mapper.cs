namespace ModManager.Core.Dtos;

public static class Mapper
{
    public static ModDto MapMod(Mod mod)
    {
        return new()
        {
            Id = mod.Id,
            Name = mod.Name,
            ModPath = mod.ModPath,
            IsEnable = mod.IsEnable,
            OverwrittenFiles = mod.OverwrittenFiles,
            GameId = mod.Game.Id
        };
    }

    public static Mod MapMod(ModDto mod, Game game)
    {
        return new(mod.Id, mod.Name, mod.ModPath, game, mod.IsEnable, mod.OverwrittenFiles);
    }

    public static IEnumerable<ModDto> MapMods(IEnumerable<Mod> mods)
    {
        var dtos = new List<ModDto>();
        foreach (var mod in mods)
        {
            dtos.Add(MapMod(mod));
        }

        return dtos;
    }

    public static IEnumerable<Mod> MapMods(IEnumerable<ModDto> dtos, Game game)
    {
        var mods = new List<Mod>();
        foreach (var dto in dtos)
        {
            mods.Add(MapMod(dto, game));
        }

        return mods;
    }

    public static GameDto MapGame(Game game)
    {
        return new()
        {
            Name = game.Name,
            GamePath = game.GamePath,
            BackupPath = game.BackupPath,
            ModsPath = game.ModsPath,
            Mods = [.. MapMods(game.Mods)]
        };
    }

    public static Game MapGame(GameDto dto)
    {
        var game = new Game(dto.Id, dto.Name, dto.GamePath, dto.BackupPath, dto.ModsPath);
        game.Mods = [.. MapMods(dto.Mods, game)];
        return game;
    }

    public static IEnumerable<GameDto> MapGames(IEnumerable<Game> games)
    {
        var dtos = new List<GameDto>();
        foreach (var game in games)
        {
            dtos.Add(MapGame(game));
        }

        return dtos;
    }

    public static IEnumerable<Game> MapGames(IEnumerable<GameDto> dtos)
    {
        var games = new List<Game>();
        foreach (var game in dtos)
        {
            games.Add(MapGame(game));
        }

        return games;
    }

}
