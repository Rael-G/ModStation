using System;

namespace ModManager.Core.Dtos;

public class GameDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string GamePath { get; set; }

    public string BackupPath { get; set; }

    public string ModsPath { get; set; }

    public List<ModDto> Mods { get; set; } = [];
}
