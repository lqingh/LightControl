using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightControl
{
    public partial class Frm_Main : Form
    {
        int loopTootal = 0;       // 记录回路的总数
        int loopPage = 0; // 回路总页数
        int TootalPageNum = 0;  // 用来记录在"用户"中当前所在的页码数
        int onePageRowNum = 15;     // 设置一页显示的行数
        string nodeName = "";
        isee see;
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public Frm_Main()
        {
            InitializeComponent();
        }
        public string user = "";
        private void Frm_Main_Load(object sender, EventArgs e)
        {
            name.Text = user;
            see = new isee();
            see.getIseeData();
            //isee.getIseeData();
            //  string st1 = "12:13";

            // string st2 = "14:14";
            //MessageBox.Show(see.getOneTagValue("test34") + "");
            // MessageBox.Show( DateTime.Compare(Convert.ToDateTime(st1), Convert.ToDateTime(st2))+"");
            // MessageBox.Show( Convert.ToInt32( DateTime.Now.DayOfWeek)+"");
             Thread th = new Thread(new ThreadStart(ThreadMethod)); //创建线程                     
              th.Start();
        }
        int stmcount = 100;
        int stncount = 0;
        DateTime[,] sceneT = new DateTime[100,2];//存储场景里每个定时的起始时间和结束时间
        int[,] sceneO = new int[100, 4];//存储场景id,场景里面每个定时的重复日期，是否有节假日以及是否执行过，1 表示开灯完毕，2表示关灯完毕
        int hmcount = 50;
        int hncount = 0;
        DateTime[,] holiday = new DateTime[50, 2];//节假日
        int week = 0;
        void ThreadMethod()
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rday = 0;
            int tag = 0;
            int temp = 1;
            DateTime tNow;
            int oldDay = 0;
            int day = 0;
            Boolean isHoliday = false; //判断当前是否为节假日
            Hashtable ht = new Hashtable();
            DataTable dt,dt1;
            string s,s1;
            string szdtag;
           
            while (true)
            {
                if (com.updateTime) {
                  //  string s = "select count(*) from scene_epoint se join scene s on se.scene_id = s.id where s.enablement = 1";
                    s = "select se.scene_id,e.rday,e.holiday_visiable,e.s_time,e.e_time from scene_epoint se join epoint e on se.epoint_id = e.id join scene s on se.scene_id = s.id where s.enablement = 1";
                   
                    dt = new DataTable();
                    TEST_DB.ExecuteSQL(s, dt);
                    stncount = Convert.ToInt32(dt.Rows.Count);
                    if (stncount > stmcount) {//防止每次场景数变更时，都要改变数组大小
                        stmcount = stncount + 50;
                        sceneT = new DateTime[stmcount, 2];
                        sceneO = new int[stmcount, 4];
                    }
                   
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        sceneO[i,0] =Convert.ToInt32( dt.Rows[i][0]);
                        sceneO[i, 1] = Convert.ToInt32(dt.Rows[i][1]);
                        sceneO[i, 2] = Convert.ToInt32(dt.Rows[i][2]);
                        sceneO[i, 3] = 0; 
                        sceneT[i, 0] = Convert.ToDateTime(dt.Rows[i][3]+"");
                        sceneT[i, 1] = Convert.ToDateTime(dt.Rows[i][4]+"");
                    }
                   
                    dt.Dispose();
                    s = "select sDate,eDate from holidays ";//查找节假日
                    dt = new DataTable();
                    TEST_DB.ExecuteSQL(s, dt);
                    hncount = Convert.ToInt32(dt.Rows.Count);
                    if (hncount > hmcount)
                    {//防止每次节假日数变更时，都要改变数组大小
                        hmcount = hmcount + 50;
                        holiday = new DateTime[hmcount, 2];
                    }
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        holiday[i, 0] = Convert.ToDateTime(dt.Rows[i][0] + "");
                        holiday[i, 1] = Convert.ToDateTime(dt.Rows[i][1] + "");
                    }
                    dt.Dispose();
                    s = "select t.`name`,ts.`name` as szd from tags t join equipment e on t.equipment_id = e.id  join tags ts on e.szd_tag = ts.id";//查找回路对应的手自动回路
                    dt = new DataTable();
                    TEST_DB.ExecuteSQL(s, dt);
                    ht.Clear();
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        ht.Add(Convert.ToString( dt.Rows[i][0]), Convert.ToString(dt.Rows[i][1]));
                    }
                    dt.Dispose();
                    com.updateTime = false;
                }
                day =  System.DateTime.Now.Day;
                if (day != oldDay) {
                    oldDay = day;
                    for (j = 0; j < stncount; j++)//防止由于切换成手自动后跳过关灯，使得标识位为1，导致第二天无法正常开灯
                    {
                        sceneO[j, 3] = 0;
                    }
                    week = Convert.ToInt32(DateTime.Now.DayOfWeek);//周日为0
                    if (week == 0)
                    {
                        week = 7;
                    }
                   
                    temp = 1;
                    temp = temp << week - 1;
                    isHoliday = false;
                    for (j = 0; j < hncount; j++)
                    {
                        if (DateTime.Compare(System.DateTime.Now,holiday[j, 0])>=0 && DateTime.Compare(System.DateTime.Now, holiday[j, 1]) <= 0) {
                            isHoliday = true;
                            break;
                        }
                    }
                }
                tNow = Convert.ToDateTime(System.DateTime.Now.ToString("t"));
                s = "select t.name from scene s join scene_tags st on s.id = st.scene_id join tags t on st.tags_id = t.id where s.id = @id";
                s1 = "select e.s_time,e.e_time,s.enablement from scene_tags st join scene s on st.scene_id = s.id join tags t on st.tags_id = t.id join scene_epoint se on st.scene_id = se.scene_id join epoint e on se.epoint_id = e.id where t.name = @name";
              
                for (j = 0; j < stncount; j++)
                {
                   
                    if (sceneO[j, 2] == 1 && isHoliday)//有启用节假日，并且当前为节假日
                    {
                        continue;
                    }
                    else
                    {
                        rday = sceneO[j, 1];
                        
                        if ((rday & temp) > 0)//判断是否在重复日期之内
                        {
                           
                            if (DateTime.Compare(sceneT[j, 0], tNow) <= 0 && DateTime.Compare(sceneT[j, 1], tNow) >=0 && sceneO[j, 3] != 1)//到达开灯时间，且之前没有开灯过
                            {
                              //  MessageBox.Show("a1");
                                sceneO[j, 3] = 1;
                                TEST_DB.Add_Param("@id", sceneO[j, 0]);
                                dt = new DataTable();
                                TEST_DB.ExecuteSQL(s, dt);
                                for (i = 0; i < dt.Rows.Count; i++)
                                {
                                    
                                    szdtag = Convert.ToString(ht[Convert.ToString(dt.Rows[i][0])]);
                                    if (see.getOneTagValue(szdtag) == 1)//判断手自动状态,0为自动，1为手动
                                    {
                                       
                                        see.ModifyOneTag(Convert.ToString(dt.Rows[i][0]), 1);//1为开灯，0为关灯
                                    }
                                }
                                dt.Dispose();
                            }
                            else if (DateTime.Compare(sceneT[j, 1], tNow) <= 0 && sceneO[j, 3] != 2)//到达关灯时间，且之前没有关灯过
                            {
                                //MessageBox.Show("fg1");
                                sceneO[j, 3] = 2;
                                TEST_DB.Add_Param("@id", sceneO[j, 0]);
                                dt = new DataTable();
                                TEST_DB.ExecuteSQL(s, dt);
                                for (i = 0; i < dt.Rows.Count; i++)
                                {
                                  
                                    szdtag = Convert.ToString(ht[Convert.ToString(dt.Rows[i][0])]);
                                   
                                    if (see.getOneTagValue(szdtag) == 1)//判断手自动状态,1为自动，0为手动
                                    {
                                        
                                        TEST_DB.Add_Param("@name", Convert.ToString(dt.Rows[i][0]));
                                        dt1 = new DataTable();
                                        TEST_DB.ExecuteSQL(s1, dt1);
                                        tag = 0;
                                        if (dt1.Rows.Count > 1)
                                        {//判断回路是否存在于多个场景
                                            
                                            for (k = 0; k < dt1.Rows.Count; k++)//判断不同场景上时间是否有重叠
                                            {       
                                                if (Convert.ToInt32( dt1.Rows[k][2]) == 1) {//判断场景是否启用
                                                   
                                                    if (DateTime.Compare(Convert.ToDateTime(dt1.Rows[k][0]+""), sceneT[j, 1]) <=0 && DateTime.Compare(Convert.ToDateTime(dt1.Rows[k][1]+""), sceneT[j, 1]) > 0) {
                                                        tag = 1;
                                                        break;
                                                    }                        
                                                }
                                            }
                                        }
                                        if (tag ==0) {
                                            //MessageBox.Show("fg");
                                            see.ModifyOneTag(Convert.ToString(dt.Rows[i][0]), 0);
                                        }
                                        dt1.Dispose();
                                    }
                                }
                                dt.Dispose();
                            }
                        }
                    }
                }
                //MessageBox.Show("fg");
                Thread.Sleep(60);//如果不延时，将占用CPU过高  
            }
        }
        string strSql = "select t.id,t.name,t.note,e.`name` as eName,t.tt_id,tt.type  from tags t left join equipment e on t.equipment_id = e.id join tags_type tt on t.tt_id = tt.id limit @curPage,@pageSize";

      
        private void uregion() {
            int i = 0;
            DataTable dt = new DataTable();
            TEST_DB.Add_Param("@curPage", TootalPageNum * onePageRowNum);
            TEST_DB.Add_Param("@pageSize", onePageRowNum);
            TEST_DB.ExecuteSQL(strSql, dt);
            manage();
            tabControl11.Visible = true;
            loopPage = loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum) : (loopTootal / onePageRowNum + 1);
            tabControl11.Dock = DockStyle.Fill;
            dataGridView1.Rows.Clear();
            for (i = 0; i < dt.Rows.Count; i++)
            {
               // MessageBox.Show(Convert.ToString(dt.Rows[i][1]));
                if (Convert.ToInt32(dt.Rows[i][4]) == 1) {//手自动
                    if (see.getOneTagValue(Convert.ToString(dt.Rows[i][1])) == 0)
                    {
                        dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][5], dt.Rows[i][2], dt.Rows[i][3], "手动", dt.Rows[i][4]);

                    }
                    else
                    {
                        dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][5], dt.Rows[i][2], dt.Rows[i][3], "自动", dt.Rows[i][4]);
                    }
                } else if (Convert.ToInt32(dt.Rows[i][4]) == 2) {//回路控制
                    if (see.getOneTagValue(Convert.ToString(dt.Rows[i][1])) == 1)
                    {
                        dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][5], dt.Rows[i][2], dt.Rows[i][3], "开启", dt.Rows[i][4]);

                    }
                    else
                    {
                        dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][5], dt.Rows[i][2], dt.Rows[i][3], "关闭", dt.Rows[i][4]);
                    }
                }
                else if (Convert.ToInt32(dt.Rows[i][4]) == 3)
                {//数据收集
                    dataGridView1.Rows.Add(dt.Rows[i][0], TootalPageNum * onePageRowNum + i + 1, dt.Rows[i][1], dt.Rows[i][5], dt.Rows[i][2], dt.Rows[i][3], "无", dt.Rows[i][4]);
                }
                
            }
            dt.Dispose();
            toolStripLabel7.Text = "/共" + loopPage.ToString() + "页";
        }
        private void region_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 15;
            string s = "select count(*) from tags";
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            loopTootal = Convert.ToInt32(dt.Rows[0][0]);    
           
            dt.Dispose();
            uregion();
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
            TootalPageNum = loopPage-1;
            toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
        }

        // 上一页
        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
                toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
            }
            
        }

        // 下一页
        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < loopPage-1)
            {
                TootalPageNum++;
                toolStripTextBox1.Text = (TootalPageNum + 1).ToString();
            }
           
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (toolStripTextBox1.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox1.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox1.Text.ToString()) <= loopPage)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox1.Text.ToString()) - 1;
                    uregion();
                    return; 
                }
                if (Convert.ToInt32(toolStripTextBox1.Text.ToString())<=0) {
                    toolStripTextBox1.Text = "1";
                }
               
                if (Convert.ToInt32(toolStripTextBox1.Text.ToString())> loopPage) {
                    toolStripTextBox1.Text = loopPage.ToString();
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
            int i = 0;
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
            int i = 0;
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
            loopPage = loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum) : (loopTootal / onePageRowNum + 1);
            toolStripLabel1.Text = "/共" + loopPage.ToString() + "页";

        }
        private void button3_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 15;
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
            Frm_Addloop_Manual ah = new Frm_Addloop_Manual();
            if (ah.ShowDialog() == DialogResult.OK)
            {
                loopTootal = loopTootal + 1;
                uregion();
            }
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
            TootalPageNum = loopPage - 1;
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();          
        }
        //下一页
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < loopPage - 1)
            {
                TootalPageNum++;
                toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            }
           
           
        }
        //上一页
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
                toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
            }
            
           
        }
        //首页
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            toolStripTextBox2.Text = (TootalPageNum + 1).ToString();
           
        }
        string us = "select *  from scene limit @curPage,@pageSize";

        public void uscene() {
            int i = 0;
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
            loopPage = loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum) : (loopTootal / onePageRowNum + 1);
            toolStripLabel2.Text = "/共" + loopPage.ToString() + "页";

        }
        private void scene_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            onePageRowNum = 15;
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
                loopTootal = loopTootal + 1;
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
                        timeScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()), dataGridView4.Rows[e.RowIndex].Cells[2].Value.ToString());
                    else if (rectHL.Contains(curPosition))//回路设置
                        loopScene(Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[0].Value.ToString()),dataGridView4.Rows[e.RowIndex].Cells[2].Value.ToString());

                }
            }
        }
        private void timeScene(int id, string name)
        {
            Frm_SceneAddTime sdt = new Frm_SceneAddTime();
            sdt.eId = id;
            sdt.eName = name;
            if (sdt.ShowDialog() == DialogResult.OK)
            {

            }
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
            onePageRowNum = 15;
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
            int i = 0;
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
            loopPage = loopTootal % onePageRowNum == 0 ? (loopTootal / onePageRowNum) : (loopTootal / onePageRowNum + 1);
            toolStripLabel3.Text = "/共" + loopPage.ToString() + "页";

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Frm_addTime at = new Frm_addTime();
            if (at.ShowDialog() == DialogResult.OK) {
                loopTootal = loopTootal + 1;
                utime();
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)//回路
        {
            if (e.ColumnIndex == 7 && e.RowIndex >= 0)
            {

                if (this.dataGridView1.Columns[e.ColumnIndex].HeaderText == "操作")
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
                    SizeF sizeDS = e.Graphics.MeasureString("启用", myFont);
                   
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width ); //
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width);
                    float fDS = sizeDS.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width); //
                   

                    //  float fLook = sizeLook.Width / (sizeDel.Width + sizeMod.Width + sizeLook.Width);

                    //设置每个“按钮的边界”
                    RectangleF rectDel = new RectangleF(e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width * fDel, e.CellBounds.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, e.CellBounds.Top, e.CellBounds.Width * fMod, e.CellBounds.Height);
                    RectangleF rectDS = new RectangleF(rectMod.Right, e.CellBounds.Top, e.CellBounds.Width * fDS, e.CellBounds.Height);
                  
                    e.Graphics.DrawString("删 除", myFont, Brushes.Black, rectDel, sf); //绘制“按钮”
                    e.Graphics.DrawString("修 改", myFont, Brushes.Black, rectMod, sf);
                  //  MessageBox.Show(Convert.ToString(this.dataGridView1.Rows[e.RowIndex].Cells[7].Value));
                    if (Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[7].Value) == 1)
                    {
                        /*if (Convert.ToString(this.dataGridView1.Rows[e.RowIndex].Cells[6].Value) == "手动")
                        {
                            e.Graphics.DrawString("自动", myFont, Brushes.Black, rectDS, sf); //绘制“按钮”
                        }
                        else
                        {
                            e.Graphics.DrawString("手动", myFont, Brushes.Black, rectDS, sf); //绘制“按钮”
                        }*/
                    }
                    else if (Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[7].Value) == 2)
                    {
                        if (Convert.ToString(this.dataGridView1.Rows[e.RowIndex].Cells[6].Value) == "开启")
                        {
                            e.Graphics.DrawString("关闭", myFont, Brushes.Black, rectDS, sf); //绘制“按钮”
                        }
                        else
                        {
                            e.Graphics.DrawString("开启", myFont, Brushes.Black, rectDS, sf); //绘制“按钮”
                        }
                    }
                    else if (Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[7].Value) == 3)
                    {

                    }
                  
                    
                  
                    e.Handled = true;
                }

            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)//回路
        {
            if (e.ColumnIndex == 7 && e.RowIndex >= 0)
            {
                Point curPosition = e.Location;//当前鼠标在当前单元格中的坐标
                if (this.dataGridView1.Columns[e.ColumnIndex].HeaderText == "操作")
                {
                    Graphics g = this.dataGridView1.CreateGraphics();
                    System.Drawing.Font myFont = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = g.MeasureString("删 除", myFont);
                    SizeF sizeMod = g.MeasureString("修 改", myFont);
                    SizeF sizeDS = g.MeasureString("启用", myFont);
                  
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width ); //
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width);
                    float fDS = sizeDS.Width / (sizeDel.Width + sizeMod.Width + sizeDS.Width); //
                  
                    Rectangle rectTotal = new Rectangle(0, 0, this.dataGridView1.Columns[e.ColumnIndex].Width, this.dataGridView1.Rows[e.RowIndex].Height);
                    RectangleF rectDel = new RectangleF(rectTotal.Left, rectTotal.Top, rectTotal.Width * fDel, rectTotal.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, rectTotal.Top, rectTotal.Width * fMod, rectTotal.Height);
                    RectangleF rectDS = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fDS, rectTotal.Height);

                    //   MessageBox.Show(dataGridView8.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //  RectangleF rectLook = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fLook, rectTotal.Height);
                    //判断当前鼠标在哪个“按钮”范围内
                    if (rectDel.Contains(curPosition))//删除
                        deleteTag(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectMod.Contains(curPosition))//修改
                        editScene(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectDS.Contains(curPosition)) {
                        if (Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[7].Value) ==2)
                        {
                         string s =   cTag(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(), dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());
                            dataGridView1.Rows[e.RowIndex].Cells[6].Value = s;
                        }

                    }
                      //  cTag(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                 
                }
            }
        }
        private string cTag(string tagName, string  name) {
            string s,temp;
            temp = name;
            if (MessageBox.Show("您确定更改"+ name + "状态吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                    DataTable dt = new DataTable();
                    s = "select t1.`name` from tags t join equipment e on t.equipment_id = e.id join tags t1 on e.szd_tag = t1.id where t.name= @name";
                    TEST_DB.Add_Param("@name", tagName);
                    TEST_DB.ExecuteSQL(s, dt);
                    if (dt.Rows.Count > 0)
                    {
                      //  MessageBox.Show(Convert.ToString(dt.Rows[0][0]));
                        if (see.getOneTagValue(Convert.ToString( dt.Rows[0][0])) == 0) {
                            MessageBox.Show("请先将回路的手自动标识设置为自动");
                            dt.Dispose();
                            return name;
                        }
                        if (name == "开启")
                        {  
                            see.ModifyOneTag(tagName, 0);//关闭
                            temp = "关闭";
                        }
                        else
                        {
                            see.ModifyOneTag(tagName, 1);
                            temp = "开启";
                        }
                    }
                    else {
                        MessageBox.Show("请先在区域中配置回路的手自动标识");
                    }   
              
                
            }
            return temp;
        }
        private void deleteTag(int id)
        {
            if (MessageBox.Show("您确定要删除该记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string s = "delete from tags where id =@id";
                TEST_DB.Add_Param("@id", id);
                if (TEST_DB.ExecuteDML(s) > 0)
                {
                    MessageBox.Show("删除成功");
                    loopTootal = loopTootal - 1;
                    uregion();

                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
        }

        private void dataGridView5_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 6 && e.RowIndex >= 0)
            {

                if (this.dataGridView5.Columns[e.ColumnIndex].HeaderText == "操作")
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

        private void dataGridView5_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 6 && e.RowIndex >= 0)
            {
                Point curPosition = e.Location;//当前鼠标在当前单元格中的坐标
                if (this.dataGridView5.Columns[e.ColumnIndex].HeaderText == "操作")
                {
                    Graphics g = this.dataGridView5.CreateGraphics();
                    System.Drawing.Font myFont = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    SizeF sizeDel = g.MeasureString("删除", myFont);
                    SizeF sizeMod = g.MeasureString("修改", myFont);
                    //SizeF sizeLook = g.MeasureString("查看", myFont);
                    float fDel = sizeDel.Width / (sizeDel.Width + sizeMod.Width);
                    float fMod = sizeMod.Width / (sizeDel.Width + sizeMod.Width);
                    //float fLook = sizeLook.Width / (sizeDel.Width + sizeMod.Width + sizeLook.Width);

                    Rectangle rectTotal = new Rectangle(0, 0, this.dataGridView5.Columns[e.ColumnIndex].Width, this.dataGridView5.Rows[e.RowIndex].Height);
                    RectangleF rectDel = new RectangleF(rectTotal.Left, rectTotal.Top, rectTotal.Width * fDel, rectTotal.Height);
                    RectangleF rectMod = new RectangleF(rectDel.Right, rectTotal.Top, rectTotal.Width * fMod, rectTotal.Height);
                    //   MessageBox.Show(dataGridView8.Rows[e.RowIndex].Cells[0].Value.ToString());
                    //  RectangleF rectLook = new RectangleF(rectMod.Right, rectTotal.Top, rectTotal.Width * fLook, rectTotal.Height);
                    //判断当前鼠标在哪个“按钮”范围内
                    if (rectDel.Contains(curPosition))//删除
                        deleteTime(Convert.ToInt32(dataGridView5.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    else if (rectMod.Contains(curPosition))//修改
                        editTime(Convert.ToInt32(dataGridView5.Rows[e.RowIndex].Cells[0].Value.ToString()));

                }
            }
        }
        private void editTime(int id)
        {
            Frm_addTime fat = new Frm_addTime();
            fat.id = id;
            if (fat.ShowDialog()== DialogResult.OK) {
                utime();
            }
        }
        private void deleteTime(int id)
        {
            if (MessageBox.Show("您确定要删除该记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string s = "delete from epoint where id =@id";
                TEST_DB.Add_Param("@id", id);
                if (TEST_DB.ExecuteDML(s) > 0)
                {
                    MessageBox.Show("删除成功");
                    loopTootal = loopTootal - 1;
                    utime();

                }
                else
                {
                    MessageBox.Show("删除失败");
                }
            }
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox2.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox2.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox2.Text.ToString()) <= loopPage)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox2.Text.ToString()) - 1;
                    udgv3();
                    return;
                }
                if (Convert.ToInt32(toolStripTextBox2.Text.ToString()) <= 0)
                {
                    toolStripTextBox2.Text = "1";
                }

                if (Convert.ToInt32(toolStripTextBox2.Text.ToString()) > loopPage)
                {
                    toolStripTextBox2.Text = loopPage.ToString();
                }
            }
        }
        //首页
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            toolStripTextBox4.Text = (TootalPageNum + 1).ToString();
        }
        //上一页
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
                toolStripTextBox4.Text = (TootalPageNum + 1).ToString();
            }

        }
        //下一页
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < loopPage - 1)
            {
                TootalPageNum++;
                toolStripTextBox4.Text = (TootalPageNum + 1).ToString();
            }
        }
        //尾页
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            TootalPageNum = loopPage - 1;
            toolStripTextBox4.Text = (TootalPageNum + 1).ToString();
        }

        private void toolStripTextBox4_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox4.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox4.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox4.Text.ToString()) <= loopPage)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox4.Text.ToString()) - 1;
                    utime();
                    return;
                }
                if (Convert.ToInt32(toolStripTextBox4.Text.ToString()) <= 0)
                {
                    toolStripTextBox4.Text = "1";
                }

                if (Convert.ToInt32(toolStripTextBox4.Text.ToString()) > loopPage)
                {
                    toolStripTextBox4.Text = loopPage.ToString();
                }
            }
        }
        //场景首页
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            TootalPageNum = 0;
            toolStripTextBox3.Text = (TootalPageNum + 1).ToString();
        }
        //上一页
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (TootalPageNum > 0)
            {
                TootalPageNum--;
                toolStripTextBox3.Text = (TootalPageNum + 1).ToString();
            }
        }
        //下一页
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (TootalPageNum < loopPage - 1)
            {
                TootalPageNum++;
                toolStripTextBox3.Text = (TootalPageNum + 1).ToString();
            }
        }
        //尾页
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            TootalPageNum = loopPage - 1;
            toolStripTextBox3.Text = (TootalPageNum + 1).ToString();
        }

        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox3.Text != "")
            {
                if (Convert.ToInt32(toolStripTextBox3.Text.ToString()) > 0 && Convert.ToInt32(toolStripTextBox3.Text.ToString()) <= loopPage)
                {
                    TootalPageNum = Convert.ToInt32(toolStripTextBox3.Text.ToString()) - 1;
                    uscene();
                    return;
                }
                if (Convert.ToInt32(toolStripTextBox3.Text.ToString()) <= 0)
                {
                    toolStripTextBox3.Text = "1";
                }

                if (Convert.ToInt32(toolStripTextBox3.Text.ToString()) > loopPage)
                {
                    toolStripTextBox3.Text = loopPage.ToString();
                }
            }
        }
    }
}
