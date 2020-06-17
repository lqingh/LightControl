using dvDispatchServerForRdbAndHtrLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightControl
{
    class isee
    {
        string ip = "";
        string user = "";
        string password = "";
        string appName = "LightControl";
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        string strSql = "select * from isee";
        DataTable dt = new DataTable();
        public void ModifyOneTag()
        {
            dvRdbDispatchServer t = new dvRdbDispatchServer();
            int s = t.Connect(ip, 0, user, password, appName);
            t.ModifyTagFieldValue("test3.F_CV", 12);
                  
        }
        public void ModifyTags()
        {
            dvRdbDispatchServer t = new dvRdbDispatchServer();
            int s = t.Connect("127.0.0.1", 0, "Admin", "Admin", appName);
            Console.WriteLine(Convert.ToString(s));
            t.ModifyTagFieldValue("test3.F_CV", 12);
            
           
        }
        public void getIseeData() {
            TEST_DB.ExecuteSQL(strSql, dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("请先设置isee的ip地址,用户名,密码");
            }
            else {
                ip = dt.Rows[0]["ip"].ToString();
                user = dt.Rows[0]["user"].ToString();
                password = dt.Rows[0]["password"].ToString();
            }
        }
    }
}
