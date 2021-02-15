using P01_HospitalDatabase.Data;
using System;

namespace P01_HospitalDatabase
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var context = new HospitalContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
