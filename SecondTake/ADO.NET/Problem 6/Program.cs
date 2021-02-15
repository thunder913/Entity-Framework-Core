using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Problem_6
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");

            var villainId = int.Parse(Console.ReadLine());

            connection.Open();
            using (connection)
            {
                var getVillianName = new SqlCommand("SELECT Name FROM Villains WHERE Id = @id", connection);
                getVillianName.Parameters.AddWithValue("@id", villainId);
                var villainName = getVillianName.ExecuteScalar();
                if (villainName==null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }

                var getMinions = new SqlCommand("SELECT M.Id, M.Name FROM Minions M " +
                    "JOIN MinionsVillains MV ON MV.MinionId = M.Id " +
                    "WHERE MV.VillainId = @id", connection);
                getMinions.Parameters.AddWithValue("@id", villainId);

                var minions = new List<Minion>();
                var minionReader = getMinions.ExecuteReader();
                using (minionReader)
                {
                    while (minionReader.Read())
                    {
                        minions.Add(new Minion(minionReader[1].ToString(), int.Parse(minionReader[0].ToString())));
                    }
                }

                var deleteMiddleTable = new SqlCommand("DELETE FROM MinionsVillains " +
                    "WHERE VillainId = @id", connection);
                deleteMiddleTable.Parameters.AddWithValue("@id", villainId);
                deleteMiddleTable.ExecuteNonQuery();

                var deleteFromVillains = new SqlCommand("DELETE FROM Villains WHERE Id = @id", connection);
                deleteFromVillains.Parameters.AddWithValue("@id", villainId);
                deleteFromVillains.ExecuteNonQuery();
                Console.WriteLine($"{villainName} was deleted.");

                foreach (var item in minions)
                {
                    var deleteMinion = new SqlCommand("DELETE FROM Minions	WHERE Id=@id", connection);
                    deleteMinion.Parameters.AddWithValue("@id", item.Id);
                }
                Console.WriteLine($"{minions.Count()} minions were released.");


            }
        }
    }
}
