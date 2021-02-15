using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Problem7
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            var names = new List<string>();
            dbCon.Open();
            using (dbCon)
            {
                var selectCommand = new SqlCommand("SELECT Name FROM MINIONS", dbCon);
                var reader = selectCommand.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        names.Add((string)reader["Name"]);
                    }
                }
            }

            for (int i = 0,j=names.Count()-1; i < (names.Count())/2; i++,j--)
            {
                Console.WriteLine(names[i]);
                Console.WriteLine(names[j]);
            }

            if (names.Count()%2!=0)
            {
                Console.WriteLine(names[(names.Count())/2]);
            }
        }
    }
}