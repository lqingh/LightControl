using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightControl
{
    public partial class Frm_Main : Form
    {

        int TootalPageNum = 0;  // 用来记录在"用户"中当前所在的页码数
        int onePageRowNum = 26;     // 设置一页显示的行数
        int ZongShu_Num = 0;        // 用来记录总的用户数
        int Message_num = 0;        // 用来记录当前的分类中消息条数
        int UserMessagePageNum = 0; // 用来记录在"任务"中当前所在的页码数
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public Frm_Main()
        {
            InitializeComponent();
        }
       public  string user = "";
        private void Frm_Main_Load(object sender, EventArgs e)
        {
            name.Text = user;
        }

        string strSql = "select * from tags limit @curPage,@pageSize";
      
        int i = 0;
        private void region_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum  * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(strSql, dt);
            MessageBox.Show(Convert.ToString(dt.Rows.Count));
            tabControl11.Visible = true;
            tabControl11.Dock = DockStyle.Fill;
            dataGridView1.Rows.Clear();
            for (i=0;i< dt.Rows.Count;i++) {
                dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum  * onePageRowNum+i+1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);

            }
            // dataGridView1.DataSource = dt;
            //  MessageBox.Show(Convert.ToString( dt.Rows.Count));
        }

        // 首页
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        // 尾页
        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            TootalPageNum = (ZongShu_Num % onePageRowNum == 0) ? (ZongShu_Num / onePageRowNum) - 1 : (ZongShu_Num / onePageRowNum);
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        // 上一页
        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
            }
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        // 下一页
        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < ((ZongShu_Num % onePageRowNum == 0) ? (ZongShu_Num / onePageRowNum) - 1 : (ZongShu_Num / onePageRowNum)))
            {
               TootalPageNum++;
            }
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox1.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox1.Text.ToString()) <= ((ZongShu_Num % onePageRowNum) == 0 ? (ZongShu_Num / onePageRowNum) : (ZongShu_Num / onePageRowNum)) + 1)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox1.Text.ToString()) - 1;
                    dataGridView1.Rows.Clear();
                   /* string ss1 = com.HttpGet(com.URL + "User/getData?userName=" + userName + "&departmentName=" + departmentName + "&pageSize=" + onePageRowNum + "&pageNumber=" + (UserTootalPageNum));
                    JObject jo1 = (JObject)JsonConvert.DeserializeObject(ss1);
                    JArray jar1 = JArray.Parse(jo1["result"].ToString());
                    toolStripLabel7.Text = "/共" + (Convert.ToInt32(jo1["message"]) % onePageRowNum == 0 ? (Convert.ToInt32(jo1["message"]) / onePageRowNum).ToString() : (Convert.ToInt32(jo1["message"]) / onePageRowNum + 1).ToString()) + "页";
                    ZongShu_Num = Convert.ToInt32(jo1["message"]);  // 记录当前页面的用户总数
                    int index = 0;
                    foreach (var item in jar1)
                    {
                        index = index + 1;
                        dataGridView8.Rows.Add(item["id"], index + onePageRowNum * UserTootalPageNum, item["userName"], item["realName"], item["departmentName"], item["userType"], item["superName"]);
                        if (index % 2 == 1)
                        {
                            dataGridView8.Rows[index - 1].DefaultCellStyle.BackColor = Color.FromArgb(209, 215, 231);
                        }
                        else if (index % 2 == 0)
                        {
                            dataGridView8.Rows[index - 1].DefaultCellStyle.BackColor = Color.FromArgb(233, 237, 246);
                        }
                    }*/
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Frm_Addloop_Manual am = new Frm_Addloop_Manual();
            am.StartPosition = FormStartPosition.CenterParent;
            am.ShowDialog();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Frm_Addloop_Manual am = new Frm_Addloop_Manual();
            am.StartPosition = FormStartPosition.CenterParent;
            am.ShowDialog();
        }
    }
}
