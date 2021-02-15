using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Problem_7
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();
            var minionNames = new List<string>();
            using (connection)
            {
                var getMinions = new SqlCommand("SELECT Name FROM Minions", connection);
                var reader = getMinions.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        minionNames.Add(reader[0].ToString());
                    }
                }
            }

            for (int i = 1; i <= minionNames.Count()/2; i++)
            {
                Console.WriteLine(minionNames[i-1]);
                Console.WriteLine(minionNames[^i]);
            }
            if (minionNames.Count()%2==1)
            {
                Console.WriteLine(minionNames[minionNames.Count()/2]);
            }
        }
    }
}
