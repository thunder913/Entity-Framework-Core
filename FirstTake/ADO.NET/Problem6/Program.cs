using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Problem6
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            var id = int.Parse(Console.ReadLine());
            dbCon.Open();

            int minionCount = 0;

            var listDeleted = new List<string>();

            using (dbCon)
            {
                var selectResults = new SqlCommand("SELECT * FROM Villains WHERE Id = @Id", dbCon);
                selectResults.Parameters.AddWithValue("@Id", id);
                var reader = selectResults.ExecuteReader();
                bool hasAny = false;
                using (reader)
                {
                    while (reader.Read())
                    {
                        hasAny = true;
                        listDeleted.Add((string)reader["Name"]);
                    }
                }

                if (hasAny == false)
                {
                    Console.WriteLine($"No such villain was found.");
                    return;
                }
                var getMinionsCount = new SqlCommand("SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = @Id");
                     getMinionsCount.Parameters.AddWithValue("@Id", id);
                getMinionsCount.Connection = dbCon;
                minionCount = (int)getMinionsCount.ExecuteScalar();

                var deleteFromMinionsVillains = new SqlCommand("DELETE FROM MinionsVillains " +
                                                                      "WHERE VillainId = @Id", dbCon);
                deleteFromMinionsVillains.Parameters.AddWithValue("@Id", id);
                deleteFromMinionsVillains.ExecuteNonQuery();

                var deleteFromVillains = new SqlCommand("DELETE FROM Villains " +
                                                             "WHERE Id = @Id", dbCon);
                     deleteFromVillains.Parameters.AddWithValue("@Id", id);
                deleteFromVillains.ExecuteNonQuery();

                
            }

            foreach (var item in listDeleted)
            {
                Console.WriteLine($"{item} was deleted.");
            }
            Console.WriteLine($"{minionCount} minions were released.");
        }
    }
}
