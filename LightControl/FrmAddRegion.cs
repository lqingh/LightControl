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
    public partial class FrmAddRegion : Form
    {
        public FrmAddRegion()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public string name;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        int parent_id = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            string s = "insert into equipment(name,parent_id) value (@name,@parent_id)";
            string s1 = "select id from equipment where name =@name";
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("请填写节点名称！");
                return;
            }
            else
            {
                if (name != "根节点.")
                {
                    DataTable dt = new DataTable();
                    // MessageBox.Show(cnodeName);
                    TEST_DB.Add_Param("@name", name);
                    TEST_DB.ExecuteSQL(s1, dt);
                    //  MessageBox.Show(Convert.ToString( dt.Rows.Count));
                    if (dt.Rows.Count > 0)
                    {
                        parent_id = Convert.ToInt32(dt.Rows[0][0]);
                    }
                    dt.Dispose();
                }
                name = textBox1.Text.Trim() + ".";
                TEST_DB.Add_Param("@name", textBox1.Text.Trim()+".");
            }
            if (this.parent_id == 0)
            {
                TEST_DB.Add_Param("@parent_id", null);
            }
            else
            {
                TEST_DB.Add_Param("@parent_id", parent_id);
            }
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
    }
}
