using Microsoft.Data.SqlClient;
using System;

namespace Problem1
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; "+ "Database=master;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                SqlCommand command = new SqlCommand("CREATE DATABASE MinionsDB ", dbCon);
                
                command.ExecuteNonQuery();
                
                Console.WriteLine("done1");
            }
            dbCon = new SqlConnection("Server=.; " + "Database=MinionsDB;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                SqlCommand CreateTables = new SqlCommand("CREATE TABLE Countries(Id INT PRIMARY KEY IDENTITY, Name varchar(30))" +
                "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY, Name varchar(30), ContryCode INT FOREIGN KEY REFERENCES Countries(Id))" +
                "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY, Name varchar(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))" +
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(30))" +
                "CREATE TABLE Villains(Id INT PRIMARY KEY IDENTITY, Name varchar(30), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))" +
                "CREATE TABLE MinionsVillains(MinionId INT FOREIGN KEY REFERENCES Minions(Id), VillainId INT FOREIGN KEY REFERENCES Villains(Id) PRIMARY KEY(MinionId, VillainId))" +
                "INSERT INTO Countries(Name) VALUES('Bulgaria'), ('Greece'), ('Japan'), ('China'), ('USA')" +
                "INSERT INTO Towns(Name, ContryCode) VALUES('Sandanski', 1), ('Sofia', 1), ('Athens', 2), ('Beijin', 4), ('New York', 5), ('California', 5)" +
                "INSERT INTO Minions(Name, Age, TownId) VALUES('Ivan', 15, 1), ('Pesho', 15, 2), ('Georgi', 15, 3), ('Andon', 15, 4), ('Niki', 15, 5), ('Georgi', 15, 2), ('Georgios', 15, 2)" +
                "INSERT INTO EvilnessFactors(Name) VALUES('super good'), ('good'), ('bad'), ('evil'), ('super evil')" +
                "INSERT INTO Villains(Name, EvilnessFactorId) VALUES('John', 1), ('Ivailo', 2), ('Svetlin', 3), ('Tomi', 4), ('Doni', 5)" +
                "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(1, 2), (2, 3), (4, 4), (3, 5), (5, 1), (1, 3), (1, 5), (3, 2)", dbCon);
                CreateTables.ExecuteNonQuery();
                Console.WriteLine("done2");
            }

        }
    }
}
