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
    public partial class Frm_Addloop_Manual : Form
    {
        string tagName = "";        // 标签名
        string remarksText = "";    // 备注信息

        public Frm_Addloop_Manual()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) {
                MessageBox.Show("回路名称不能为空");
                return;
            }
            string s = "insert into tags(name,note,tt_id) VALUE (@name,@note,@tt_id)";
            TEST_DB.Add_Param("@name", textBox1.Text);
            TEST_DB.Add_Param("@note", textBox2.Text);
            TEST_DB.Add_Param("@tt_id",comboBox1.SelectedIndex+1);
            if (TEST_DB.ExecuteDML(s) > 0)
            {
                MessageBox.Show("添加成功");
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("添加失败");
            }
        }

        private void Frm_Addloop_Manual_Load(object sender, EventArgs e)
        {
            string s = "select * from tags_type";
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "ID";
            List<listItem>  list = new List<listItem>();
            int i = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                listItem li = new listItem();
                li.ID = Convert.ToInt32(dt.Rows[i][0]);
                li.Name = Convert.ToString(dt.Rows[i][1]);
                list.Add(li);
            }
            dt.Dispose();
            comboBox1.DataSource = list;
        }
    }
}
