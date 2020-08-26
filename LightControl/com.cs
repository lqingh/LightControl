using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl
{
    class com
    {
       public static DBConn.DB_SQL  TEST_DB = new DBConn.DB_SQL();
       public static Boolean updateTime = true;
    }
    public class listItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
