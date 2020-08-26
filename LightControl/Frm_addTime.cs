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
    public partial class Frm_addTime : Form
    {
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public Frm_addTime()
        {
            InitializeComponent();
        }
        int rday = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rday==0) {
                MessageBox.Show("重复日期至少要选择一天");
                return;
            }
            MessageBox.Show(rday + "");
            if (textBox1.Text.Length == 0) {
                MessageBox.Show("名称不能为空");
                return;
            }
            string s = "insert into epoint(s_time,e_time,rday,note,name,holiday_visiable) value(@s_time,@e_time,@rday,@note,@name,@holiday_visiable)";
            TEST_DB.Add_Param("@s_time", dateTimePicker1.Value);
            TEST_DB.Add_Param("@e_time", dateTimePicker2.Value);
            TEST_DB.Add_Param("@rday", rday);
            TEST_DB.Add_Param("@note", textBox2.Text);
            TEST_DB.Add_Param("@name", textBox1.Text);
            if (checkBox1.Checked)
            {
                TEST_DB.Add_Param("@holiday_visiable", 1);
            }
            else
            {
                TEST_DB.Add_Param("@holiday_visiable", 0);
            }
            if (TEST_DB.ExecuteDML(s) > 0)
            {
                MessageBox.Show("添加时控成功");
                DialogResult = DialogResult.OK;
            }
            DialogResult = DialogResult.OK;
        }
        private void Repeat_day() {
            rday = checkBox3.Checked ? rday | 1 : rday & ~1;
            rday = checkBox4.Checked ? rday | 2 : rday & ~2;
            rday = checkBox5.Checked ? rday | 4 : rday & ~4;
            rday = checkBox6.Checked ? rday | 8 : rday & ~8;
            rday = checkBox7.Checked ? rday | 16 : rday & ~16;
            rday = checkBox8.Checked ? rday | 32 : rday & ~32;
            rday = checkBox9.Checked ? rday | 64 : rday & ~64;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox3.Checked = checkBox2.Checked;
            checkBox4.Checked = checkBox2.Checked;
            checkBox5.Checked = checkBox2.Checked;
            checkBox6.Checked = checkBox2.Checked;
            checkBox7.Checked = checkBox2.Checked;
            checkBox8.Checked = checkBox2.Checked;
            checkBox9.Checked = checkBox2.Checked;
            Repeat_day();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox3.Checked ? rday | 1 : rday & ~1;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox4.Checked ? rday | 2 : rday & ~2;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox5.Checked ? rday | 4 : rday & ~4;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox6.Checked ? rday | 8 : rday & ~8;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox7.Checked ? rday | 16 : rday & ~16;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox8.Checked ? rday | 32 : rday & ~32;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            rday = checkBox9.Checked ? rday | 64 : rday & ~64;
        }

        private void Frm_addTime_Load(object sender, EventArgs e)
        {
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker2.ShowUpDown = true;
        }
    }
}
