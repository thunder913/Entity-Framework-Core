using P03_FootballBetting.Data;
using System.Linq;

namespace P03_FootballBetting
{
    class StartUp
    {
        static void Main(string[] args)
        {

            //TODO FOREIGN KEY AND ONE TO MANY RELATIONS(CURRENTLY NOT WORKING)!!
            var db = new FootballBettingContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

        }
    }
}
