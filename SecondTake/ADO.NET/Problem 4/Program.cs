using System;
using System.Data.SqlClient;

namespace Problem_4
{
    class Program
    {
        static void Main(string[] args)
        {
            var minionAndTown = Console.ReadLine().Split();
            var villainName = Console.ReadLine().Split()[^1];
            var minionName = minionAndTown[1];
            var minionAge = int.Parse(minionAndTown[2]);
            var town = minionAndTown[^1];

            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();

            using (connection)
            {
                var getTownId = new SqlCommand("SELECT Id FROM Towns " +
                                            "WHERE Name = @name", connection);
                getTownId.Parameters.AddWithValue("@name",town);
                var townId = getTownId.ExecuteScalar();
                if (townId==null)
                {
                    var insertCommand = new SqlCommand("INSERT INTO Towns(Name) VALUES (@townName)", connection);
                    insertCommand.Parameters.AddWithValue("@townName", town);
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"Town {town} was added to the database.");
                    townId = getTownId.ExecuteScalar();
                }

                var getVillainId = new SqlCommand("SELECT Id FROM Villains	WHERE Name=@name", connection);
                getVillainId.Parameters.AddWithValue("@name", villainName);
                var villainId = getVillainId.ExecuteScalar();
                if (villainId==null)
                {
                    var insertVillain = new SqlCommand("INSERT INTO Villains(Name, EvilnessFactorId) VALUES(@name, 4)", connection);
                    insertVillain.Parameters.AddWithValue("@name", villainName);
                    insertVillain.ExecuteNonQuery();
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                    villainId = getVillainId.ExecuteScalar();
                }

                var insertMinion = new SqlCommand("INSERT INTO Minions(Name,Age,TownId) VALUES (@name, @age, @townId)", connection);
                insertMinion.Parameters.AddWithValue("@name", minionName);
                insertMinion.Parameters.AddWithValue("@age", minionAge);
                insertMinion.Parameters.AddWithValue("@townId", townId);
                insertMinion.ExecuteNonQuery();

                var getMinionId = new SqlCommand("SELECT Id FROM Minions WHERE Name=@name", connection);
                getMinionId.Parameters.AddWithValue("@name", minionName);
                var minionId = getMinionId.ExecuteScalar();

                var insertMinionVillain = new SqlCommand("INSERT INTO MinionsVillains(MinionId,VillainId) VALUES(@minionId, @villainId)", connection);
                insertMinionVillain.Parameters.AddWithValue("@minionId", minionId);
                insertMinionVillain.Parameters.AddWithValue("@villainId", villainId);

                insertMinionVillain.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

            }
        }
    }
}
