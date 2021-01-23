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
    public partial class Main_form : Form
    {
        OFC_FS[] filesys = new OFC_FS[0]; byte level_wm = 0, client_wm = 0;
        public Main_form()
        {
            InitializeComponent();
        }
        private void Main_form_Load(object sender, EventArgs e)
        {
            _context = System.Threading.SynchronizationContext.Current;
            comboBox1.SelectedIndex = 0;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Program.answer = 1;
            Close();
        }

        System.Threading.SynchronizationContext _context;
        private void th_download_prog(object mode)
        {
            if ((mode as string) == "0")
            {
                filesys[0].OFC_Fill_list_by_Array(Auxiliary.Get_lines_fffs(Program.ftp_client.download(Program.reserve_str + "\\progs.dat")));
                string[] names = filesys[0].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                _context.Post(s =>
                {
                    foreach (string i in names)
                    {
                        if (i == "UIv1")
                        {
                            listView1.Items.Add("UIv1", "UIv1.gif");
                        }
                        else
                        {
                            listView1.Items.Add(i, "app.gif");
                        }
                    }
                    textBox2.Text = "P";
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    button3.Enabled = true;
                }, null);
            }
            else if ((mode as string) == "1")
            {
                bool loaded = Program.ftp_client.upload(Program.reserve_str + "\\progs.dat", Auxiliary.Get_text_fffs(filesys[0].OFC_export_to_arr()));
                _context.Post(s =>
                {
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    button6.Enabled = true;
                    if (!loaded)MessageBox.Show("Не удалось выполнить загрузку файла \"progs.dat\" на сервер!\nПопробуйте повторить попытку позже.", "Ошибка");
                }, null);
            }
            else if ((mode as string) == "2")
            {
                filesys[0].OFC_Fill_list_by_Array(Auxiliary.Get_lines_fffs(Program.ftp_client.download(Program.reserve_str + "\\progs.dat")));
                filesys[2].OFC_Fill_list_by_Array(Auxiliary.Get_lines_fffs(Program.ftp_client.download(Program.reserve_str + "\\returns.dat")));
                filesys[1].OFC_Fill_list_by_Array(Auxiliary.Get_lines_fffs(Program.ftp_client.download(Program.reserve_str + "\\profiles.dat")));
                string[] names = filesys[1].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                _context.Post(s =>
                {
                    foreach (string i in names)
                    {
                        listView1.Items.Add(i, "user.gif");
                    }
                    textBox2.Text = "U";
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    button3.Enabled = true;
                }, null);
            }
            else if ((mode as string) == "3")
            {
                bool[] loaded = { 
                Program.ftp_client.upload(Program.reserve_str + "\\profiles.dat", Auxiliary.Get_text_fffs(filesys[1].OFC_export_to_arr())),
                Program.ftp_client.upload(Program.reserve_str + "\\returns.dat", Auxiliary.Get_text_fffs(filesys[2].OFC_export_to_arr())),
                };
                _context.Post(s =>
                {
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    button6.Enabled = true;
                    if (!loaded[0]) MessageBox.Show("Не удалось выполнить загрузку файла \"profiles.dat\" на сервер!\nПопробуйте повторить попытку позже.", "Ошибка");
                    if (!loaded[1]) MessageBox.Show("Не удалось выполнить загрузку файла \"returns.dat\" на сервер!\nПопробуйте повторить попытку позже.", "Ошибка");
                }, null);
            }
        }
        byte current_open = 0; //0 - Nothing, 1 - 
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                Array.Resize(ref filesys, 1);
                filesys[0] = new OFC_FS();
                client_wm = 0;
                listView1.Items.Clear();
                panel2.Visible = true;
                panel6.Visible = true;
                panel4.Visible = false;
                panel7.Visible = false;
                button6.Enabled = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                button3.Enabled = false;
                level_wm = 0;
                System.Threading.Thread mth = new System.Threading.Thread(th_download_prog);
                mth.Start("0");
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                Array.Resize(ref filesys, 4);
                filesys[0] = new OFC_FS(); //prog.dat
                filesys[1] = new OFC_FS(); //profiles.dat
                filesys[2] = new OFC_FS(); //returns.dat
                filesys[3] = null; //profiles.dat -> NFS
                client_wm = 1;
                listView1.Items.Clear();
                panel2.Visible = true;
                panel6.Visible = true;
                panel4.Visible = false;
                panel7.Visible = false;
                button6.Enabled = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                button3.Enabled = false;
                level_wm = 0;
                System.Threading.Thread mth = new System.Threading.Thread(th_download_prog);
                mth.Start("2");
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Array.Resize(ref filesys, 0);
                client_wm = 2;
                listView1.Items.Clear();
                panel2.Visible = true;
                panel6.Visible = true;
                panel4.Visible = false;
                panel7.Visible = true;
                button6.Enabled = false;
                level_wm = 0;
                source_list = Program.ftp_client.directoryListSimple(Program.reserve_str + "\\source");
                for (int i = 2; i < source_list.Length; i++)
                {
                    listView1.Items.Add(source_list[i].Remove(source_list[i].Length - 4), "ic_zip.gif");
                }
                textBox2.Text = "S";
            }
        }

        string[] source_list;
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (client_wm == 0)
            {
                if (level_wm == 0)
                {
                    if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
                    button1.Enabled = true;
                    panel6.Visible = false;
                    level_wm = 1;
                    textBox2.Text = "P\\" + listView1.SelectedItems[0].Text;
                    if (listView1.SelectedItems[0].Text == "UIv1")
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add("Обновления", "update.gif");
                        listView1.Items.Add("Конфигурация", "props.gif");
                        listView1.Items.Add("Версия", "prog.gif");
                        listView1.Items.Add("Сервера", "servers.gif");
                    }
                    else
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add("Обновления", "update.gif");
                        listView1.Items.Add("Конфигурация", "props.gif");
                        listView1.Items.Add("Версия", "prog.gif");
                    }
                }
                else if (level_wm == 1)
                {
                    if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
                    level_wm = 2;
                    textBox2.Text += "\\" + listView1.SelectedItems[0].Text;
                    panel2.Visible = false;
                    panel4.Visible = true;
                    switch (listView1.SelectedItems[0].Text)
                    {
                        case "Обновления":
                            rtb_way = textBox2.Text.Split('\\')[1];
                            rtb_name = "update";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[0].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Конфигурация":
                            rtb_way = textBox2.Text.Split('\\')[1];
                            rtb_name = "props";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[0].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Версия":
                            rtb_way = textBox2.Text.Split('\\')[1];
                            rtb_name = "prog";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[0].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Сервера":
                            rtb_way = textBox2.Text.Split('\\')[1];
                            rtb_name = "UI_servers";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Text = Encryption.DecryptStringFromBytes_Aes(Encoding.Default.GetBytes(Auxiliary.Get_text_from_lines(filesys[0].OFC_FileData_read(rtb_way, rtb_name))), Encryption.Encryption_KEY, Encryption.Encryption_IV);
                            break;
                    }
                }
            }
            else if (client_wm == 1)
            {
                if (level_wm == 0)
                {
                    if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
                    button1.Enabled = true;
                    level_wm = 1;
                    filesys[3] = new OFC_FS();
                    filesys[3].OFC_Fill_list_by_Array(filesys[1].OFC_FileData_read(listView1.SelectedItems[0].Text, "user"));
                    textBox2.Text = "U\\" + listView1.SelectedItems[0].Text;
                    string[] names = filesys[3].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                    listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        if (i == "UIv1")
                        {
                            listView1.Items.Add("UIv1", "UIv1.gif");
                        }
                        else
                        {
                            listView1.Items.Add(i, "app.gif");
                        }
                    }
                }
                else if (level_wm == 1)
                {
                    if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
                    level_wm = 2;
                    textBox2.Text += "\\" + listView1.SelectedItems[0].Text;
                    string[] names = filesys[3].OFC_get_names(listView1.SelectedItems[0].Text, OFC_FS.OFC_Types.File);
                    if (filesys[2].OFC_Entry_exist(textBox2.Text.Split('\\')[1], listView1.SelectedItems[0].Text, OFC_FS.OFC_Types.File))
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add("Возвраты", "return.gif");
                    }
                    else
                        listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        if (i == "commands")
                        {
                            listView1.Items.Add("Команды", "command.gif");
                        }
                        else if (i == "config")
                        {
                            listView1.Items.Add("Настройки", "props.gif");
                        }
                        else if (i == "log")
                        {
                            listView1.Items.Add("Журнал", "journal.gif");
                        }
                    }
                }
                else if (level_wm == 2)
                {
                    if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
                    level_wm = 3;
                    textBox2.Text += "\\" + listView1.SelectedItems[0].Text;
                    panel2.Visible = false;
                    panel4.Visible = true;
                    panel6.Visible = false;
                    switch (listView1.SelectedItems[0].Text)
                    {
                        case "Команды":
                            rtb_way = textBox2.Text.Split('\\')[2];
                            rtb_name = "commands";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[3].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Настройки":
                            rtb_way = textBox2.Text.Split('\\')[2];
                            rtb_name = "config";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[3].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Журнал":
                            rtb_way = textBox2.Text.Split('\\')[2];
                            rtb_name = "log";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[3].OFC_FileData_read(rtb_way, rtb_name);
                            break;
                        case "Возвраты":
                            rtb_way = textBox2.Text.Split('\\')[2];
                            rtb_name = "returns";
                            richTextBox1.Select(0, richTextBox1.Text.Length);
                            richTextBox1.SelectionColor = Color.Black;
                            richTextBox1.Lines = filesys[2].OFC_FileData_read(textBox2.Text.Split('\\')[1], textBox2.Text.Split('\\')[2]);
                            break;
                    }
                }
            }
        }
        string rtb_way, rtb_name;
        private void button1_Click(object sender, EventArgs e)
        {
            if (client_wm == 0)
            {
                if (level_wm == 1)
                {
                    panel6.Visible = true;
                    button1.Enabled = false;
                    level_wm = 0;
                    string[] names = filesys[0].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                    textBox2.Text = "P";
                    listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        if (i == "UIv1")
                        {
                            listView1.Items.Add("UIv1", "UIv1.gif");
                        }
                        else
                        {
                            listView1.Items.Add(i, "app.gif");
                        }
                    }
                }
                else if (level_wm == 2)
                {
                    level_wm = 1;
                    textBox2.Text = "P\\" + textBox2.Text.Split('\\')[1];
                    panel2.Visible = true;
                    panel4.Visible = false;
                    source_list = null;
                }
            }
            else if (client_wm == 1)
            {
                if (level_wm == 1)
                {
                    button1.Enabled = false;
                    panel6.Visible = true;
                    level_wm = 0;
                    filesys[3] = null;
                    textBox2.Text = "U";
                    string[] names = filesys[1].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                    listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        listView1.Items.Add(i, "user.gif");
                    }
                    source_list = null;
                }
                else if (level_wm == 2)
                {
                    level_wm = 1;
                    textBox2.Text = "U\\" + textBox2.Text.Split('\\')[1];
                    string[] names = filesys[3].OFC_get_names("", OFC_FS.OFC_Types.Folder);
                    listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        if (i == "UIv1")
                        {
                            listView1.Items.Add("UIv1", "UIv1.gif");
                        }
                        else
                        {
                            listView1.Items.Add(i, "app.gif");
                        }
                    }
                }
                else if (level_wm == 3)
                {
                    level_wm = 2;
                    string[] way = new string[2] { textBox2.Text.Split('\\')[1], textBox2.Text.Split('\\')[2] };
                    textBox2.Text = "U\\" + way[0] + "\\" + way[1];
                    string[] names = filesys[3].OFC_get_names(way[1], OFC_FS.OFC_Types.File);
                    panel6.Visible = true;
                    panel2.Visible = true;
                    panel4.Visible = false;
                    if (filesys[2].OFC_Entry_exist(way[0], way[1], OFC_FS.OFC_Types.File))
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add("Возвраты", "return.gif");
                    }
                    else
                        listView1.Items.Clear();
                    foreach (string i in names)
                    {
                        if (i == "commands")
                        {
                            listView1.Items.Add("Команды", "command.gif");
                        }
                        else if (i == "config")
                        {
                            listView1.Items.Add("Настройки", "props.gif");
                        }
                        else if (i == "log")
                        {
                            listView1.Items.Add("Журнал", "journal.gif");
                        }
                    }
                }
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.SelectedText, TextDataFormat.Text);
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text)) richTextBox1.SelectedText = Clipboard.GetText(TextDataFormat.Text);
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.SelectedText, TextDataFormat.Text);
            richTextBox1.SelectedText = "";
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "";
        }

        byte rtb_timer_num = 0;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rtb_timer_num = 0;
                tbtimer.Start();
            }
        }

        private void tbtimer_Tick(object sender, EventArgs e)
        {
            if (rtb_timer_num == 1)
            {
                ushort[] select = new ushort[2] { (ushort)richTextBox1.SelectionStart, (ushort)richTextBox1.SelectionLength };
                if (rtb_name == "update")
                {
                    ushort str_len = 0;
                    for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                    {
                        string[] key_val = richTextBox1.Lines[i].Split('=');
                        if (key_val.Length == 2)
                        {
                            DateTime test;
                            if (key_val[0] == "type" && (key_val[1] == "standart" || key_val[1] == "important" || key_val[1] == "optional"))
                            {
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkGreen;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else if (key_val[0] == "date" && (DateTime.TryParse(key_val[1], out test)))
                            {
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkGreen;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else
                            {
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkRed;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkRed;
                            }
                            richTextBox1.Select(str_len + key_val[0].Length, 1);
                            richTextBox1.SelectionColor = Color.Blue;
                        }
                        else
                        {
                            richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                            richTextBox1.SelectionColor = Color.Gray;
                        }
                        str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                    }
                }
                else if (rtb_name == "props")
                {
                    if (rtb_way == "UIv1")
                    {
                        ushort str_len = 0;
                        for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            string[] key_val = richTextBox1.Lines[i].Split('=');
                            if (key_val.Length == 2)
                            {
                                if ((key_val[0] == "config" || key_val[0] == "log") && (key_val[1] == "yes" || key_val[1] == "no"))
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkGreen;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkOrange;
                                }
                                else
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                }
                                richTextBox1.Select(str_len + key_val[0].Length, 1);
                                richTextBox1.SelectionColor = Color.Blue;
                            }
                            else
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.Gray;
                            }
                            str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                        }
                    }
                    else
                    {
                        ushort str_len = 0;
                        for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            string[] key_val = richTextBox1.Lines[i].Split('=');
                            if (key_val.Length == 2)
                            {
                                if ((key_val[0] == "install" || key_val[0] == "start" || key_val[0] == "stop" || key_val[0] == "uninstall" || key_val[0] == "config" || key_val[0] == "commands" || key_val[0] == "retdata" || key_val[0] == "log") && (key_val[1] == "yes" || key_val[1] == "no"))
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkGreen;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkOrange;
                                }
                                else
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                }
                                richTextBox1.Select(str_len + key_val[0].Length, 1);
                                richTextBox1.SelectionColor = Color.Blue;
                            }
                            else
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.Gray;
                            }
                            str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                        }
                    }
                }
                else if (rtb_name == "prog")
                {
                    ushort str_len = 0;
                    for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                    {
                        string[] key_val = richTextBox1.Lines[i].Split('=');
                        if (key_val.Length == 2)
                        {
                            float test;
                            if (key_val[0] == "version" && float.TryParse(key_val[1], out test))
                            {
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkGreen;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else if (key_val[0] == "location" && Check_source_folder(key_val[1]+".zip"))
                            {
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkGreen;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else if (key_val[0] == "code" && key_val[1] != "")
                            {
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkGreen;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else
                            {
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                                richTextBox1.Select(str_len, key_val[0].Length);
                                richTextBox1.SelectionColor = Color.DarkRed;
                                richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                richTextBox1.SelectionColor = Color.DarkRed;
                            }
                            richTextBox1.Select(str_len + key_val[0].Length, 1);
                            richTextBox1.SelectionColor = Color.Blue;
                        }
                        else
                        {
                            richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                            richTextBox1.SelectionColor = Color.Gray;
                        }
                        str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                    }

                }
                else if (rtb_name == "UI_servers")
                {
                    ushort str_len = 0;
                    for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                    {
                        string[] key_val = richTextBox1.Lines[i].Split('|');
                        if (key_val.Length == 4)
                        {
                            System.Net.IPAddress test; ushort test2;
                            if (ushort.TryParse(key_val[0], out test2) && System.Net.IPAddress.TryParse(key_val[1], out test) && key_val[2] != "" && key_val[3] != "")
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.DarkOrange;
                            }
                            else
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.DarkRed;
                            }
                        }
                        else
                        {
                            richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                            richTextBox1.SelectionColor = Color.Gray;
                        }
                        str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                    }
                }
                else if (rtb_name == "commands")
                {
                    ushort str_len = 0;
                    for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                    {
                        if (richTextBox1.Lines[i] == "[always]" || richTextBox1.Lines[i] == "[alwayso]" || richTextBox1.Lines[i] == "[once]")
                        {
                            richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                            richTextBox1.SelectionColor = Color.Blue;
                        }
                        else
                        {
                            richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                            richTextBox1.SelectionColor = Color.DarkOrange;
                        }
                        str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                    }
                }
                else if (rtb_name == "config")
                {
                    if (rtb_way == "UIv1")
                    {
                        ushort str_len = 0;
                        for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            string[] key_val = richTextBox1.Lines[i].Split('=');
                            if (key_val.Length == 2)
                            {
                                if ((key_val[0] == "update") && (key_val[1] == "allow" || key_val[1] == "deny"))
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkGreen;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkOrange;
                                }
                                else
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                }
                                richTextBox1.Select(str_len + key_val[0].Length, 1);
                                richTextBox1.SelectionColor = Color.Blue;
                            }
                            else
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.Gray;
                            }
                            str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                        }
                    }
                    else
                    {
                        ushort str_len = 0;
                        for (ushort i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            string[] key_val = richTextBox1.Lines[i].Split('=');
                            if (key_val.Length == 2)
                            {
                                if ((key_val[0] == "update" && (key_val[1] == "allow" || key_val[1] == "deny")) || (key_val[0] == "status" && (key_val[1] == "active" || key_val[1] == "deactive")))
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkGreen;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkOrange;
                                }
                                else
                                {
                                    richTextBox1.Select(str_len, key_val[0].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                    richTextBox1.Select(str_len + key_val[0].Length + 1, key_val[1].Length);
                                    richTextBox1.SelectionColor = Color.DarkRed;
                                }
                                richTextBox1.Select(str_len + key_val[0].Length, 1);
                                richTextBox1.SelectionColor = Color.Blue;
                            }
                            else
                            {
                                richTextBox1.Select(str_len, richTextBox1.Lines[i].Length);
                                richTextBox1.SelectionColor = Color.Gray;
                            }
                            str_len += (ushort)(richTextBox1.Lines[i].Length + 1);
                        }
                    }
                }
                richTextBox1.Select(select[0], select[1]);
                tbtimer.Stop();
            }
            else
                rtb_timer_num = 1;
        }
        bool Check_source_folder(string name)
        {
            if (!checkBox2.Checked) return true;
            if (source_list == null) return false;
            for (int i = 0; i < source_list.Length; i++)
            {
                if (name == source_list[i]) return true;
            }
            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (rtb_name == "prog")
            {
                source_list = Program.ftp_client.directoryListSimple(Program.reserve_str + "\\source");
                if (source_list.Length == 2)
                {
                    Array.Resize(ref source_list, 0);
                }
                else if (source_list.Length == 3)
                {
                    source_list[0] = source_list[2];
                    Array.Resize(ref source_list, 1);
                }
                else
                {
                    source_list[0] = source_list[source_list.Length - 1];
                    source_list[1] = source_list[source_list.Length - 2];
                    Array.Resize(ref source_list, source_list.Length - 2);
                }
                if (checkBox1.Checked)
                {
                    rtb_timer_num = 0;
                    tbtimer.Start();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rtb_timer_num = 0;
                tbtimer.Start();
            }
            else
            {
                ushort[] select = new ushort[2] { (ushort)richTextBox1.SelectionStart, (ushort)richTextBox1.SelectionLength };
                richTextBox1.Select(0, richTextBox1.Text.Length);
                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.Select(select[0], select[1]);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            rtb_timer_num = 0;
            tbtimer.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (client_wm == 0)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                button6.Enabled = false;
                System.Threading.Thread mth = new System.Threading.Thread(th_download_prog);
                mth.Start("1");
            }
            else if (client_wm == 1)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                button6.Enabled = false;
                System.Threading.Thread mth = new System.Threading.Thread(th_download_prog);
                mth.Start("3");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (rtb_name == "update" || rtb_name == "props" || rtb_name == "prog" || rtb_name == "UI_servers")
            {
                if (rtb_name == "UI_servers")
                {
                    filesys[0].OFC_FileData_write(rtb_way, "UI_servers", new string[] { Encoding.Default.GetString(Encryption.EncryptStringToBytes_Aes(richTextBox1.Text, Encryption.Encryption_KEY, Encryption.Encryption_IV)) });
                }
                else
                    filesys[0].OFC_FileData_write(rtb_way, rtb_name, richTextBox1.Lines);
            }
            else if (rtb_name == "commands" || rtb_name == "config" || rtb_name == "log")
            {
                filesys[3].OFC_FileData_write(textBox2.Text.Split('\\')[2], rtb_name, richTextBox1.Lines);
                filesys[1].OFC_FileData_write(textBox2.Text.Split('\\')[1], "user", filesys[3].OFC_export_to_arr());
            }
            else if (rtb_name == "returns")
            {
                filesys[2].OFC_FileData_write(textBox2.Text.Split('\\')[1], textBox2.Text.Split('\\')[2], richTextBox1.Lines);
            }
        }

        Help h_form;
        private void button8_Click(object sender, EventArgs e)
        {
            if (h_form != null) h_form.Close();
            h_form = new Help();
            if (rtb_name == "update")
            {
                h_form.richTextBox1.Rtf = Properties.Resources.help_3;
            }
            else if (rtb_name == "props")
            {
                if (rtb_way == "UIv1")
                {
                    h_form.richTextBox1.Rtf = Properties.Resources.help_4;
                }
                else
                {
                    h_form.richTextBox1.Rtf = Properties.Resources.help_1;
                }
            }
            else if (rtb_name == "prog")
            {
                h_form.richTextBox1.Rtf = Properties.Resources.help_2;
            }
            else if (rtb_name == "UI_servers")
            {
                h_form.richTextBox1.Rtf = Properties.Resources.help_5;
            }
            else if (rtb_name == "commands")
            {
                h_form.richTextBox1.Rtf = Properties.Resources.help_8;
            }
            else if (rtb_name == "config")
            {
                if (rtb_way == "UIv1")
                {
                    h_form.richTextBox1.Rtf = Properties.Resources.help_7;
                }
                else
                {
                    h_form.richTextBox1.Rtf = Properties.Resources.help_6;
                }
            }
            h_form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (client_wm == 0)
            {
                if (level_wm == 0)
                {
                    Enter_fname form = new Enter_fname()
                    {
                        fs = filesys[0],
                        current_way = "",
                        text = "Введите имя программы:"
                    };
                    form.ShowDialog();
                    if (form.DialogResult == DialogResult.OK)
                    {
                        filesys[0].OFC_Make_entry(form.current_way, "update", OFC_FS.OFC_Types.File, new string[] { "type=standart", "date=" + string.Format("{0:d} {0:t}", DateTime.Now) });
                        filesys[0].OFC_Make_entry(form.current_way, "props", OFC_FS.OFC_Types.File, new string[] { "install=yes", "start=yes", "stop=yes", "uninstall=yes", "config=yes", "commands=yes", "retdata=yes", "log=yes" });
                        filesys[0].OFC_Make_entry(form.current_way, "prog", OFC_FS.OFC_Types.File, new string[] { "version=1,0", "location={location}", "code={code}" });
                        listView1.Items.Add(form.current_way, "app.gif");
                    }

                }
            }
            else if (client_wm == 1)
            {
                if (level_wm == 0)
                {
                    Enter_fname form = new Enter_fname()
                    {
                        fs = filesys[1],
                        current_way = "",
                        text = "Введите ID пользователя:"
                    };
                    form.ShowDialog();
                    if (form.DialogResult == DialogResult.OK)
                    {
                        filesys[3] = new OFC_FS();
                        filesys[3].OFC_Make_entry("UIv1", "config", OFC_FS.OFC_Types.File, new string[] { "update=allow" });
                        filesys[3].OFC_Make_entry("UIv1", "commands", OFC_FS.OFC_Types.File, new string[] { "" });
                        filesys[3].OFC_Make_entry("UIv1", "log", OFC_FS.OFC_Types.File, new string[] { "" });
                        filesys[2].OFC_Make_entry("", form.current_way, OFC_FS.OFC_Types.Folder);
                        filesys[1].OFC_Make_entry(form.current_way, "user", OFC_FS.OFC_Types.File, filesys[3].OFC_export_to_arr());
                        filesys[3] = null;
                        listView1.Items.Add(form.current_way, "user.gif");
                    }

                }
                else if (level_wm == 1)
                {
                    Enter_fname form = new Enter_fname()
                    {
                        fs = filesys[3],
                        current_way = "",
                        text = "Введите имя программы:"
                    };
                    form.ShowDialog();
                    if (form.DialogResult == DialogResult.OK)
                    {
                        if (!filesys[0].OFC_Entry_exist("", form.current_way, OFC_FS.OFC_Types.Folder))
                        {
                            MessageBox.Show("Программа \"" + form.current_way + "\" отсутствует!", "Ошибка"); return;
                        }
                        string[] config = filesys[0].OFC_FileData_read(form.current_way, "props");
                        if (Auxiliary.Get_val_by_key(config, "config") == "yes")
                        {
                            filesys[3].OFC_Make_entry(form.current_way, "config", OFC_FS.OFC_Types.File, new string[] { "status=active", "update=allow" });
                        }
                        if (Auxiliary.Get_val_by_key(config, "commands") == "yes")
                        {
                            filesys[3].OFC_Make_entry(form.current_way, "commands", OFC_FS.OFC_Types.File);
                        }
                        if (Auxiliary.Get_val_by_key(config, "log") == "yes")
                        {
                            filesys[3].OFC_Make_entry(form.current_way, "log", OFC_FS.OFC_Types.File);
                        }
                        if (Auxiliary.Get_val_by_key(config, "retdata") == "yes")
                        {
                            filesys[2].OFC_Make_entry(textBox2.Text.Split('\\')[1], form.current_way, OFC_FS.OFC_Types.File);
                        }
                        listView1.Items.Add(form.current_way, "app.gif");
                        filesys[1].OFC_FileData_write(textBox2.Text.Split('\\')[1], "user", filesys[3].OFC_export_to_arr());
                    }
                }
                else if (level_wm == 2)
                {
                    Enter_file form = new Enter_file();
                    string[] way = new string[2] { textBox2.Text.Split('\\')[1], textBox2.Text.Split('\\')[2] };
                    foreach (string i in filesys[3].OFC_get_names(way[1], OFC_FS.OFC_Types.File))
                    {
                        if (i == "config")
                        {
                            form.a_files[0] = false;
                        }
                        else if (i == "commands")
                        {
                            form.a_files[1] = false;
                        }
                        else if (i == "log")
                        {
                            form.a_files[2] = false;
                        }
                    }
                    if (filesys[2].OFC_Entry_exist(way[0], way[1], OFC_FS.OFC_Types.File))
                    {
                        form.a_files[3] = false;
                    }
                    form.ShowDialog();
                    if (form.DialogResult == DialogResult.OK)
                    {
                        if (form.answer == 0)
                        {
                            if (way[1] == "UIv1")
                            {
                                filesys[3].OFC_Make_entry(way[1], "config", OFC_FS.OFC_Types.File, new string[] { "update=allow" });
                            }
                            else
                            {
                                filesys[3].OFC_Make_entry(way[1], "config", OFC_FS.OFC_Types.File, new string[] { "status=active", "update=allow" });
                            }
                            filesys[1].OFC_FileData_write(way[0], "user", filesys[3].OFC_export_to_arr());
                            listView1.Items.Add("Настройки", "props.gif");
                        }
                        else if (form.answer == 1)
                        {
                            filesys[3].OFC_Make_entry(way[1], "commands", OFC_FS.OFC_Types.File, new string[] { "" });
                            filesys[1].OFC_FileData_write(way[0], "user", filesys[3].OFC_export_to_arr());
                            listView1.Items.Add("Команды", "command.gif");
                        }
                        else if (form.answer == 2)
                        {
                            filesys[3].OFC_Make_entry(way[1], "log", OFC_FS.OFC_Types.File, new string[] { "" });
                            filesys[1].OFC_FileData_write(way[0], "user", filesys[3].OFC_export_to_arr());
                            listView1.Items.Add("Журнал", "journal.gif");
                        }
                        else if (form.answer == 3)
                        {
                            filesys[2].OFC_Make_entry(way[0], way[1], OFC_FS.OFC_Types.File, new string[] { "" });
                            listView1.Items.Add("Возвраты", "return.gif");
                        }
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) { MessageBox.Show("Выберите один элемент!", "Ошибка"); return; }
            Enter_name form = new Enter_name();
            form.current_name = listView1.SelectedItems[0].Text;
            form.f_list = new string[listView1.Items.Count];
            for (ushort i = 0; i < listView1.Items.Count; i++)
            {
                form.f_list[i] = listView1.Items[i].Text;
            }
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (Program.ftp_client.rename(Program.reserve_str + "\\source\\" + listView1.SelectedItems[0].Text + ".zip", form.current_name + ".zip"))
                    listView1.SelectedItems[0].Text = form.current_name;
                else
                    MessageBox.Show("Не удалось выполнить переименование файла \"" + listView1.SelectedItems[0].Text + ".zip\".", "Ошибка");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { MessageBox.Show("Выберите элементы!", "Ошибка"); return; }
            if (MessageBox.Show("Вы уверены, что хотите удалить выбранные программы?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
            string[] arr = new string[0];
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                if (Program.ftp_client.deleteFile(Program.reserve_str + "\\source\\" + i.Text + ".zip"))
                    i.Remove();
                else
                {
                    Array.Resize(ref arr, arr.Length + 1);
                    arr[arr.Length - 1] = i.Text + ".zip";
                }   
            }
            if (arr.Length > 0) MessageBox.Show("Не удалось выполнить удаление следующих файлов: " + Auxiliary.Construct_str_by_file_arr(arr) + ".", "Ошибка");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileD.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem i in listView1.Items)
                {
                    foreach (string i1 in openFileD.SafeFileNames)
                    {
                        if (i.Text + ".zip" == i1)
                        {
                            MessageBox.Show("Файл \"" + i1 + "\" уже существует!", "Ошибка");
                            return;
                        }
                    }
                }
                progressBar1.Style = ProgressBarStyle.Marquee;
                System.Threading.Thread mth = new System.Threading.Thread(upload_file);
                mth.Start(new UpDownStruct(openFileD.SafeFileNames, openFileD.FileNames));
            }
        }
        private void upload_file(object o)
        {
            UpDownStruct us = o as UpDownStruct;
            string[] arr = new string[0];
            bool[] arr1 = new bool[openFileD.FileNames.Length];
            for (ushort i = 0; i < openFileD.FileNames.Length; i++)
            {
                arr1[i] = true;
                if (!Program.ftp_client.file_upload(Program.reserve_str + "\\source\\" + us.snames[i], us.dnames[i]))
                {
                    arr1[i] = false;
                    Array.Resize(ref arr, arr.Length + 1);
                    arr[arr.Length - 1] = us.snames[i];
                }
            }
            _context.Post(s =>
            {
                for (ushort i = 0; i < openFileD.FileNames.Length; i++)
                {
                    if (arr1[i])
                    listView1.Items.Add(us.snames[i].Remove(us.snames[i].Length - 4), "ic_zip.gif");
                }
                progressBar1.Style = ProgressBarStyle.Blocks;
                if (arr.Length > 0) MessageBox.Show("Не удалось выполнить загрузку следующих файлов: " + Auxiliary.Construct_str_by_file_arr(arr) + ".", "Ошибка");
            }, null);
        }
        private void download_file(object o)
        {
            UpDownStruct us = o as UpDownStruct;
            string[] arr = new string[0];
            for (ushort i = 0; i < us.snames.Length; i++)
            {
                if (!Program.ftp_client.file_download(us.snames[i], us.dnames[i]))
                {
                    Array.Resize(ref arr, arr.Length+1);
                    arr[arr.Length - 1] = us.snames[i];
                }
            }
            _context.Post(s =>
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                if (arr.Length > 0) MessageBox.Show("Не удалось выполнить скачивание следующих файлов: " + Auxiliary.Construct_str_by_file_arr(arr) + ".", "Ошибка");
            }, null);
        }
        class UpDownStruct
        {
            public string[] snames, dnames;
            public UpDownStruct(string[] snames, string[] dnames)
            {
                this.snames = snames; this.dnames = dnames;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { MessageBox.Show("Выберите элементы!", "Ошибка"); return; }
            if (listView1.SelectedItems.Count == 1)
            {
                if (saveFileD.ShowDialog() == DialogResult.OK)
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    System.Threading.Thread mth = new System.Threading.Thread(download_file);
                    mth.Start(new UpDownStruct(new string[] { Program.reserve_str + "\\source\\" + listView1.SelectedItems[0].Text + ".zip" }, new string[] { saveFileD.FileName }));
                }
            }
            else
            {
                if (folderBrowserD.ShowDialog() == DialogResult.OK)
                {
                    string[] n = new string[listView1.SelectedItems.Count], d = new string[listView1.SelectedItems.Count];
                    for (ushort i = 0; i < n.Length; i++)
                    {
                        n[i] = Program.reserve_str + "\\source\\" + listView1.SelectedItems[i] + ".zip";
                        d[i] = folderBrowserD.SelectedPath + "\\" + listView1.SelectedItems[i] + ".zip";
                    }
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    System.Threading.Thread mth = new System.Threading.Thread(download_file);
                    mth.Start(new UpDownStruct(n, d));
                }
            }
        }

        private void Main_form_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (client_wm == 0)
            {
                if (level_wm == 0)
                {
                    if (listView1.SelectedItems.Count == 0) { MessageBox.Show("Выберите элементы!", "Ошибка"); return; }
                    foreach (ListViewItem i in listView1.SelectedItems) { if (i.Text == "UIv1") { MessageBox.Show("Нельзя удалить программу UIv1!", "Ошибка"); return; } }
                    if (MessageBox.Show("Вы уверены, что хотите удалить выбранные программы?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                    foreach (ListViewItem i in listView1.SelectedItems)
                    {
                        filesys[0].OFC_Delete_entry("", i.Text, OFC_FS.OFC_Types.Folder, false);
                        i.Remove();
                    }
                }
            }
            else if (client_wm == 1)
            {
                if (level_wm == 0)
                {
                    if (listView1.SelectedItems.Count == 0) { MessageBox.Show("Выберите элементы!", "Ошибка"); return; }
                    if (MessageBox.Show("Вы уверены, что хотите удалить выбранных пользователей?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                    foreach (ListViewItem i in listView1.SelectedItems)
                    {
                        filesys[1].OFC_Delete_entry("", i.Text, OFC_FS.OFC_Types.Folder, false);
                        filesys[2].OFC_Delete_entry("", i.Text, OFC_FS.OFC_Types.Folder, false);
                        i.Remove();
                    }
                }
                else if (level_wm == 1)
                {
                    foreach (ListViewItem i in listView1.SelectedItems) { if (i.Text == "UIv1") { MessageBox.Show("Нельзя удалить программу UIv1!", "Ошибка"); return; } }
                    if (MessageBox.Show("Вы уверены, что хотите удалить выбранные программы?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                    foreach (ListViewItem i in listView1.SelectedItems)
                    {
                        filesys[3].OFC_Delete_entry("", i.Text, OFC_FS.OFC_Types.Folder, false);
                        filesys[2].OFC_Delete_entry(textBox2.Text.Split('\\')[1], i.Text, OFC_FS.OFC_Types.Folder, true);
                        i.Remove();
                    }
                    filesys[1].OFC_FileData_write(textBox2.Text.Split('\\')[1], "user", filesys[3].OFC_export_to_arr());
                }
                else if (level_wm == 2)
                {
                    if (listView1.SelectedItems.Count == 0) { MessageBox.Show("Выберите элементы!", "Ошибка"); return; }
                    if (MessageBox.Show("Вы уверены, что хотите удалить выбранные элементы?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                    foreach (ListViewItem i in listView1.SelectedItems)
                    {
                        switch (i.Text)
                        {
                            case "Команды":
                                filesys[3].OFC_Delete_entry(textBox2.Text.Split('\\')[2], "commands", OFC_FS.OFC_Types.File, true);
                                break;
                            case "Настройки":
                                filesys[3].OFC_Delete_entry(textBox2.Text.Split('\\')[2], "config", OFC_FS.OFC_Types.File, true);
                                break;
                            case "Журнал":
                                filesys[3].OFC_Delete_entry(textBox2.Text.Split('\\')[2], "log", OFC_FS.OFC_Types.File, true);
                                break;
                            case "Возвраты":
                                filesys[2].OFC_Delete_entry(textBox2.Text.Split('\\')[1], textBox2.Text.Split('\\')[2], OFC_FS.OFC_Types.File, true);
                                break;
                        }
                        i.Remove();
                    }
                    filesys[1].OFC_FileData_write(textBox2.Text.Split('\\')[1], "user", filesys[3].OFC_export_to_arr());
                }
            }
        }
        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
}
