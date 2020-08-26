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
    public partial class Frm_loopScene : Form
    {
        public Frm_loopScene()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public string eName = "";
        public int eId = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                dataGridView2.Rows.Add(dataGridView1.Rows[e.RowIndex].Cells[0].Value, dataGridView1.Rows[e.RowIndex].Cells[1].Value, dataGridView1.Rows[e.RowIndex].Cells[2].Value, dataGridView1.Rows[e.RowIndex].Cells[3].Value);
                this.dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows.Count > 1)
            {
                dataGridView1.Rows.Add(dataGridView2.Rows[e.RowIndex].Cells[0].Value, dataGridView2.Rows[e.RowIndex].Cells[1].Value, dataGridView2.Rows[e.RowIndex].Cells[2].Value, dataGridView2.Rows[e.RowIndex].Cells[3].Value);
                this.dataGridView2.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void Frm_loopScene_Load(object sender, EventArgs e)
        {
            label1.Text = eName;
            string s = "select t.id,t.`name`,t.note,e.`name` as eName from tags t join equipment e on t.equipment_id = e.id  where t.tt_id = 2 && t.id not in (select sts.tags_id from scene_tags sts join scene s on sts.scene_id = s.id where sts.scene_id = @id) ";
            string s1 = "select t.id,t.`name`,t.note,e.`name` from scene_tags st join tags t on st.tags_id = t.id join equipment e on t.equipment_id = e.id where st.scene_id = @id";
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            // dt.Dispose();
            TEST_DB.ExecuteSQL(s1, dt);

            int i = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
            }
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            TEST_DB.ExecuteSQL(s, dt);

 
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView2.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = "DELETE from scene_tags  WHERE scene_tags.scene_id = @id";
            TEST_DB.Add_Param("@id", eId);
            TEST_DB.ExecuteDML(s);
            string s1 = "insert into scene_tags(scene_id,tags_id) value(@scene_id,@tags_id)";
            int count = dataGridView1.Rows.Count - 1;
            int i;
            for (i = 0; i < count; i++)
            {
                TEST_DB.Add_Param("@scene_id", eId);
                TEST_DB.Add_Param("@tags_id", Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value));
                TEST_DB.ExecuteDML(s1);
            }
            MessageBox.Show("保存成功");
            DialogResult = DialogResult.OK;
        }
    }
}
