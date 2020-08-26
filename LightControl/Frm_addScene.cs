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
    public partial class Frm_addScene : Form
    {
        public Frm_addScene()
        {
            InitializeComponent();
        }
        public int id = 0;
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length ==0)
            {
                MessageBox.Show("场景名称不能为空");
            }
            else {
                if (id == 0)
                {
                    string s = "insert into scene(name,enablement,note) value(@name,@enablement,@note)";
                    TEST_DB.Add_Param("@name", textBox1.Text);
                    TEST_DB.Add_Param("@note", textBox2.Text);
                    if (checkBox1.Checked)
                    {
                        TEST_DB.Add_Param("@enablement", 1);
                    }
                    else
                    {
                        TEST_DB.Add_Param("@enablement", 0);
                    }
                    if (TEST_DB.ExecuteDML(s) > 0)
                    {
                        MessageBox.Show("添加场景成功");
                        DialogResult = DialogResult.OK;
                    }

                }
                else
                {
                    string s = "UPDATE scene set name = @name,enablement = @enablement,note = @note WHERE id = @id";
                    TEST_DB.Add_Param("@name", textBox1.Text);
                    TEST_DB.Add_Param("@note", textBox2.Text);
                    TEST_DB.Add_Param("@id", id);
                    if (checkBox1.Checked)
                    {
                        TEST_DB.Add_Param("@enablement", 1);
                    }
                    else
                    {
                        TEST_DB.Add_Param("@enablement", 0);
                    }
                    if (TEST_DB.ExecuteDML(s) > 0)
                    {
                        MessageBox.Show("修改成功");
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("修改失败");
                    }

                }

                // MessageBox.Show(checkBox1.Checked+"");
                //  DialogResult = DialogResult.OK;
            }
            
        }

        private void Frm_addScene_Load(object sender, EventArgs e)
        {
            if (id > 0)
            {
                string s = "select * from scene where id = @id";
                TEST_DB.Add_Param("@id", id);
                DataTable dt = new DataTable();
                TEST_DB.ExecuteSQL(s, dt);
                if (dt.Rows.Count > 0)
                {
                    textBox1.Text = Convert.ToString(dt.Rows[0][1]);
                    textBox2.Text = Convert.ToString(dt.Rows[0][3]);
                    checkBox1.Checked = Convert.ToInt32(dt.Rows[0][2])==1 ? true : false;
                }
            }
        }
    }
}
