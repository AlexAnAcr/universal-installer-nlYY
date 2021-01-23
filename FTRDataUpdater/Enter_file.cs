using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FTPDataUpdater
{
    public partial class Enter_file : Form
    {
        public bool[] a_files = new bool[4] { true, true, true, true }; public byte answer = 0;
        public Enter_file()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked)
            {
                if(radioButton2.Checked)
                    answer = 1;
                else if (radioButton3.Checked)
                    answer = 2;
                else if (radioButton4.Checked)
                    answer = 3;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Вы не выбрали элемент!", "Ошибка");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Enter_file_Load(object sender, EventArgs e)
        {
            radioButton1.Enabled = a_files[0];
            radioButton2.Enabled = a_files[1];
            radioButton3.Enabled = a_files[2];
            radioButton4.Enabled = a_files[3];
        }
    }
}
