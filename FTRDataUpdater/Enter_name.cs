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
    public partial class Enter_name : Form
    {
        public string current_name; public string[] f_list;
        public Enter_name()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Auxiliary.Get_Equals_by_key(f_list, textBox3.Text) || textBox3.Text == "")
            {
                MessageBox.Show("Поле для ввода имени пустое, либо файл с таким именем уже существует!", "Ошибка");
            }
            else
            {
                if (textBox3.Text.IndexOf('\\') != -1 || textBox3.Text.IndexOf('<') != -1 || textBox3.Text.IndexOf('>') != -1)
                {
                    MessageBox.Show("Имя содержит недопустимые символы!", "Ошибка");
                    return;
                }
                current_name = textBox3.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void Enter_name_Load(object sender, EventArgs e)
        {
            textBox2.Text = current_name;
        }
    }
}
