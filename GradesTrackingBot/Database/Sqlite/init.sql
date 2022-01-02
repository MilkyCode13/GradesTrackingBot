CREATE TABLE Disciplines (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    UserId INTEGER NOT NULL,
    Target INTEGER
);

CREATE TABLE MarkElements (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    DisciplineId TEXT NOT NULL,
    Weight INTEGER NOT NULL,
    Grade REAL,
    FOREIGN KEY (DisciplineId) REFERENCES Disciplines(Id)
);
