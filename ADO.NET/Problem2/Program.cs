using Microsoft.Data.SqlClient;
using System;

namespace Problem2
{
    class Program
    {
        static void Main(string[] args)
        {

            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                SqlCommand command = new SqlCommand("SELECT V.Name + ' - ' + CONVERT(VARCHAR(5), COUNT(*)) FROM Villains V " +
                    "JOIN MinionsVillains MV ON MV.VillainId = V.Id " +
                    "GROUP BY MV.VillainId, V.Name " +
                    "HAVING COUNT(*) > 1 " + //THIS CAN BE 3, BUT I DONT HAVE ENOUGH ELEMENTS
                    "ORDER BY COUNT(*) DESC", dbCon);
                SqlDataReader reader= command.ExecuteReader();
                using (reader)
                    while (reader.Read())
                    {
                        var item = (string)reader[0];
                        Console.WriteLine(item);
                    }
            }
        }
    }
}
