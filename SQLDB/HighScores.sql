CREATE TABLE ICanRead.[HighScore]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(100) NULL,
	[Score] INT NOT NULL,
	[Date] DATETIME NOT NULL Default (sysdatetimeoffset())
)
GO
Create index IX_ByScoreAndName on ICanRead.HighScore(Score, Name)
GO
Create index IX_ByDateScoreAndName on ICanRead.HighScore(Date, Score, Name)
