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
    public partial class Frm_Addloop_Manual : Form
    {
        string tagName = "";        // 标签名
        string remarksText = "";    // 备注信息

        public Frm_Addloop_Manual()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("标签名不能为空");
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("备注不能为空");
                return;
            }

            tagName = textBox1.Text;
            remarksText = textBox2.Text;
        }

    }
}
