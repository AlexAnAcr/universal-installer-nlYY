using System;
//using System.Linq;

namespace UniversalInstaller
{
    static class Code_1
    {
        public static void Load_database()
        {
            if (System.IO.File.Exists(Variables.MY_WAY + "\\database.dat"))
            {
                Variables.local_database.OFC_Fill_list_by_Array(System.IO.File.ReadAllLines(Variables.MY_WAY + "\\database.dat"));
                if (Variables.local_database.OFC_Entry_exist("UI", "components", OFC_FS.OFC_Types.File) && Variables.local_database.OFC_Entry_exist("UI", "prog", OFC_FS.OFC_Types.File) && Variables.local_database.OFC_Entry_exist("UI", "servers", OFC_FS.OFC_Types.File))
                {
                    foreach (string i in Variables.local_database.OFC_FileData_read("UI", "components"))
                    {
                        string[] split_arr = Auxiliary.Split_key_val(i);
                        switch (split_arr[0])
                        {
                            case "UI":
                                Variables.components[0] = split_arr[1]; //UI.exe
                                break;
                            case "UIK":
                                Variables.components[1] = split_arr[1]; //UI keeper.exe
                                break;
                            case "UIU":
                                Variables.components[2] = split_arr[1]; //UI updater.exe
                                break;
                            case "UIL":
                                Variables.components[3] = split_arr[1]; //UI launcher.exe
                                break;
                            case "UILD":
                                Variables.components[4] = split_arr[1]; //UI launcher destination name.exe
                                break;
                        }
                    }

                    foreach (string i in Variables.local_database.OFC_FileData_read("UI", "prog"))
                    {
                        string[] split_arr = Auxiliary.Split_key_val(i);
                        switch (split_arr[0])
                        {
                            case "startup":
                                Variables.prog_str[0] = split_arr[1]; //Autorun folder (default: FCUSER)
                                break;
                            case "name":
                                Variables.prog_str[1] = split_arr[1]; //Name of current UI copy
                                break;
                            case "Ito":
                                Variables.time_func[0] = ushort.Parse(split_arr[1]); //Internet connect timeout
                                break;
                            case "Ics":
                                Variables.time_func[1] = ushort.Parse(split_arr[1]); //Interval of internet check
                                break;
                            case "Ltp":
                                Variables.time_func[2] = ushort.Parse(split_arr[1]); //Launcher exist check pause
                                break;
                            case "Ktp":
                                Variables.time_func[3] = ushort.Parse(split_arr[1]); //UI keeper check pause
                                break;
                            case "PKtp":
                                Variables.time_func[4] = ushort.Parse(split_arr[1]); //Pause of all programs keeper without UI
                                break;
                            case "Ictc":
                                Variables.time_func[5] = ushort.Parse(split_arr[1]); //Count of failure attempts to connect to server. Turn to next server (course, if internet connected)
                                break;
                            case "ulocation":
                                Variables.prog_str[2] = split_arr[1]; //Update folder location
                                break;
                        }
                    }

                    if (Variables.local_database.OFC_Entry_exist("UI", "al_com", OFC_FS.OFC_Types.File))
                    {
                        Variables.al_commands = Variables.local_database.OFC_FileData_read("UI", "al_com");
                    }

                    foreach (string i in Auxiliary.Get_lines_from_text(Encryption.DecryptStringFromBytes_Aes(System.Text.Encoding.Default.GetBytes(Auxiliary.Get_text_from_lines(Variables.local_database.OFC_FileData_read("UI", "servers"))), Variables.Encryption_KEY, Variables.Encryption_IV)))
                    {
                        string[] lines_split = i.Split('|');
                        Variables.servers.Add(new Variables.Server_entry(ushort.Parse(lines_split[0]), lines_split[1], lines_split[2], lines_split[3]));
                    }

                    foreach (string i in Variables.local_database.OFC_get_names("Progs", OFC_FS.OFC_Types.Folder))
                    {
                        Variables.Program_entry re = new Variables.Program_entry
                        {
                            name = i
                        };
                        foreach (string i1 in Variables.local_database.OFC_FileData_read("Progs\\" + i, "prog"))
                        {
                            string[] split_arr = Auxiliary.Split_key_val(i1);
                            switch (split_arr[0])
                            {
                                case "version":
                                    re.version = float.Parse(split_arr[1]); //Version
                                    break;
                                case "location":
                                    re.p_data[0] = split_arr[1]; //Program main locatgion
                                    break;
                                case "launch":
                                    re.p_data[1] = split_arr[1]; //Way to main launch exe file
                                    break;
                                case "launchArgs":
                                    re.p_data[2] = split_arr[1]; //Arguments for launch
                                    break;
                                case "launchRestoreArgs":
                                    re.p_data[3] = split_arr[1]; //Arguments for launch after process exit
                                    break;
                                case "uninstall":
                                    re.p_data[4] = split_arr[1]; //Uninstall exe in main folder
                                    break;
                                case "uninstallArgs":
                                    re.p_data[5] = split_arr[1]; //Arguments for uninstall
                                    break;
                            }
                        }
                        foreach (string i1 in Variables.local_database.OFC_FileData_read("Progs\\" + i, "cprops"))
                        {
                            string[] split_arr = Auxiliary.Split_key_val(i1);
                            switch (split_arr[0])
                            {
                                case "install":
                                    re.props[0] = (split_arr[1] == "yes" ? true : false); //Allow installing
                                    break;
                                case "start":
                                    re.props[1] = (split_arr[1] == "yes" ? true : false); //Allow program start
                                    break;
                                case "stop":
                                    re.props[2] = (split_arr[1] == "yes" ? true : false); //Deny program stop (enable program keeper)
                                    break;
                                case "uninstall":
                                    re.props[3] = (split_arr[1] == "yes" ? true : false); //Allow program uninstall
                                    break;
                                case "config":
                                    re.props[4] = (split_arr[1] == "yes" ? true : false); //Allow program custom config
                                    break;
                                case "commands":
                                    re.props[5] = (split_arr[1] == "yes" ? true : false); //Allow program commands
                                    break;
                                case "retdata":
                                    re.props[6] = (split_arr[1] == "yes" ? true : false); //Allow program data to return
                                    break;
                                case "log":
                                    re.props[7] = (split_arr[1] == "yes" ? true : false); //Allow program log
                                    break;
                            }
                        }
                        if (re.props[4])
                        {
                            if (Variables.local_database.OFC_Entry_exist("Progs\\" + i, "set", OFC_FS.OFC_Types.File))
                            {
                                foreach (string i1 in Variables.local_database.OFC_FileData_read("Progs\\" + i, "set"))
                                {
                                    string[] split_arr = Auxiliary.Split_key_val(i1);
                                    if (split_arr[0] == "active") re.props[8] = (split_arr[1] == "yes" ? true : false); //Program is active
                                }
                            }
                        }
                        if (re.props[5])
                        {
                            if (Variables.local_database.OFC_Entry_exist("Progs\\" + i, "al_com", OFC_FS.OFC_Types.File))
                            {
                                re.always_commands = Variables.local_database.OFC_FileData_read("Progs\\" + i, "al_com");
                            }
                        }
                        Variables.programs.Add(re);
                    }
                }
            }
        }
        public static void Correct_database()
        {
            bool somecorrect = false;
            if (Variables.prog_str[2] == "")
            {
                Variables.prog_str[2] = Variables.MY_WAY + "\\PUpdates";
                DatabaseToolkit.Write_file_database("prog");
                somecorrect = true;
            }
            foreach (Variables.Program_entry i in Variables.programs)
            {
                if (i.props[4])
                {
                    if (!Variables.local_database.OFC_Entry_exist("Progs\\" + i.name, "set", OFC_FS.OFC_Types.File))
                    {
                        Variables.local_database.OFC_Make_entry("Progs\\" + i.name, "set", OFC_FS.OFC_Types.File, new string[] { "active=yes" });
                        somecorrect = true;
                    }
                }
                else
                {
                    if (Variables.local_database.OFC_Entry_exist("Progs\\" + i.name, "set", OFC_FS.OFC_Types.File))
                    {
                        Variables.local_database.OFC_Delete_entry("Progs\\" + i.name, "set", OFC_FS.OFC_Types.File, true);
                        somecorrect = true;
                    }
                }
                if (i.props[5])
                {
                    if (!Variables.local_database.OFC_Entry_exist("Progs\\" + i.name, "al_com", OFC_FS.OFC_Types.File))
                    {
                        Variables.local_database.OFC_Make_entry("Progs\\" + i.name, "al_com", OFC_FS.OFC_Types.File);
                        somecorrect = true;
                    }
                }
                else
                {
                    if (Variables.local_database.OFC_Entry_exist("Progs\\" + i.name, "al_com", OFC_FS.OFC_Types.File))
                    {
                        Variables.local_database.OFC_Delete_entry("Progs\\" + i.name, "al_com", OFC_FS.OFC_Types.File, true);
                        somecorrect = true;
                    }
                }
                
            }
            if (somecorrect) System.IO.File.WriteAllLines(Variables.MY_WAY + "\\database.dat", Variables.local_database.OFC_export_to_arr());
        }
        public enum Check_conneck_state
        {
            NoAnswer, NoConnect, WrongFormat, Connected
        }
        public static Check_conneck_state First_try_connect(Variables.Server_entry server_entry)
        {
            for (int i = 0; i < 3; i++)
            {
                Connect.Check_conneck_state cs = Connect.Check_connection();
                if (cs == Connect.Check_conneck_state.Connected)
                {
                    Connect.Check_server_ret sr = Connect.Check_server(server_entry);
                    if (sr.connection_state == Connect.Check_conneck_state.Connected)
                    {
                        if (sr.rdata.format == "FTP_FIwO1" && Auxiliary.Get_Equals_by_key(Variables.SUPPORT_VERSIONS, sr.rdata.version))
                        {
                            Program.ftp_client = new FTP("ftp://" + server_entry.ip + "/", server_entry.user_name, server_entry.password);
                            Program.ftp_way_str = sr.rdata.location;
                            return Check_conneck_state.Connected;
                        }
                        else
                            return Check_conneck_state.WrongFormat;
                    }
                    else if (i == 2)
                    {
                        return (sr.connection_state == Connect.Check_conneck_state.NoAnswer ? Check_conneck_state.NoAnswer : Check_conneck_state.NoConnect);
                    }
                }
                else if (i == 2)
                {
                    return (cs == Connect.Check_conneck_state.NoAnswer ? Check_conneck_state.NoAnswer : Check_conneck_state.NoConnect);
                }
                System.Threading.Thread.Sleep(5000);
            }
            return Check_conneck_state.NoAnswer;
        }
    }
}