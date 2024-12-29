namespace ModManager.Core.Dtos;

public class ModDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ModPath { get; set; }

    public bool IsEnable { get; set; } = false;

    public List<string> OverwrittenFiles { get; set; } = [];

    public Guid GameId { get; set; }
}
