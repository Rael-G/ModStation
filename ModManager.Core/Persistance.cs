using System.Reflection;
using System.Xml.Serialization;
using ModManager.Core.Dtos;

namespace ModManager;

public class Persistance
{
    private static string _gamesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "games.xml");

    public static void Save(List<Game> games)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<GameDto>));
        using var file = File.Open(_gamesPath, FileMode.OpenOrCreate);
        xmlSerializer.Serialize(file, Mapper.MapGames(games));
    }

    public static List<Game> Load()
    {
        if (!File.Exists(_gamesPath))
        {
            return  [];
        }

        var xmlSerializer = new XmlSerializer(typeof(List<GameDto>));
        using var file = File.Open(_gamesPath, FileMode.OpenOrCreate);
        var xml = xmlSerializer.Deserialize(file);

        if (xml is List<GameDto> dtos)
        {
            return [.. Mapper.MapGames(dtos)];
        }

        return [];
    }
}
