USE MinionsDB

CREATE TABLE Countries
(
	Id INT PRIMARY KEY IDENTITY,
	Name varchar(30) 
)

CREATE TABLE Towns
(
	Id INT PRIMARY KEY IDENTITY,
	Name varchar(30),
	ContryCode INT FOREIGN KEY REFERENCES Countries(Id)
)

CREATE TABLE Minions
(
	Id INT PRIMARY KEY IDENTITY,
	Name varchar(30),
	Age INT,
	TownId INT FOREIGN KEY REFERENCES Towns(Id)
)

CREATE TABLE EvilnessFactors
(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(30)
)

CREATE TABLE Villains
(
	Id INT PRIMARY KEY IDENTITY,
	Name varchar(30),
	EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id)
)

CREATE TABLE MinionsVillains
(
	MinionId INT FOREIGN KEY REFERENCES Minions(Id),
	VillainId INT FOREIGN KEY REFERENCES Villains(Id)
	PRIMARY KEY(MinionId,VillainId)
)


INSERT INTO Countries(Name)
VALUES
('Bulgaria'), 
('Greece'),
('Japan'), 
('China'), 
('USA')

INSERT INTO Towns(Name,ContryCode)
VALUES
('Sandanski',1),
('Sofia',1),
('Athens',2),
('Beijin',4),
('New York',5),
('California',5)

INSERT INTO Minions(Name,Age,TownId)
VALUES
('Ivan', 15, 1),
('Pesho', 15, 2),
('Georgi', 15, 3),
('Andon', 15, 4),
('Niki', 15, 5),
('Georgi', 15, 2),
('Georgios', 15, 2)

INSERT INTO EvilnessFactors(Name)
VALUES
('super good'),
('good'),
('bad'),
('evil'),
('super evil')

INSERT INTO Villains(Name,EvilnessFactorId)
VALUES
('John',1),
('Ivailo',2),
('Svetlin',3),
('Tomi',4),
('Doni',5)

INSERT INTO MinionsVillains(MinionId,VillainId)
VALUES
(1,2),
(2,3),
(4,4),
(3,5),
(5,1),
(1,3),
(1,5),
(3,2)

SELECT * FROM MinionsVillains

--2

SELECT V.Name + ' - ' + CONVERT(VARCHAR(5),COUNT(*)) FROM Villains V
	JOIN MinionsVillains MV ON MV.VillainId = V.Id
	GROUP BY MV.VillainId, V.Name
	HAVING COUNT(*) > 1
	ORDER BY COUNT(*) DESC
	
SELECT V.Name AS Villain,M.Name as MinionName,M.Age as Age FROM Minions M
	JOIN MinionsVillains MV ON MV.MinionId = M.Id
	JOIN Villains V ON MV.VillainId = V.Id
	WHERE V.Id = 2
	ORDER BY M.Name
GO

CREATE OR ALTER PROC usp_GetOlder(@MinionId int)
AS
BEGIN
UPDATE Minions
	SET Age +=1
	WHERE Id = @MinionId
	SELECT Name + ' - ' + CONVERT(VARCHAR(10),AGE) + ' years old' as Output FROM Minions
		WHERE Id= @MinionId
END

EXEC usp_GetOlder 4