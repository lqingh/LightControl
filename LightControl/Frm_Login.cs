using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LightControl
{
    public partial class Frm_Login : Form
    {
        public Frm_Login()
        {
            InitializeComponent();
        }

        DBConn.DB_SQL TEST_DB = com.TEST_DB;

        private bool ValidateInput()
        {
            if (txtID.Text.Trim() == "")                            //登录账号
            {
                MessageBox.Show("请输入登录账号", "登录提示", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
                txtID.Focus();                                  //使登录账号文本框获得鼠标焦点
                return false;
            }
            else if (txtPwd.Text.Trim() == "")//密码
            {
                MessageBox.Show("请输入密码", "登录提示", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
                txtPwd.Focus();                                 //使密码文本框获得鼠标焦点
                return false;
            }
            return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (char.IsDigit(e.KeyChar) || ('a'<= e.KeyChar && e.KeyChar <= 'z' ) || ('A' <= e.KeyChar && e.KeyChar <= 'Z'))
                e.Handled =   false;                                      //在控件中显示该字符
            else
                e.Handled = true;										//取消在控件中显示该字符
        }


        private void pboxLogin_Click_1(object sender, EventArgs e)
        {
           
              
         
            
         }
     
       
        private void pboxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Frm_Login_Load(object sender, EventArgs e)
        {
            TEST_DB.fDBServerIP = "192.168.0.102";

            TEST_DB.fDBAccount = "root";

            TEST_DB.fDBPasswd = "123456";
            TEST_DB.fDBServiceName = "lightcontrol";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                string strMsg = string.Empty;
                DataTable dt = new DataTable();  
                string passWord = txtPwd.Text.Trim();
                string userName = txtID.Text.Trim();
                if (TEST_DB.DBConnect(ref strMsg))
                {
                    string strSql = "select * from user where name = @userName and password = @passWord";
                    TEST_DB.Add_Param("@userName", userName);
                    TEST_DB.Add_Param("@passWord", passWord); 
                    TEST_DB.ExecuteSQL(strSql, dt);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("用户名或密码不正确");
                    }
                    else {
                        MessageBox.Show("登录成功");
                        Frm_Main frmMain = new Frm_Main();                  //创建主窗体对象
                        frmMain.user = userName;
                        frmMain.StartPosition = FormStartPosition.CenterScreen;
                        frmMain.Show();                                     //显示主窗体
                        this.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show(this, strMsg);
                   
                   
                }
            //    if (rdata.code == 10200)
              //  {
                  
                  //  Frm_Main frmMain = new Frm_Main();                  //创建主窗体对象
                  //  frmMain.StartPosition = FormStartPosition.CenterScreen;
                  //  frmMain.Show();                                     //显示主窗体
               //     this.Visible = false;
               // }
                //   com.user = JsonConvert.DeserializeObject<user>(rdata.result);


              //  MessageBox.Show(rdata.message);
            }

        }
    }
}
    



