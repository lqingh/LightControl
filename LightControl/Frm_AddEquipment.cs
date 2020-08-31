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
    public partial class Frm_AddEquipment : Form
    {
        public Frm_AddEquipment()
        {
            InitializeComponent();
        }
        DBConn.DB_SQL TEST_DB = com.TEST_DB;
        public string cnodeName;
        int tagsCount = 0;
        public string nodeName
         {
             get { return textBox1.Text.Trim(); }
         }
        string s = "select * from equipment_type";
        private void Frm_AddEquipment_Load(object sender, EventArgs e)
        {
            int i = 0;
            DataTable dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "ID";
            List<listItem> list = new List<listItem>();
            for (i=0;i<dt.Rows.Count;i++)
            {
                listItem li = new listItem();
                li.ID = Convert.ToInt32(dt.Rows[i][0]);
                li.Name = Convert.ToString(dt.Rows[i][1]);
                list.Add(li);
            }
            dt.Dispose();
            comboBox2.DataSource = list;
            s = "select * from tags where tt_id = 1 and tags.id not in (select e.szd_tag from equipment e where e.szd_tag is not null)";
            dt = new DataTable();
            TEST_DB.ExecuteSQL(s, dt);
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "ID";
            list = new List<listItem>();
            for (i = 0; i < dt.Rows.Count; i++)
            {
                listItem li = new listItem();
                li.ID = Convert.ToInt32(dt.Rows[i][0]);
                li.Name = Convert.ToString(dt.Rows[i][1]);
                list.Add(li);
            }
            tagsCount = dt.Rows.Count;
            dt.Dispose();
            comboBox1.DataSource = list;
        }

        private void button2_Click(object sender, EventArgs e)//取消
        {
            DialogResult = DialogResult.Cancel;
        }
        string sfe = "select id from equipment where name =@name";
        string ie = "insert into equipment(NAME,parent_id,et_id,szd_tag) VALUE (@name,@parent_id,@et_id,@szd_tag)";
        int parent_id=0;
        private void button1_Click(object sender, EventArgs e)//保存
        {

            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                MessageBox.Show("请填写节点名称！");
                return;
            }
            else {
                if (textBox1.Text.Trim().EndsWith(".")) {
                    MessageBox.Show("机器名称不能以句号结尾");
                    return;
                }
                if (tagsCount == 0) {
                    MessageBox.Show("每台机器必须有一个手自动标签点");
                    return;
                }
                if (cnodeName != "根节点.") {
                    DataTable dt = new DataTable();
                   // MessageBox.Show(cnodeName);
                    TEST_DB.Add_Param("@name", cnodeName);
                    TEST_DB.ExecuteSQL(sfe, dt);
                  //  MessageBox.Show(Convert.ToString( dt.Rows.Count));
                    if (dt.Rows.Count>0) {
                        parent_id =Convert.ToInt32( dt.Rows[0][0]);
                    }
                    dt.Dispose();
                }
                TEST_DB.Add_Param("@name", textBox1.Text.Trim());
                TEST_DB.Add_Param("@et_id", this.comboBox2.SelectedValue);
               
                if (this.comboBox2.Text == "回路控制器")
                {
                    TEST_DB.Add_Param("@szd_tag", this.comboBox1.SelectedValue);
                }
                else {
                    TEST_DB.Add_Param("@szd_tag",null);
                }
               
                if (this.parent_id == 0)
                {
                    TEST_DB.Add_Param("@parent_id", null);
                }
                else
                {
                    TEST_DB.Add_Param("@parent_id", parent_id);
                }
                if (TEST_DB.ExecuteDML(ie) > 0)
                {
                    MessageBox.Show("添加成功");
                    DialogResult = DialogResult.OK;
                }
                else {
                    MessageBox.Show("添加失败");
                }
            }
           
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.comboBox2.Text == "回路控制器")
            {
                this.comboBox1.Visible = true;
                label3.Visible = true;
            }
            else {
                this.comboBox1.Visible = false;
                label3.Visible = false;
            }
        }
    }
}
