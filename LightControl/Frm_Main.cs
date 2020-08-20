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
        int loopTootal = 0;       // 记录回路的总数
        int TootalPageNum = 0;  // 用来记录在"用户"中当前所在的页码数
        int onePageRowNum = 2;     // 设置一页显示的行数

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

        string strSql = "select t.id,t.name,t.note,e.`name` as eName from tags t left join equipment e on t.equipment_id = e.id limit @curPage,@pageSize";
      
        int i = 0;
        private void region_Click(object sender, EventArgs e)
        {
            MessageBox.Show(TootalPageNum * onePageRowNum + "");
            MessageBox.Show(onePageRowNum + "");

            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum  * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(strSql, dt);
            tabControl11.Visible = true;
            tabControl11.Dock = DockStyle.Fill;
            dataGridView1.Rows.Clear();
            for (i=0;i< dt.Rows.Count;i++) {
                dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum  * onePageRowNum+i+1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
            }
            strSql = "select count(*) from tags";
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.ExecuteSQL(strSql, dt);
            loopTootal = Convert.ToInt32(dt.Rows[0][0]);
            dt.Dispose();
            toolStripLabel7.Text = "/共" + (loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum).ToString() : (loopTootal / onePageRowNum + 1).ToString()) + "页";
         //   toolStripTextBox1.Text = "1";
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
            TootalPageNum = (loopTootal % onePageRowNum == 0) ? (loopTootal / onePageRowNum) - 1 : (loopTootal / onePageRowNum);
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
            if (TootalPageNum < ((loopTootal % onePageRowNum == 0) ? (loopTootal / onePageRowNum) - 1 : (loopTootal / onePageRowNum)))
            {
               TootalPageNum++;
            }
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox1.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox1.Text.ToString()) <= ((loopTootal % onePageRowNum) == 0 ? (loopTootal / onePageRowNum) : (loopTootal / onePageRowNum)) + 1)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox1.Text.ToString()) - 1;
                    dataGridView1.Rows.Clear();

                    MessageBox.Show(TootalPageNum * onePageRowNum + "");
                    MessageBox.Show(onePageRowNum + "");

                    DataTable adt = new DataTable();
                    TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
                    TEST_DB.Add_Param("@pageSize", onePageRowNum);
                    TEST_DB.ExecuteSQL(strSql, adt);
                    tabControl11.Visible = true;
                    tabControl11.Dock = DockStyle.Fill;
                    dataGridView1.Rows.Clear();
                    for (i = 0; i < adt.Rows.Count; i++)
                    {
                        dataGridView1.Rows.Add(adt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, adt.Rows[i][1], adt.Rows[i][2], adt.Rows[i][3]);
                    }
                    strSql = "select count(*) from tags";
                    adt.Dispose();
                    adt = new DataTable();
                    TEST_DB.ExecuteSQL(strSql, adt);
                //    loopTootal = Convert.ToInt32(adt.Rows[0][0]);
                    adt.Dispose();
                   // MessageBox.Show(Convert.ToString(loopTootal));
                   // toolStripLabel7.Text = "/共" + (loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum).ToString() : (loopTootal / onePageRowNum + 1).ToString()) + "页";
                   // toolStripTextBox1.Text = "1";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Frm_Addloop_Manual am = new Frm_Addloop_Manual();
            am.StartPosition = FormStartPosition.CenterParent;
            am.ShowDialog();
        }
    }
}
