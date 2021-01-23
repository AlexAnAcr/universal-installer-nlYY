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
    public partial class Enter_fname : Form
    {
        public OFC_FS fs; public string current_way, text;
        public Enter_fname()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fs.OFC_Entry_exist(current_way, textBox3.Text, OFC_FS.OFC_Types.Folder) || textBox3.Text == "")
            {
                MessageBox.Show("Поле для ввода пустое, либо элемент с таким именем уже существует!", "Ошибка");
            }
            else
            {
                if (textBox3.Text.IndexOf('\\') != -1 || textBox3.Text.IndexOf('<') != -1 || textBox3.Text.IndexOf('>') != -1)
                {
                    MessageBox.Show("Имя содержит недопустимые символы!", "Ошибка");
                    return;
                }
                current_way = textBox3.Text;
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
            label1.Text = text;
        }
    }
}
