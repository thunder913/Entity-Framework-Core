using Microsoft.Data.SqlClient;
using System;

namespace Problem9
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            var id = int.Parse(Console.ReadLine());
            dbCon.Open();
            using (dbCon)
            {
                var command = new SqlCommand($"EXEC usp_GetOlder {id}", dbCon);
                var output = (string)command.ExecuteScalar();
                Console.WriteLine(output);
            }
        }
    }
}
