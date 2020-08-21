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
        string nodeName = "";
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public Frm_Main()
        {
            InitializeComponent();
        }
        public string user = "";
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
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(strSql, dt);
            MessageBox.Show(Convert.ToString(dt.Rows.Count));
            manage();
            tabControl11.Visible = true;

            tabControl11.Dock = DockStyle.Fill;
            dataGridView1.Rows.Clear();
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
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

                    //   MessageBox.Show(TootalPageNum * onePageRowNum + "");


                    DataTable adt = new DataTable();
                    TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
                    TEST_DB.Add_Param("@pageSize", onePageRowNum);
                    TEST_DB.ExecuteSQL(strSql, adt);
                    tabControl11.Visible = true;
                    tabControl11.Dock = DockStyle.Fill;
                    MessageBox.Show(adt.Rows.Count + "");
                    MessageBox.Show(adt.Columns.Count + "");
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
        private void manage()
        {
            tabControl11.Visible = false;
            panel5.Visible = false;
        }
        string etop = "select name,id from equipment where parent_id is null";
        string echild = "select name from equipment where parent_id = @parent_id";
        private void module_Click(object sender, EventArgs e)
        {
            manage();
            panel5.Visible = true;

            panel5.Dock = DockStyle.Fill;
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("根节点");
            DataTable adt = new DataTable();
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(etop, adt);
            int parent_id = 1;
            int j = 0;
            int k = 0;
            for (i = 0; i < adt.Rows.Count; i++)
            {
                treeView1.Nodes[0].Nodes.Add(adt.Rows[i][0] + "");
                parent_id = Convert.ToInt32(adt.Rows[i][1]);
                TEST_DB.Add_Param("@parent_id", parent_id);
                TEST_DB.ExecuteSQL(echild, dt);
                for (k = 0; k < dt.Rows.Count; k++)
                {
                    treeView1.Nodes[0].Nodes[j].Nodes.Add(dt.Rows[k][0] + "");
                }
                j++;
                dt.Dispose();
            }
            adt.Dispose();
            // treeView1.Nodes.Add("根节点");

        }

        private void 添加子节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_AddEquipment f5 = new Frm_AddEquipment();
            f5.cnodeName = nodeName;
            if (f5.ShowDialog() == DialogResult.OK)
            {
                treeView1.SelectedNode.Nodes.Add(f5.nodeName);
            }
        }
        string dae = "delete from equipment where id =@id || parent_id=@parent_id ";
        string sfe = "select id from equipment where name =@name";
        private void 删除节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            TEST_DB.Add_Param("@name", treeView1.SelectedNode.Text.Trim());
            TEST_DB.ExecuteSQL(sfe, dt);
            if (dt.Rows.Count > 0)
            {
                int id = Convert.ToInt32(dt.Rows[0][0]);

                TEST_DB.Add_Param("@id", id);
                TEST_DB.Add_Param("@parent_id", id);
                if (TEST_DB.ExecuteDML(dae) > 0)
                {
                    treeView1.SelectedNode.Remove();
                }
                else
                {
                    MessageBox.Show("删除失败");
                }

            }
            else
            {
                MessageBox.Show("删除失败");
            }
            dt.Dispose();

        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                int x = e.X;
                int y = e.Y;
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);

                if (CurrentNode is TreeNode)//判断你点的是不是一个节点
                {

                    if (CurrentNode.Level < 2)
                    {
                        nodeName = CurrentNode.Text;
                        treeView1.SelectedNode = CurrentNode;
                        CurrentNode.ContextMenuStrip = this.contextMenuStrip7;
                        contextMenuStrip7.Show(MousePosition);
                    }
                    else
                    {
                        // nodeName ="根节点";
                        treeView1.SelectedNode = CurrentNode;
                        CurrentNode.ContextMenuStrip = this.contextMenuStrip1;
                        contextMenuStrip1.Show(MousePosition);
                    }
                    ///MessageBox.Show(Convert.ToString(CurrentNode.Level));

                }
            }
        }
        string dde = "delete from equipment where name =@name";
        //string ide = "insert into equipment ()";
        private void 删除节点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // MessageBox.Show(treeView1.SelectedNode.Text);
            TEST_DB.Add_Param("@name", treeView1.SelectedNode.Text.Trim());

            if (TEST_DB.ExecuteDML(dde) > 0)
            {
                treeView1.SelectedNode.Remove();
            }
            else
            {
                MessageBox.Show("删除失败");
            }

            //treeView1.SelectedNode.Remove();
        }
        public void updateDataGridView2(string name) {
            dataGridView2.Rows.Clear();
            DataTable dt = new DataTable();
            temp = new string[8];
            //  MessageBox.Show(id + "");
            TEST_DB.Add_Param("@name", name );
            // dt.Dispose();
            TEST_DB.ExecuteSQL(setstring, dt);
            if (dt.Rows.Count > 0)
            {

                int i = 0;
                int j = dt.Rows.Count / 8;
                // MessageBox.Show(dt.Rows.Count + "");
                // MessageBox.Show(dt.Columns.Count + "");

                for (i = 0; i < j; i++)
                {
                    dataGridView2.Rows.Add(dt.Rows[0][0], dt.Rows[0][1], dt.Rows[i * 8][2], dt.Rows[i * 8 + 1][2], dt.Rows[i * 8 + 2][2], dt.Rows[i * 8 + 3][2], dt.Rows[i * 8 + 4][2], dt.Rows[i * 8 + 5][2], dt.Rows[i * 8 + 6][2], dt.Rows[i * 8 + 7][2], "修改");
                }
                int k = 0;
                for (i = j * 8; i < dt.Rows.Count; i++)
                {
                    temp[k] = dt.Rows[i][2] + "";
                    k++;
                }
                dataGridView2.Rows.Add(dt.Rows[0][0], dt.Rows[0][1], temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], "修改");
            }
            else
            {
                int id = 0;
                TEST_DB.Add_Param("@name", name);
                TEST_DB.ExecuteSQL(sfe, dt);
                if (dt.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dt.Rows[0][0]);

                }
                dataGridView2.Rows.Add(id, name, temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], "修改");
            }

            dt.Dispose();
        }
        string setstring = "select e.id, e.`name`,t.`name` from equipment e   join tags t on e.id = t.equipment_id where e.`name` = @name";
        string[] temp;
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point ClickPoint = new Point(e.X, e.Y);
            //  int x = e.X;
            //  int y = e.Y;
            TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
          
            if (CurrentNode.Level == 2)
            {
                updateDataGridView2(CurrentNode.Text.Trim());
            }
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MessageBox.Show(e.ColumnIndex+"hg");
            if (e.ColumnIndex == 10 && e.RowIndex >= 0)
            {
                Point curPosition = e.Location;//当前鼠标在当前单元格中的坐标
                if (this.dataGridView2.Columns[e.ColumnIndex].HeaderText == "操作")
                {
                    Frm_EAddTags f = new Frm_EAddTags();
                    f.eName = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f.eId =Convert.ToInt32( dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                    //  f5.cnodeName = nodeName;
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        updateDataGridView2(f.eName);
                        //   treeView1.SelectedNode.Nodes.Add(f5.nodeName);
                    }
                    // this.dataGridView2.Rows.RemoveAt(e.RowIndex);
                    //MessageBox.Show( "hg");
                }
            }
        }
    }
}
