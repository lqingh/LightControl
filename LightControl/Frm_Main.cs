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
            //MessageBox.Show(TootalPageNum * onePageRowNum + "");
            // MessageBox.Show(onePageRowNum + "");
            TootalPageNum = 0;
            onePageRowNum = 2;
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(strSql, dt);
           // MessageBox.Show(Convert.ToString(dt.Rows.Count));
            manage();
            tabControl11.Visible = true;

            tabControl11.Dock = DockStyle.Fill;
            dataGridView1.Rows.Clear();
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
            }
            string s = "select count(*) from tags";
            dt.Dispose();
            dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
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
            tabControl1.Visible = false;
            tabControl2.Visible = false;
            tabControl3.Visible = false;
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
        string hs = "select *  from holidays limit @curPage,@pageSize";
        void udgv3() {
          
            dataGridView3.Rows.Clear();
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(hs, dt);
            // int index = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView3.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3]);
            }
            dt.Dispose();
            
            toolStripLabel1.Text = "/共" + (loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum).ToString() : (loopTootal / onePageRowNum + 1).ToString()) + "页";

        }
        private void button3_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 2;
            string s = "select count(*) from holidays";
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            loopTootal = Convert.ToInt32(dt.Rows[0][0]);
            dt.Dispose();
            manage();
            tabControl1.Visible = true;

            tabControl1.Dock = DockStyle.Fill;
            udgv3();
        }

        private void dataGridView3_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {

                if (this.dataGridView3.Columns[e.ColumnIndex].HeaderText == "操作")
                {

                    StringFormat sf = StringFormat.GenericDefault.Clone() as StringFormat;//设置重绘入单元格的字体样式
                    sf.FormatFlags = StringFormatFlags.DisplayFormatControl;
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    e.PaintBackground(e.CellBounds, false);//重绘边框

                    //设置要写入字体的大小
                    System.Drawing.Font myFont = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = e.Graphics.MeasureString("删 除", myFont);
                    SizeF sizeMod = e.Graphics.MeasureString("修 改", myFont);
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width); //
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width);
                    //  float fLook = sizeLook.Width / (sizeDel.Width + sizeMod.Width + sizeLook.Width);

                    //设置每个“按钮的边界”
                    RectangleF rectDel = new RectangleF(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width * fDel, e.CellBounds.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, e.CellBounds.Top, e.CellBounds.Width * fMod, e.CellBounds.Height);
                    e.Graphics.DrawString("删 除", myFont, Brushes.Black, rectDel, sf); //绘制“按钮”
                    e.Graphics.DrawString("修 改", myFont, Brushes.Black, rectMod, sf);
                    e.Handled = true;
                }

            }
        }

        private void button6_Click(object sender, EventArgs e)//添加节假日
        {
            Frm_addHoliday ah = new Frm_addHoliday();
            if (ah.ShowDialog() == DialogResult.OK)
            {
                loopTootal = loopTootal + 1;
                udgv3();
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
           
        }
        private void deleteHoliday(int id)
        {
            if (MessageBox.Show("您确定要删除该记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string s = "delete from holidays where id =@id";
                TEST_DB.Add_Param("@id", id);
                if (TEST_DB.ExecuteDML(s)>0)
                {
                    MessageBox.Show("删除成功");
                    loopTootal = loopTootal - 1;
                    udgv3();

                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
        }
        private void editHoliday(int id)
        {
            Frm_addHoliday ah = new Frm_addHoliday();
            ah.id = id;
            if (ah.ShowDialog() == DialogResult.OK)
            {
                udgv3();
            }
        }
        private void dataGridView3_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                Point curPosition = e.Location;//当前鼠标在当前单元格中的坐标
                if (this.dataGridView3.Columns[e.ColumnIndex].HeaderText == "操作")
                {
                    Graphics g = this.dataGridView3.CreateGraphics();
                    System.Drawing.Font myFont = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = g.MeasureString("删除", myFont);
                    SizeF sizeMod = g.MeasureString("修改", myFont);
                    //SizeF sizeLook = g.MeasureString("查看", myFont);
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width);
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width);
                    //float fLook = sizeLook.Width / (sizeDel.Width + sizeMod.Width + sizeLook.Width);

                    Rectangle rectTotal = new Rectangle(0, 0, this.dataGridView3.Columns[e.ColumnIndex].Width, this.dataGridView3.Rows[e.RowIndex].Height);
                    RectangleF rectDel = new RectangleF(rectTotal.Left, rectTotal.Top, rectTotal.Width * fDel, rectTotal.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, rectTotal.Top, rectTotal.Width * fMod, rectTotal.Height);
                    //   MessageBox.Show(dataGridView8.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //  RectangleF rectLook = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fLook, rectTotal.Height);
                    //判断当前鼠标在哪个“按钮”范围内
                    if (rectDel.Contains(curPosition))//删除
                        deleteHoliday(Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectMod.Contains(curPosition))//修改
                        editHoliday(Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString()));

                }
            }
        }
        //节假日尾页
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            TootalPageNum = (loopTootal % onePageRowNum == 0) ? (loopTootal / onePageRowNum) - 1 : (loopTootal / onePageRowNum);
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            udgv3();
        }
        //下一页
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < ((loopTootal % onePageRowNum == 0) ? (loopTootal / onePageRowNum) - 1 : (loopTootal / onePageRowNum)))
            {
                TootalPageNum++;
            }
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            udgv3();
        }
        //上一页
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
            }
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            udgv3();
        }
        //首页
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            udgv3();
        }
        string us = "select *  from scene limit @curPage,@pageSize";

        public void uscene() {
            dataGridView4.Rows.Clear();
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(us, dt);
            // int index = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView4.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], Convert.ToInt32( dt.Rows[i][2])==1 ? "是":"否", dt.Rows[i][3]);
            }
            dt.Dispose();

            toolStripLabel2.Text = "/共" + (loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum).ToString() : (loopTootal / onePageRowNum + 1).ToString()) + "页";

        }
        private void scene_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 2;
            string s = "select count(*) from scene";
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            loopTootal = Convert.ToInt32(dt.Rows[0][0]);
            dt.Dispose();
            manage();
         
            tabControl2.Visible = true;

            tabControl2.Dock = DockStyle.Fill;
            uscene();
        }

        private void dataGridView4_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {

                if (this.dataGridView4.Columns[e.ColumnIndex].HeaderText == "操作")
                {

                    StringFormat sf = StringFormat.GenericDefault.Clone() as StringFormat;//设置重绘入单元格的字体样式
                    sf.FormatFlags = StringFormatFlags.DisplayFormatControl;
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    e.PaintBackground(e.CellBounds, false);//重绘边框

                    //设置要写入字体的大小
                    System.Drawing.Font myFont = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = e.Graphics.MeasureString("删 除", myFont);
                    SizeF sizeMod = e.Graphics.MeasureString("修 改", myFont);
                    SizeF sizeDS = e.Graphics.MeasureString("定时设置", myFont);
                    SizeF sizeHL = e.Graphics.MeasureString("回路设置", myFont);
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width+ sizeHL.Width); //
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width);
                    float fDS = sizeDS.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width); //
                    float fHL = sizeHL.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width);

                    //  float fLook = sizeLook.Width / (sizeDel.Width + sizeMod.Width + sizeLook.Width);

                    //设置每个“按钮的边界”
                    RectangleF rectDel = new RectangleF(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width * fDel, e.CellBounds.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, e.CellBounds.Top, e.CellBounds.Width * fMod, e.CellBounds.Height);
                    RectangleF rectDS = new RectangleF(rectMod.Right, e.CellBounds.Top, e.CellBounds.Width * fDS, e.CellBounds.Height);
                    RectangleF rectHL = new RectangleF(rectDS.Right, e.CellBounds.Top, e.CellBounds.Width * fHL, e.CellBounds.Height);

                    e.Graphics.DrawString("删 除", myFont, Brushes.Black, rectDel, sf); //绘制“按钮”
                    e.Graphics.DrawString("修 改", myFont, Brushes.Black, rectMod, sf);
                    e.Graphics.DrawString("定时设置", myFont, Brushes.Black, rectDS, sf); //绘制“按钮”
                    e.Graphics.DrawString("回路设置", myFont, Brushes.Black, rectHL, sf);

                    e.Handled = true;
                }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Frm_addScene fa = new Frm_addScene();
            if (fa.ShowDialog() == DialogResult.OK)
            {
                uscene();
            }
        }

        private void dataGridView4_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                Point curPosition = e.Location;//当前鼠标在当前单元格中的坐标
                if (this.dataGridView4.Columns[e.ColumnIndex].HeaderText == "操作")
                {
                    Graphics g = this.dataGridView4.CreateGraphics();
                    System.Drawing.Font myFont = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = g.MeasureString("删 除", myFont);
                    SizeF sizeMod = g.MeasureString("修 改", myFont);
                    SizeF sizeDS = g.MeasureString("定时设置", myFont);
                    SizeF sizeHL = g.MeasureString("回路设置", myFont);
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width); //
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width);
                    float fDS = sizeDS.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width); //
                    float fHL = sizeHL.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width + sizeHL.Width);

                    Rectangle rectTotal = new Rectangle(0, 0, this.dataGridView4.Columns[e.ColumnIndex].Width, this.dataGridView4.Rows[e.RowIndex].Height);
                    RectangleF rectDel = new RectangleF(rectTotal.Left, rectTotal.Top, rectTotal.Width * fDel, rectTotal.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, rectTotal.Top, rectTotal.Width * fMod, rectTotal.Height);
                    RectangleF rectDS = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fDS, rectTotal.Height);
                    RectangleF rectHL = new RectangleF(rectDS.Right, rectTotal.Top, rectTotal.Width * fHL, rectTotal.Height);

                    //   MessageBox.Show(dataGridView8.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //  RectangleF rectLook = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fLook, rectTotal.Height);
                    //判断当前鼠标在哪个“按钮”范围内
                    if (rectDel.Contains(curPosition))//删除
                        deleteScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectMod.Contains(curPosition))//修改
                        editScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectDS.Contains(curPosition))//定时设置
                        timeScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectHL.Contains(curPosition))//回路设置
                        loopScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()),dataGridView4.Rows[e.RowIndex].Cells[2].Value.ToString());

                }
            }
        }
        private void timeScene(int id)
        {

        }
        private void loopScene(int id,string name) {
            Frm_loopScene ls = new Frm_loopScene();
            ls.eId = id;
            ls.eName = name;
            if (ls.ShowDialog() == DialogResult.OK) {

            }
        }
        private void deleteScene(int id)
        {
            if (MessageBox.Show("您确定要删除该记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string s = "delete from scene where id =@id";
                TEST_DB.Add_Param("@id", id);
                if (TEST_DB.ExecuteDML(s) > 0)
                {
                    MessageBox.Show("删除成功");
                    loopTootal = loopTootal - 1;
                    uscene();

                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
        }
        private void editScene(int id)
        {
            Frm_addScene fas = new Frm_addScene();
            fas.id = id;
            if (fas.ShowDialog() == DialogResult.OK)
            {
                uscene();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 2;
            string s = "select count(*) from epoint";
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            loopTootal = Convert.ToInt32(dt.Rows[0][0]);
            dt.Dispose();
            manage();

            tabControl3.Visible = true;

            tabControl3.Dock = DockStyle.Fill;
            utime();
        }
     
        public void utime()
        {
            string s = "select id,name,s_time,e_time,note  from epoint limit @curPage,@pageSize";

            dataGridView5.Rows.Clear();
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(s, dt);
            // int index = 0;
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView5.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][2], dt.Rows[i][3], dt.Rows[i][4]);
            }
            dt.Dispose();

            toolStripLabel3.Text = "/共" + (loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum).ToString() : (loopTootal / onePageRowNum + 1).ToString()) + "页";

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Frm_addTime at = new Frm_addTime();
            if (at.ShowDialog() == DialogResult.OK) {
                utime();
            }
        }
    }
}
