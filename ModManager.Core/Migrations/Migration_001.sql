CREATE TABLE Games (
    Id TEXT NOT NULL,
    Name TEXT NOT NULL,
    GamePath TEXT NOT NULL,
    BackupPath TEXT NOT NULL,
    ModsPath TEXT NOT NULL,

    PRIMARY KEY (Id)
);

CREATE TABLE Mods (
    Id TEXT NOT NULL,
    Name TEXT NOT NULL,
    ModPath TEXT NOT NULL,
    IsEnable INTEGER NOT NULL,
    IsOverwritten INTEGER NOT NULL,
    GameId TEXT,

    PRIMARY KEY (Id),
    FOREIGN KEY (GameId) REFERENCES Games(Id)
);

CREATE TABLE Archives (
    Id TEXT NOT NULL,
    RelativePath TEXT UNIQUE NOT NULL,
    GameId TEXT,

    PRIMARY KEY (Id),
    FOREIGN KEY (GameId) REFERENCES Games(Id)
);

CREATE TABLE ArchiveMod (
    ArchiveId TEXT NOT NULL,
    ModId TEXT NOT NULL,

    PRIMARY KEY (ArchiveId, ModId),
    FOREIGN KEY (ArchiveId) REFERENCES Archives(Id),
    FOREIGN KEY (ModId) REFERENCES Mods(Id)
);

