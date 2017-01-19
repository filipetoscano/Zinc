using Platinum.Data;

namespace Zinc.WebServices.Journaling
{
    public class Q
    {
        /// <summary />
        public static string SqlFull
        {
            get { return Db.Command( "Journaling/SqlFull" ); }
        }


        /// <summary />
        public static string SqlPost
        {
            get { return Db.Command( "Journaling/SqlPost" ); }
        }


        /// <summary />
        public static string SqlPre
        {
            get { return Db.Command( "Journaling/SqlPre" ); }
        }
    }
}
