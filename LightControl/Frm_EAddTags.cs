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
    public partial class Frm_EAddTags : Form
    {
        public Frm_EAddTags()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public string eName = "";
        public int eId = 0;
        string setstring = "select t.id,t.`name`,t.note from equipment e   join tags t on e.id = t.equipment_id where e.id = @id";
        string s = "select t.id,t.`name`,t.note from  tags t  where t.tt_id=@tt_id  && t.equipment_id is null";
        string s1 = "select e.et_id from equipment e WHERE e.id = @id";
        private void Frm_EAddTags_Load(object sender, EventArgs e)
        {
            label1.Text = eName;
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            // dt.Dispose();
            TEST_DB.ExecuteSQL(setstring, dt);
            
            int i = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2]);
            }
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.Add_Param("@id", eId);
            // dt.Dispose();
            TEST_DB.ExecuteSQL(s1, dt);
            int et_id =Convert.ToInt32( dt.Rows[0][0]);
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.Add_Param("@tt_id", et_id);
            TEST_DB.ExecuteSQL(s, dt);
          
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView2.Rows.Add(dt.Rows[i][0], dt.Rows[i][1], dt.Rows[i][2]);
            }
            dt.Dispose();
        }
        string uts = "UPDATE tags set equipment_id = @equipment_id WHERE id = @id";
        string dts = "UPDATE tags set equipment_id = null WHERE equipment_id = @equipment_id";
        private void button2_Click(object sender, EventArgs e)
        {
            int i = 0;
            //  MessageBox.Show(dataGridView2.Rows.Count+"");
           
            TEST_DB.Add_Param("@equipment_id", eId);
            TEST_DB.ExecuteDML(dts);
            int count = dataGridView1.Rows.Count-1;
            for (i=0;i<count;i++) {
                TEST_DB.Add_Param("@equipment_id", eId);
                TEST_DB.Add_Param("@id",Convert.ToInt32( dataGridView1.Rows[i].Cells[0].Value));
                TEST_DB.ExecuteDML(uts);
            }
            DialogResult = DialogResult.OK;
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
            if (dataGridView2.Rows.Count>1) {
                dataGridView1.Rows.Add(dataGridView2.Rows[e.RowIndex].Cells[0].Value, dataGridView2.Rows[e.RowIndex].Cells[1].Value, dataGridView2.Rows[e.RowIndex].Cells[2].Value);
                this.dataGridView2.Rows.RemoveAt(e.RowIndex);
            }
            
        }
    }
}
