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
    public partial class Frm_addHoliday : Form
    {
        public Frm_addHoliday()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public int id = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        string ih = "insert into holidays(sDate,eDate,note) value(@sDate,@eDate,@note)";
        private void button2_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                TEST_DB.Add_Param("@sDate", dateTimePicker1.Value);
                TEST_DB.Add_Param("@eDate", dateTimePicker2.Value);
                TEST_DB.Add_Param("@note", textBox1.Text);
                if (TEST_DB.ExecuteDML(ih) > 0)
                {
                    MessageBox.Show("添加成功");
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("添加失败");
                }

            }
            else {
                string s = "UPDATE holidays set sDate = @sDate,eDate = @eDate,note = @note WHERE id = @id";
                TEST_DB.Add_Param("@sDate", dateTimePicker1.Value);
                TEST_DB.Add_Param("@eDate", dateTimePicker2.Value);
                TEST_DB.Add_Param("@note", textBox1.Text);
                    TEST_DB.Add_Param("@id",id);
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


        }

        private void Frm_addHoliday_Load(object sender, EventArgs e)
        {
           
            if (id>0) {
                string s = "select * from holidays where id = @id";
                TEST_DB.Add_Param("@id", id);
                DataTable dt = new DataTable();
                TEST_DB.ExecuteSQL(s,dt);
                if (dt.Rows.Count>0) {
                    dateTimePicker1.Value =Convert.ToDateTime( dt.Rows[0][1]);
                    dateTimePicker2.Value = Convert.ToDateTime(dt.Rows[0][2]);
                    textBox1.Text = Convert.ToString(dt.Rows[0][3]);
                }
            }
        }
    }
}
