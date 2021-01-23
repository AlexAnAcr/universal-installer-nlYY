using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FTPDataUpdater
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        private void Login_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\mems.dat"))
            {
                string[] lines = System.IO.File.ReadAllLines(Application.StartupPath + "\\mems.dat");
                for (ushort i = 0; i < lines.Length; i++)
                {
                    string[] lines_split = lines[i].Split('|');
                    if (lines_split.Length == 4)
                    {
                        servers.Add(new Server_entry(lines_split[0], lines_split[1], lines_split[2], lines_split[3]));
                        comboBox1.Items.Add(lines_split[0]);
                    }
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox3.PasswordChar = '\0';
            else
                textBox3.PasswordChar = '⚫';
        }
        static List<Server_entry> servers = new List<Server_entry>();
        public class Server_entry
        {
            public string name, ip, user_name, password;
            public Server_entry(string name, string ip, string user_name, string password)
            {
                this.name = name; this.ip = ip; this.user_name = user_name; this.password = password;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "")
            {
                servers.Add(new Server_entry(textBox4.Text, textBox1.Text, textBox2.Text, textBox3.Text));
                comboBox1.Items.Add(textBox4.Text);
                string[] lines = new string[servers.Count];
                for (ushort i = 0; i < lines.Length; i++)
                {
                    lines[i] = servers[i].name + "|" + servers[i].ip + "|" + servers[i].user_name + "|" + servers[i].password;
                }
                System.IO.File.WriteAllLines(Application.StartupPath + "\\mems.dat", lines);
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("Не заполнены поля!", "Ошибка");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Text = servers[comboBox1.SelectedIndex].ip;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            servers.RemoveAt(comboBox1.SelectedIndex);
            comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
            textBox5.Text = "";
            string[] lines = new string[servers.Count];
            for (ushort i = 0; i < lines.Length; i++)
            {
                lines[i] = servers[i].name + "|" + servers[i].ip + "|" + servers[i].user_name + "|" + servers[i].password;
            }
            System.IO.File.WriteAllLines(Application.StartupPath + "\\mems.dat", lines);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) return;
            textBox1.Text = servers[comboBox1.SelectedIndex].ip;
            textBox2.Text = servers[comboBox1.SelectedIndex].user_name;
            textBox3.Text = servers[comboBox1.SelectedIndex].password;
            textBox4.Text = servers[comboBox1.SelectedIndex].name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
            {
                if (!Auxiliary.CheckConnection()) { MessageBox.Show("Нет подключения к интернету!", "Ошибка"); return; }
                Program.ftp_client = new FTP("ftp://" + textBox1.Text + "/", textBox2.Text, textBox3.Text);
                string file = Program.ftp_client.download("FTP_UI_Updade_System.ini");
                if (file == null) { MessageBox.Show("Не удалось выполнить вход!", "Ошибка"); return; }
                string[] lines = Auxiliary.Get_lines_fffs(file);
                if (Auxiliary.Get_val_by_key(lines, "format")!= "FTP_FIwO1") { MessageBox.Show("Организация сервера имеет неподдерживаемый формат!", "Ошибка"); return; }
                if (!Auxiliary.Get_Equals_by_key(Program.SUPPORT_VERSIONS, Auxiliary.Get_val_by_key(lines, "version"))) { MessageBox.Show("Организация сервера имеет неподдерживаемую версию!", "Ошибка"); return; }
                Program.reserve_str = Auxiliary.Get_val_by_key(lines, "location");
                Program.answer = 1;
                Close();
            }
            else
            {
                MessageBox.Show("Не заполнены поля!", "Ошибка");
            }
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            if (!Auxiliary.CheckConnection()) { MessageBox.Show("Нет подключения к интернету!", "Ошибка"); }
        }
    }
}
