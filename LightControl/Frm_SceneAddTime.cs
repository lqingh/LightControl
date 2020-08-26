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
    public partial class Frm_SceneAddTime : Form
    {
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public string eName = "";
        public int eId = 0;
        public Frm_SceneAddTime()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                dataGridView2.Rows.Add(dataGridView1.Rows[e.RowIndex].Cells[0].Value, dataGridView1.Rows[e.RowIndex].Cells[1].Value, dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                this.dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows.Count > 1)
            {
                dataGridView1.Rows.Add(dataGridView2.Rows[e.RowIndex].Cells[0].Value, dataGridView2.Rows[e.RowIndex].Cells[1].Value, dataGridView2.Rows[e.RowIndex].Cells[2].Value);
                this.dataGridView2.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = "DELETE from scene_epoint  WHERE scene_epoint.scene_id = @id";
            TEST_DB.Add_Param("@id", eId);
            TEST_DB.ExecuteDML(s);
            string s1 = "insert into scene_epoint(scene_id,epoint_id) value(@scene_id,@epoint_id)";
            int count = dataGridView1.Rows.Count - 1;
            int i;
            for (i = 0; i < count; i++)
            {
                TEST_DB.Add_Param("@scene_id", eId);
                TEST_DB.Add_Param("@epoint_id", Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value));
                TEST_DB.ExecuteDML(s1);
            }
            MessageBox.Show("保存成功");
            DialogResult = DialogResult.OK;
        }

        private void Frm_SceneAddTime_Load(object sender, EventArgs e)
        {
            label1.Text = eName;
            string s = "select e.id,e.`name`,e.note from epoint e where  e.id not in (select ep.id from scene_epoint ses join epoint ep on ses.scene_id = ep.id where ses.scene_id = @id) ";
            string s1 = "select e.id,e.`name`,e.note from scene_epoint se join scene s on se.scene_id = s.id join epoint e on se.epoint_id = e.id where se.scene_id = @id";
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            // dt.Dispose();
            TEST_DB.ExecuteSQL(s1, dt);

            int i = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2]);
            }
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            TEST_DB.ExecuteSQL(s, dt);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView2.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2]);
            }
        }
    }
}
