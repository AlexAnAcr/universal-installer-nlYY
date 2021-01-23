using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace UniversalInstaller
{
    static class Auxiliary
    {
        public static Variables.Server_entry Get_server_by_id(ushort server_id)
        {
            foreach (Variables.Server_entry i in Variables.servers)
            {
                if (i.number == server_id)
                {
                    return i;
                }
            }
            return null;
        }
        public static bool Get_Equals_by_key(string[] lines, string key)
        {
            for (ushort i = 0; i < lines.Length; i++)
            {
                if (lines[i] == key)
                {
                    return true;
                }
            }
            return false;
        }
        public static string Get_val_by_key(string[] lines, string key)
        {
            for (ushort i = 0; i < lines.Length; i++)
            {
                string[] split_arr = Split_key_val(lines[i]);
                if (split_arr[0] == key)
                {
                    return split_arr[1];
                }
            }
            return null;
        }
        public static string[] Split_key_val(string key_val)
        {
            string[] val = key_val.Split('=');
            if (val.Length != 2)
            {
                val = new string[2] { "", "" };
            }
            return val;
        }
        public static string[] Get_lines_fffs(string text)
        {
            string[] lines = text.Split('\n');
            Array.Resize(ref lines, lines.Length - 1);
            for (ushort i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();
            }
            return lines;
        }
        public static string Get_text_fffs(string[] lines)
        {
            string text = "";
            for (ushort i = 0; i < lines.Length; i++) { text += lines[i] + "\r\n"; }
            return text;
        }
        public static string Get_text_from_lines(string[] lines)
        {
            string text = "";
            for (ushort i = 0; i < lines.Length; i++) { text += lines[i] + "\n"; }
            return text.Remove(text.Length - 1, 1);
        }
        public static string[] Get_lines_from_text(string text)
        {
            return text.Split('\n');
        }

        [DllImport("wininet.dll")]
        static extern bool InternetGetConnectedState(ref int lpdwFlags, int dwReserved);
        public static bool CheckConnection()
        {
            try
            {
                int flags = 0x40;
                bool checkStatus = InternetGetConnectedState(ref flags, 0);
                if (checkStatus)
                    return PingServer("google.com");
                return false;
            }
            catch
            {
                return false;
            }
        }
        static bool PingServer(string serverList)
        {
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(serverList);
            bool haveAnInternetConnection = (pingReply.Status == IPStatus.Success);
            return haveAnInternetConnection;
        }

        static System.Security.Cryptography.RNGCryptoServiceProvider crypto_rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
        public static byte Generate_random(byte max_val)
        {
            byte[] b1 = new byte[1];
            crypto_rand.GetBytes(b1);
            byte result = (byte)Math.Round((float)(b1[0] * max_val / 256), MidpointRounding.ToEven);
            return result;
        }
        public static char Get_sumvol_by_number(byte num)
        {
            switch (num)
            {
                case 0: return '0';
                case 1: return '1';
                case 2: return '2';
                case 3: return '3';
                case 4: return '4';
                case 5: return '5';
                case 6: return '6';
                case 7: return '7';
                case 8: return '8';
                case 9: return '9';
                case 10: return 'A';
                case 11: return 'a';
                case 12: return 'B';
                case 13: return 'b';
                case 14: return 'C';
                case 15: return 'c';
                case 16: return 'D';
                case 17: return 'b';
                case 18: return 'E';
                case 19: return 'e';
                case 20: return 'F';
                case 21: return 'f';
                case 22: return 'G';
                case 23: return 'g';
                case 24: return 'H';
                case 25: return 'h';
                case 26: return 'I';
                case 27: return 'i';
                case 28: return 'J';
                case 29: return 'j';
                case 30: return 'K';
                case 31: return 'k';
                case 32: return 'L';
                case 33: return 'l';
                case 34: return 'M';
                case 35: return 'm';
                case 36: return 'N';
                case 37: return 'n';
                case 38: return 'O';
                case 39: return 'o';
                case 40: return 'P';
                case 41: return 'p';
                case 42: return 'Q';
                case 43: return 'q';
                case 44: return 'R';
                case 45: return 'r';
                case 46: return 'S';
                case 47: return 's';
                case 48: return 'T';
                case 49: return 't';
                case 50: return 'U';
                case 51: return 'u';
                case 52: return 'V';
                case 53: return 'v';
                case 54: return 'W';
                case 55: return 'w';
                case 56: return 'X';
                case 57: return 'x';
                case 58: return 'Y';
                case 59: return 'y';
                case 60: return 'Z';
                case 61: return 'z';
            }
            return '-';
        }
        public static string Generate_prog_key()
        {
            string result = "";
            for (int i = 0; i < 4; i++)
                result += Get_sumvol_by_number(Generate_random(61));
            result += "-";
            for (int i = 0; i < 4; i++)
                result += Get_sumvol_by_number(Generate_random(61));
            return result;
        }
    }
    static class Connect
    {
        public static Check_conneck_state Check_connection()
        {
            connect_checker cc = new connect_checker();
            return cc.Check();
        }
        public enum Check_conneck_state
        {
            NoAnswer, NoConnect, Connected
        }
        class connect_checker
        {
            System.Threading.Thread check_conn_th;
            System.Timers.Timer check_conn_timer = new System.Timers.Timer(100);
            byte check_conn_th_v = 0, tick = 0; ushort time_wait = 0;
            static System.Threading.EventWaitHandle _waitHandle = new System.Threading.AutoResetEvent(false);
            void Check_conn_th_f()
            {
                if (Auxiliary.CheckConnection())
                {
                    check_conn_th_v = 2;
                }
                else
                {
                    check_conn_th_v = 1;
                }
            }
            void Check_conn_timer_elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                tick++;
                if (check_conn_th_v != 0)
                {
                    check_conn_timer.Stop();
                    current = (check_conn_th_v == 2 ? Check_conneck_state.Connected : Check_conneck_state.NoConnect);
                    _waitHandle.Set();
                }
                else if (tick == 10)
                {
                    if (time_wait >= Variables.time_func[0])
                    {
                        check_conn_timer.Stop();
                        current = Check_conneck_state.NoAnswer;
                        _waitHandle.Set();
                    }
                    tick = 0;
                    time_wait += 1;
                }
            }

            Check_conneck_state current;
            public Check_conneck_state Check()
            {
                check_conn_timer.Elapsed += new System.Timers.ElapsedEventHandler(Check_conn_timer_elapsed);
                check_conn_th = new System.Threading.Thread(Check_conn_th_f);
                check_conn_th.Start();
                check_conn_timer.Start();
                _waitHandle.WaitOne();
                return current;
            }
        }

        public static Check_server_ret Check_server(Variables.Server_entry server_entry)
        {
            connect_checker1 cc = new connect_checker1();
            return cc.Check(server_entry);
        }
        public struct Check_server_ret
        {
            public Check_conneck_state connection_state; public Server_read rdata;
            public Check_server_ret(Check_conneck_state cs, Server_read rd)
            {
                connection_state = cs; rdata = rd;
            }
        }
        public struct Server_read
        {
            public string format, version, location;
            public Server_read(string format, string version, string location)
            {
                this.format = format; this.version = version; this.location = location;
            }
        }
        class connect_checker1
        {
            System.Threading.Thread check_conn_th;
            System.Timers.Timer check_conn_timer = new System.Timers.Timer(100);
            byte check_conn_th_v = 0, tick = 0; ushort time_wait = 0;
            Variables.Server_entry csi;
            static System.Threading.EventWaitHandle _waitHandle = new System.Threading.AutoResetEvent(false);
            void Check_conn_th_f()
            {
                FTP ftp = new FTP("ftp://" + csi.ip + "/", csi.user_name, csi.password);
                string file = ftp.download("FTP_UI_Updade_System.ini");
                if (file != null)
                {
                    string[] lines = Auxiliary.Get_lines_fffs(file);
                    format = Auxiliary.Get_val_by_key(lines, "format");
                    version = Auxiliary.Get_val_by_key(lines, "version");
                    location = Auxiliary.Get_val_by_key(lines, "location");
                    check_conn_th_v = 2;
                }
                else
                {
                    check_conn_th_v = 1;
                }
            }
            string format, version, location;
            void Check_conn_timer_elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                tick++;
                if (check_conn_th_v != 0)
                {
                    check_conn_timer.Stop();
                    current = new Check_server_ret((check_conn_th_v == 2 ? Check_conneck_state.Connected : Check_conneck_state.NoConnect), new Server_read(format, version, location));
                    _waitHandle.Set();
                }
                else if (tick == 10)
                {
                    if (time_wait >= Variables.time_func[0])
                    {
                        check_conn_timer.Stop();
                        current = new Check_server_ret(Check_conneck_state.NoAnswer, new Server_read());
                        _waitHandle.Set();
                    }
                    tick = 0;
                    time_wait += 1;
                }
            }
            
            Check_server_ret current;
            public Check_server_ret Check(Variables.Server_entry server_entry)
            {
                csi = server_entry;
                check_conn_timer.Elapsed += new System.Timers.ElapsedEventHandler(Check_conn_timer_elapsed);
                check_conn_th = new System.Threading.Thread(Check_conn_th_f);
                check_conn_th.Start();
                check_conn_timer.Start();
                _waitHandle.WaitOne();
                return current;
            }
        }
    }
    class DatabaseToolkit
    {
        public static void Write_file_database(string filename, string program = "")
        {
            if (program == "")
            {
                string[] write_str;
                switch (filename)
                {
                    case "components":
                        write_str = new string[5];
                        write_str[0] = "UI=" + Variables.components[0];
                        write_str[1] = "UIK=" + Variables.components[1];
                        write_str[2] = "UIU=" + Variables.components[2];
                        write_str[3] = "UIL=" + Variables.components[3];
                        write_str[4] = "UILD=" + Variables.components[4];
                        Variables.local_database.OFC_FileData_write("UI", "components", write_str);
                        break;
                    case "prog":
                        write_str = new string[9];
                        write_str[0] = "startup=" + Variables.prog_str[0];
                        write_str[1] = "Ito=" + Variables.time_func[0];
                        write_str[2] = "Ics=" + Variables.time_func[1];
                        write_str[3] = "Ltp=" + Variables.time_func[2];
                        write_str[4] = "Ktp=" + Variables.time_func[3];
                        write_str[5] = "PKtp=" + Variables.time_func[4];
                        write_str[6] = "Ictc=" + Variables.time_func[5];
                        write_str[7] = "ulocation=" + Variables.prog_str[2];
                        if (Variables.prog_str[1] != "")
                            write_str[8] = "name=" + Variables.prog_str[1];
                        else
                            write_str[8] = "";
                        Variables.local_database.OFC_FileData_write("UI", "prog", write_str);
                        break;
                    case "servers":
                        write_str = new string[Variables.servers.Count];
                        for (int i = 0; i < write_str.Length; i++)
                        {
                            write_str[i] = Variables.servers[i].number + "|" + Variables.servers[i].ip + "|" + Variables.servers[i].user_name + "|" + Variables.servers[i].password;
                        }
                        Variables.local_database.OFC_FileData_write("UI", "servers", Auxiliary.Get_lines_from_text(System.Text.Encoding.Default.GetString(Encryption.EncryptStringToBytes_Aes(Auxiliary.Get_text_from_lines(write_str), Variables.Encryption_KEY, Variables.Encryption_IV))));
                        break;
                    case "al_com":
                        Variables.local_database.OFC_FileData_write("UI", "al_com", Variables.al_commands);
                        break;
                }
            }
            else
            {
                string[] write_str;
                Variables.Program_entry e = null;
                switch (filename)
                {
                    case "prog":
                        foreach (Variables.Program_entry e1 in Variables.programs)
                        {
                            if (e1.name == program)
                                e = e1;
                        }
                        write_str = new string[7];
                        write_str[0] = "version=" + e.version;
                        write_str[1] = "location=" + e.p_data[0];
                        write_str[2] = "launch=" + e.p_data[1];
                        write_str[3] = "launchArgs=" + e.p_data[2];
                        write_str[4] = "launchRestoreArgs=" + e.p_data[3];
                        write_str[5] = "uninstall=" + e.p_data[4];
                        write_str[6] = "uninstallArgs=" + e.p_data[5];
                        Variables.local_database.OFC_FileData_write("Progs\\" + program, "prog", write_str);
                        break;
                    case "cprops":
                        foreach (Variables.Program_entry e1 in Variables.programs)
                        {
                            if (e1.name == program)
                                e = e1;
                        }
                        write_str = new string[8];
                        write_str[0] = "install=" + (e.props[0] ? "yes" : "no");
                        write_str[1] = "start=" + (e.props[1] ? "yes" : "no");
                        write_str[2] = "stop=" + (e.props[2] ? "yes" : "no");
                        write_str[3] = "uninstall=" + (e.props[3] ? "yes" : "no");
                        write_str[4] = "config=" + (e.props[4] ? "yes" : "no");
                        write_str[5] = "commands=" + (e.props[5] ? "yes" : "no");
                        write_str[6] = "retdata=" + (e.props[6] ? "yes" : "no");
                        write_str[7] = "log=" + (e.props[7] ? "yes" : "no");
                        Variables.local_database.OFC_FileData_write("Progs\\" + program, "cprops", write_str);
                        break;
                    case "set":
                        foreach (Variables.Program_entry e1 in Variables.programs)
                        {
                            if (e1.name == program)
                                e = e1;
                        }
                        Variables.local_database.OFC_FileData_write("Progs\\" + program, "set", new string[1] { "active=" + (e.props[8] ? "yes" : "no") });
                        break;
                    case "al_com":
                        foreach (Variables.Program_entry e1 in Variables.programs)
                        {
                            if (e1.name == program)
                                e = e1;
                        }
                        Variables.local_database.OFC_FileData_write("Progs\\" + program, "al_com", e.always_commands);
                        break;
                }
            }
        }
        public static void Write_all_file_database()
        {
            string[] write_str = new string[5];
            write_str[0] = "UI=" + Variables.components[0];
            write_str[1] = "UIK=" + Variables.components[1];
            write_str[2] = "UIU=" + Variables.components[2];
            write_str[3] = "UIL=" + Variables.components[3];
            write_str[4] = "UILD=" + Variables.components[4];
            Variables.local_database.OFC_FileData_write("UI", "components", write_str);
            write_str = new string[9];
            write_str[0] = "startup=" + Variables.prog_str[0];
            write_str[1] = "Ito=" + Variables.time_func[0];
            write_str[2] = "Ics=" + Variables.time_func[1];
            write_str[3] = "Ltp=" + Variables.time_func[2];
            write_str[4] = "Ktp=" + Variables.time_func[3];
            write_str[5] = "PKtp=" + Variables.time_func[4];
            write_str[6] = "Ictc=" + Variables.time_func[5];
            write_str[7] = "ulocation=" + Variables.prog_str[2];
            if (Variables.prog_str[1] != "")
                write_str[8] = "name=" + Variables.prog_str[1];
            else
                write_str[8] = "";
            Variables.local_database.OFC_FileData_write("UI", "prog", write_str);
            write_str = new string[Variables.servers.Count];
            for (int i = 0; i < write_str.Length; i++)
            {
                write_str[i] = Variables.servers[i].number + "|" + Variables.servers[i].ip + "|" + Variables.servers[i].user_name + "|" + Variables.servers[i].password;
            }
            Variables.local_database.OFC_FileData_write("UI", "servers", Auxiliary.Get_lines_from_text(System.Text.Encoding.Default.GetString(Encryption.EncryptStringToBytes_Aes(Auxiliary.Get_text_from_lines(write_str), Variables.Encryption_KEY, Variables.Encryption_IV))));
            Variables.local_database.OFC_FileData_write("UI", "al_com", Variables.al_commands);
            foreach (Variables.Program_entry e in Variables.programs)
            {
                write_str = new string[7];
                write_str[0] = "version=" + e.version;
                write_str[1] = "location=" + e.p_data[0];
                write_str[2] = "launch=" + e.p_data[1];
                write_str[3] = "launchArgs=" + e.p_data[2];
                write_str[4] = "launchRestoreArgs=" + e.p_data[3];
                write_str[5] = "uninstall=" + e.p_data[4];
                write_str[6] = "uninstallArgs=" + e.p_data[5];
                Variables.local_database.OFC_FileData_write("Progs\\" + e.name, "prog", write_str);
                write_str = new string[8];
                write_str[0] = "install=" + (e.props[0] ? "yes" : "no");
                write_str[1] = "start=" + (e.props[1] ? "yes" : "no");
                write_str[2] = "stop=" + (e.props[2] ? "yes" : "no");
                write_str[3] = "uninstall=" + (e.props[3] ? "yes" : "no");
                write_str[4] = "config=" + (e.props[4] ? "yes" : "no");
                write_str[5] = "commands=" + (e.props[5] ? "yes" : "no");
                write_str[6] = "retdata=" + (e.props[6] ? "yes" : "no");
                write_str[7] = "log=" + (e.props[7] ? "yes" : "no");
                Variables.local_database.OFC_FileData_write("Progs\\" + e.name, "cprops", write_str);
                if (Variables.local_database.OFC_Entry_exist("Progs\\" + e.name, "set", OFC_FS.OFC_Types.File))
                    Variables.local_database.OFC_FileData_write("Progs\\" + e.name, "set", new string[1] { "active=" + (e.props[8] ? "yes" : "no") });
                if (Variables.local_database.OFC_Entry_exist("Progs\\" + e.name, "al_com", OFC_FS.OFC_Types.File))
                    Variables.local_database.OFC_FileData_write("Progs\\" + e.name, "al_com", e.always_commands);
            }
        }
    }
}