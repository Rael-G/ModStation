CREATE TABLE Games_New (
    Id TEXT NOT NULL,
    Name TEXT UNIQUE NOT NULL,
    GamePath TEXT NOT NULL,
    BackupPath TEXT NOT NULL,
    ModsPath TEXT NOT NULL,

    PRIMARY KEY (Id)
);

CREATE TABLE Mods_New (
    Id TEXT NOT NULL,
    Name TEXT UNIQUE NOT NULL,
    ModPath TEXT NOT NULL,
    IsEnable INTEGER NOT NULL,
    GameId TEXT,
    "Order" INTEGER,

    PRIMARY KEY (Id),
    FOREIGN KEY (GameId) REFERENCES Games_New(Id)
);

CREATE TABLE Archives_New (
    Id TEXT NOT NULL,
    RelativePath TEXT UNIQUE NOT NULL,
    GameId TEXT,

    PRIMARY KEY (Id),
    FOREIGN KEY (GameId) REFERENCES Games_New(Id)
);

CREATE TABLE ArchiveMod_New (
    ArchiveId TEXT NOT NULL,
    ModId TEXT NOT NULL,

    PRIMARY KEY (ArchiveId, ModId),
    FOREIGN KEY (ArchiveId) REFERENCES Archives_New(Id),
    FOREIGN KEY (ModId) REFERENCES Mods_New(Id)
);

INSERT INTO Games_New (Id, Name, GamePath, BackupPath, ModsPath)
SELECT Id, Name, GamePath, BackupPath, ModsPath FROM Games;

INSERT INTO Mods_New (Id, Name, ModPath, IsEnable, GameId, "Order")
SELECT Id, Name, ModPath, IsEnable, GameId, "Order" FROM Mods;

INSERT INTO Archives_New (Id, RelativePath, GameId)
SELECT Id, RelativePath, GameId FROM Archives;

INSERT INTO ArchiveMod_New (ArchiveId, ModId)
SELECT ArchiveId, ModId FROM ArchiveMod;

DROP TABLE ArchiveMod;

DROP TABLE Archives;

DROP TABLE Mods;

DROP TABLE Games;

ALTER TABLE ArchiveMod_New RENAME TO ArchiveMod;

ALTER TABLE Archives_New RENAME TO Archives;

ALTER TABLE Mods_New RENAME TO Mods;

ALTER TABLE Games_New RENAME TO Games;
