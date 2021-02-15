using Microsoft.Data.SqlClient;
using System;

namespace Problem4
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                var inputMinion = Console.ReadLine().Split();
                var nameMinion = inputMinion[1];
                var age = int.Parse(inputMinion[2]);
                var town = inputMinion[3];
                var inputVillain = Console.ReadLine().Split();
                var villainName = inputVillain[1];
                int townId = 0;
                SqlCommand checkIfTownIsThere = new SqlCommand("SELECT * FROM TOWNS WHERE Name = @TownName", dbCon);
                checkIfTownIsThere.Parameters.AddWithValue("@TownName", town);
                var reader = checkIfTownIsThere.ExecuteReader();

                //CHECK FOR TOWN
                using (reader)
                {
                    if (!reader.Read())
                    {
                        SqlCommand inputTown = new SqlCommand("INSERT INTO Towns(Name) VALUES (@townName)", dbCon);
                        inputTown.Parameters.AddWithValue("@TownName", town);
                        reader.Close();
                        inputTown.ExecuteNonQuery();
                        Console.WriteLine($"Town <{town}> was added to the database.");
                    }
                }

                reader = checkIfTownIsThere.ExecuteReader();
                using (reader)
                {
                    reader.Read();
                        townId = (int)reader["Id"];
                }


                SqlCommand checkIfVillaisIsHere = new SqlCommand("SELECT * FROM Villains WHERE Name = @Villain", dbCon);
                checkIfVillaisIsHere.Parameters.AddWithValue("@Villain", villainName);
                reader = checkIfVillaisIsHere.ExecuteReader();

                //CHECK FOR VILLAIN
                using (reader)
                {
                    if (!reader.Read())
                    {
                        SqlCommand inputVillainCommand = new SqlCommand("INSERT INTO Villains(Name,EvilnessFactorId) VALUES (@villainName, 4)", dbCon);
                        inputVillainCommand.Parameters.AddWithValue("@villainName", villainName);
                        reader.Close();
                        inputVillainCommand.ExecuteNonQuery();
                        Console.WriteLine($"Villain <{villainName}> was added to the database.");

                    }
                }

                //INSERT MINION
                SqlCommand inputMinionInDB = new SqlCommand("INSERT INTO Minions(Name,Age,TownId) VALUES (@name, @age, @townId) ", dbCon);
                inputMinionInDB.Parameters.AddWithValue("@name", nameMinion);
                inputMinionInDB.Parameters.AddWithValue("@age", age);
                inputMinionInDB.Parameters.AddWithValue("@townId", townId);
                inputMinionInDB.ExecuteNonQuery();

                SqlCommand getMinionId = new SqlCommand("SELECT Id FROM Minions WHERE Name = @name AND Age = @age",dbCon);
                getMinionId.Parameters.AddWithValue("@name", nameMinion);
                getMinionId.Parameters.AddWithValue("@age", age);
                
                SqlCommand getVillainId = new SqlCommand("SELECT Id FROM Villains WHERE Name = @name",dbCon);
                getVillainId.Parameters.AddWithValue("@name", villainName);
                int villainId = (int)getVillainId.ExecuteScalar();
                //int villainId = (int)getVillainId.ExecuteScalar();
                int minionId = (int)getMinionId.ExecuteScalar();
                
                
                SqlCommand inputIntoMinionsVillain = new SqlCommand("INSERT INTO MinionsVillains(MinionId,VillainId) VALUES (@minionId, @villainId)",dbCon);
                inputIntoMinionsVillain.Parameters.AddWithValue("@minionId", minionId);
                inputIntoMinionsVillain.Parameters.AddWithValue("@villainId", villainId);
                inputIntoMinionsVillain.ExecuteNonQuery();

                Console.WriteLine($"Successfully added <{nameMinion}> to be minion of <{villainName}>.");
            }
        }
    }
}
