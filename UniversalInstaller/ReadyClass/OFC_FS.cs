namespace UniversalInstaller
{
    public class OFC_FS
    {
        System.Collections.Generic.List<OFC_Entry> OFC_FS_storage = new System.Collections.Generic.List<OFC_Entry>();
        string[] patterns = { @"^<{1}[^<>]+>{1}", @">[^<>]*>?$", @"<>$" };
        public void OFC_Fill_list_by_Array(string[] arr)
        {
            bool is_addmode = false; string[] lines = new string[0], save_dat = new string[2];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].IndexOf('>') != -1)
                {
                    if (is_addmode)
                    {
                        if (arr[i].IndexOf('>') == -1)
                        {
                            System.Array.Resize(ref lines, lines.Length + 1);
                            lines[lines.Length - 1] = OFC_Convert_string(arr[i], true);
                        }
                        else
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], @">$"))
                            {
                                System.Array.Resize(ref lines, lines.Length + 1);
                                lines[lines.Length - 1] = OFC_Convert_string(arr[i].Substring(0, arr[i].Length - 1), true);
                                OFC_Make_entry(save_dat[0], save_dat[1], OFC_Types.File, lines);
                                is_addmode = false;
                                System.Array.Resize(ref lines, 0); save_dat[0] = null; save_dat[1] = null;
                            }
                            else
                            {
                                is_addmode = false;
                                System.Array.Resize(ref lines, 0); save_dat[0] = null; save_dat[1] = null;
                            }
                        }
                    }
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], patterns[0]))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], patterns[2]))
                            {
                                string[] dir_full_way = new string[1];
                                dir_full_way[0] = System.Text.RegularExpressions.Regex.Match(arr[i], "^<{1}[^<>]+").ToString().Substring(1);
                                dir_full_way = OFC_Split_way_name(dir_full_way[0]);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                            else if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], patterns[1]))
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], @">>$"))
                                {
                                    string[] dir_full_way = new string[1];
                                    dir_full_way[0] = System.Text.RegularExpressions.Regex.Match(arr[i], "^<{1}[^<>]+").ToString().Substring(1);
                                    dir_full_way = OFC_Split_way_name(dir_full_way[0]);
                                    if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.File))
                                    {
                                        OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.File);
                                    }
                                }
                                else if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], @">[^<>]+>$"))
                                {
                                    string[] dir_full_way = new string[1];
                                    dir_full_way[0] = System.Text.RegularExpressions.Regex.Match(arr[i], "^<{1}[^<>]+").ToString().Substring(1);
                                    dir_full_way = OFC_Split_way_name(dir_full_way[0]);
                                    if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.File))
                                    {
                                        string temp = System.Text.RegularExpressions.Regex.Match(arr[i], ">[^<>]+>$").ToString();
                                        OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.File, new string[1] { OFC_Convert_string(temp.Substring(1, temp.Length - 2), true) });
                                    }
                                }
                                else if (System.Text.RegularExpressions.Regex.IsMatch(arr[i], @">[^<>]+$"))
                                {
                                    string[] dir_full_way = new string[1];
                                    dir_full_way[0] = System.Text.RegularExpressions.Regex.Match(arr[i], "^<{1}[^<>]+").ToString().Substring(1);
                                    dir_full_way = OFC_Split_way_name(dir_full_way[0]);
                                    if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.File))
                                    {
                                        is_addmode = true;
                                        System.Array.Resize(ref lines, 1);
                                        lines[0] = OFC_Convert_string(System.Text.RegularExpressions.Regex.Match(arr[i], ">[^<>]+$").ToString().Substring(1), true);
                                        save_dat[0] = dir_full_way[0]; save_dat[1] = dir_full_way[1];
                                    }
                                }
                            }
                        }
                    }
                }
                else if (is_addmode)
                {
                    System.Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = OFC_Convert_string(arr[i], true);
                }
            }
        }

        public object OFC_Create_Copy_entry_exist_map(OFC_Types e_type, string s_way, string s_name, string d_way, string d_name)
        {
            System.Collections.Generic.List<OFC_ExistEntry_data> collection = new System.Collections.Generic.List<OFC_ExistEntry_data>();
            if (e_type == OFC_Types.File)
            {
                if (OFC_Entry_exist(d_way, d_name, OFC_Types.File))
                    collection.Add(new OFC_ExistEntry_data(1, OFC_Types.File, s_way, s_name, d_way, d_name));
            }
            else
            {
                bool folder_exist = false;
                foreach (string i in OFC_get_names(d_way, OFC_Types.Folder))
                {
                    if (i == d_name) { collection.Add(new OFC_ExistEntry_data(0, OFC_Types.Folder, s_way, s_name, d_way, d_name)); folder_exist = true; break; }
                }
                if (folder_exist)
                {
                    if (d_way == "")
                    {
                        if (s_way == "")
                        {
                            for (int i = 0; i < OFC_FS_storage.Count; i++)
                            {
                                if (OFC_start_with(OFC_FS_storage[i].way, s_name) || (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                                {
                                    if (OFC_Entry_exist(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                    {
                                        collection.Add(new OFC_ExistEntry_data(1, OFC_FS_storage[i].type, OFC_FS_storage[i].way, OFC_FS_storage[i].name, d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name));
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < OFC_FS_storage.Count; i++)
                            {
                                if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name) || (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                                {
                                    if (OFC_Entry_exist(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                    {
                                        collection.Add(new OFC_ExistEntry_data(1, OFC_FS_storage[i].type, OFC_FS_storage[i].way, OFC_FS_storage[i].name, d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (s_way == "")
                        {
                            for (int i = 0; i < OFC_FS_storage.Count; i++)
                            {
                                if (OFC_start_with(OFC_FS_storage[i].way, s_name) || (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                                {
                                    if (OFC_Entry_exist(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                    {
                                        collection.Add(new OFC_ExistEntry_data(1, OFC_FS_storage[i].type, OFC_FS_storage[i].way, OFC_FS_storage[i].name, d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name));
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < OFC_FS_storage.Count; i++)
                            {
                                if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                {
                                    if (OFC_Entry_exist(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                    {
                                        collection.Add(new OFC_ExistEntry_data(1, OFC_FS_storage[i].type, OFC_FS_storage[i].way, OFC_FS_storage[i].name, d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new OFC_ExistEntry_data_collection(collection);
        }
        public delegate OFC_ExistEntry_data FileCopy_poser(OFC_ExistEntry_data poser_data);
        public void OFC_Copy_entry(OFC_Types e_type, string s_way, string s_name, string d_way, string d_name, bool is_copy, FileCopy_poser poser_func)
        {
            if (e_type == OFC_Types.File)
            {
                if (is_copy)
                {
                    if (OFC_Entry_exist(d_way, d_name, OFC_Types.File))
                    {
                        OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, s_way, s_name, d_way, d_name));
                        if (act.Action == OFC_ExistEntry_action.Replace)
                        {
                            OFC_Entry_check(OFC_Entry_get_index(d_way, d_name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data), OFC_Entry_check_param.Data);
                        }
                        else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                        {
                            ushort index = 1;
                            while (OFC_Entry_exist(d_way, d_name + " (" + index + ")", OFC_Types.File)) { index++; }
                            OFC_Make_entry(d_way, d_name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data);
                        }
                    }
                    else
                        OFC_Make_entry(d_way, d_name, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data);
                }
                else
                {
                    if (OFC_Entry_exist(d_way, d_name, OFC_Types.File))
                    {
                        OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, s_way, s_name, d_way, d_name));
                        if (act.Action == OFC_ExistEntry_action.Replace)
                        {
                            OFC_Entry_check(OFC_Entry_get_index(d_way, d_name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data), OFC_Entry_check_param.Data);
                            OFC_Delete_entry(s_way, s_name, OFC_Types.File, false);
                        }
                        else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                        {
                            ushort index = 1;
                            while (OFC_Entry_exist(d_way, d_name + " (" + index + ")", OFC_Types.File)) { index++; }
                            OFC_Entry_check(OFC_Entry_get_index(s_way, s_name, OFC_Types.File), new OFC_Entry(d_way, d_name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        OFC_Entry_check(OFC_Entry_get_index(s_way, s_name, OFC_Types.File), new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                        if (index != -1)
                        {
                            OFC_FS_storage.RemoveAt(index);
                        }
                    }
                    string[] dir_full_way = OFC_Split_way_name(s_way);
                    if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                    {
                        OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                    }
                }
            }
            else
            {
                bool folder_exist = false;
                foreach (string i in OFC_get_names(d_way, OFC_Types.Folder))
                {
                    if (i == d_name)
                    {
                        OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(0, OFC_Types.Folder, s_way, s_name, d_way, d_name));
                        if (act.Action == OFC_ExistEntry_action.NoCopy)
                        {
                            return;
                        }
                        folder_exist = true;
                        break;
                    }
                }
                if (folder_exist)
                {
                    if (is_copy)
                    {
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (!OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (!OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_FS_storage[i].type, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (!OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (!OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index1 = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        if (index1 != -1)
                        {
                            OFC_FS_storage.RemoveAt(index1);
                        }
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                            else
                                                OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                            else
                                                OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                            else
                                                OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        if (OFC_Entry_exist(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type))
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                OFC_ExistEntry_data act = poser_func(new OFC_ExistEntry_data(1, OFC_Types.File, OFC_FS_storage[i].way, OFC_FS_storage[i].name, w_str, OFC_FS_storage[i].name));
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                            else
                                                OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_Entry_exist(d_way, d_name, OFC_Types.Folder))
                                        {
                                            OFC_FS_storage.RemoveAt(i);
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (is_copy)
                    {
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Make_entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry("", d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Make_entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry("", d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Make_entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Make_entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index1 = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        if (index1 != -1)
                        {
                            OFC_FS_storage.RemoveAt(index1);
                        }
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                    }
                }
            }
        }
        OFC_ExistEntry_data OFC_get_map_entry_data(OFC_ExistEntry_data_collection exist_map, string way, string name, OFC_Types e_type)
        {
            foreach (OFC_ExistEntry_data i in exist_map.Collection)
            {
                if (i.Way == way && i.Name == name && i.Type == e_type)
                {
                    return i;
                }
            }
            return null;
        }
        public void OFC_Copy_entry(OFC_Types e_type, string s_way, string s_name, string d_way, string d_name, bool is_copy, object exist_map)
        {
            OFC_ExistEntry_data_collection data_map = exist_map as OFC_ExistEntry_data_collection;
            if (e_type == OFC_Types.File)
            {
                if (is_copy)
                {
                    OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.File);
                    if (act != null)
                    {
                        if (act.Action == OFC_ExistEntry_action.Replace)
                        {
                            OFC_Entry_check(OFC_Entry_get_index(d_way, d_name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data), OFC_Entry_check_param.Data);
                        }
                        else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                        {
                            ushort index = 1;
                            while (OFC_Entry_exist(d_way, d_name + " (" + index + ")", OFC_Types.File)) { index++; }
                            OFC_Make_entry(d_way, d_name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data);
                        }
                    }
                    else
                        OFC_Make_entry(d_way, d_name, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data);
                }
                else
                {
                    OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.File);
                    if (act != null)
                    {
                        if (act.Action == OFC_ExistEntry_action.Replace)
                        {
                            OFC_Entry_check(OFC_Entry_get_index(d_way, d_name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[OFC_Entry_get_index(s_way, s_name, OFC_Types.File)].data), OFC_Entry_check_param.Data);
                            OFC_Delete_entry(s_way, s_name, OFC_Types.File, false);
                        }
                        else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                        {
                            ushort index = 1;
                            while (OFC_Entry_exist(d_way, d_name + " (" + index + ")", OFC_Types.File)) { index++; }
                            OFC_Entry_check(OFC_Entry_get_index(s_way, s_name, OFC_Types.File), new OFC_Entry(d_way, d_name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        OFC_Entry_check(OFC_Entry_get_index(s_way, s_name, OFC_Types.File), new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                        if (index != -1)
                        {
                            OFC_FS_storage.RemoveAt(index);
                        }
                    }
                    string[] dir_full_way = new string[1];
                    dir_full_way = OFC_Split_way_name(s_way);
                    if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                    {
                        OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                    }
                }
            }
            else
            {
                bool folder_exist = false;
                {
                    OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder);
                    if (act != null)
                    {
                        if (act.Action == OFC_ExistEntry_action.NoCopy)
                        {
                            return;
                        }
                        folder_exist = true;
                    }
                }
                if (folder_exist)
                {
                    if (is_copy)
                    {
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Make_entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, OFC_FS_storage[i].data);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Make_entry(w_str, OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index1 = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        if (index1 != -1)
                        {
                            OFC_FS_storage.RemoveAt(index1);
                        }
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        }
                                        else
                                            OFC_FS_storage.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        }
                                        else
                                            OFC_FS_storage.RemoveAt(i);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        }
                                        else
                                            OFC_FS_storage.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        string w_str = d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length);
                                        OFC_ExistEntry_data act = OFC_get_map_entry_data(data_map, OFC_FS_storage[i].way, OFC_FS_storage[i].name, OFC_FS_storage[i].type);
                                        if (act != null)
                                        {
                                            if (OFC_FS_storage[i].type != OFC_Types.Folder)
                                            {
                                                if (act.Action == OFC_ExistEntry_action.Replace)
                                                {
                                                    OFC_Entry_check(OFC_Entry_get_index(w_str, OFC_FS_storage[i].name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, OFC_FS_storage[i].data), OFC_Entry_check_param.Data);
                                                    OFC_FS_storage.RemoveAt(i); i--;
                                                }
                                                else if (act.Action == OFC_ExistEntry_action.SaveBoth)
                                                {
                                                    ushort index = 1;
                                                    while (OFC_Entry_exist(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File)) { index++; }
                                                    OFC_Entry_check(i, new OFC_Entry(w_str, OFC_FS_storage[i].name + " (" + index + ")", OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                                }
                                            }
                                        }
                                        else
                                            OFC_Entry_check(i, new OFC_Entry(w_str, null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        if (OFC_get_map_entry_data(data_map, d_way, d_name, OFC_Types.Folder) == null)
                                        {
                                            OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        }
                                        else
                                            OFC_FS_storage.RemoveAt(i);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (is_copy)
                    {
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Make_entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Make_entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Make_entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Make_entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), OFC_FS_storage[i].name, OFC_FS_storage[i].type, OFC_FS_storage[i].data);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Make_entry(d_way, d_name, OFC_Types.Folder);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] dir_full_way1 = OFC_Split_way_name(d_way);
                        int index1 = OFC_Entry_get_index(dir_full_way1[0], dir_full_way1[1], OFC_Types.Folder);
                        if (index1 != -1)
                        {
                            OFC_FS_storage.RemoveAt(index1);
                        }
                        if (d_way == "")
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                        else
                        {
                            if (s_way == "")
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < OFC_FS_storage.Count; i++)
                                {
                                    if (OFC_start_with(OFC_FS_storage[i].way, s_way + "\\" + s_name))
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way + "\\" + d_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(s_way + "\\" + s_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                                    }
                                    else if (OFC_FS_storage[i].way == s_way && OFC_FS_storage[i].name == s_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                                    {
                                        OFC_Entry_check(i, new OFC_Entry(d_way, d_name, OFC_Types.File, null), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                                        break;
                                    }
                                }
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(s_way);
                                if (!OFC_Entry_exist(dir_full_way[0], dir_full_way[1], OFC_Types.Folder))
                                {
                                    OFC_Make_entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void OFC_FileData_write(string way, string name, string[] new_data)
        {
            OFC_Entry_check(OFC_Entry_get_index(way, name, OFC_Types.File), new OFC_Entry(null, null, OFC_Types.File, new_data), OFC_Entry_check_param.Data);
        }
        public string[] OFC_FileData_read(string way, string name)
        {
            return OFC_FS_storage[OFC_Entry_get_index(way, name, OFC_Types.File)].data;
        }
        public void OFC_Rename_entry(string e_way, string e_name, string new_name, OFC_Types e_type)
        {
            if (e_type == OFC_Types.File)
            {
                OFC_Entry_check(OFC_Entry_get_index(e_way, e_name, OFC_Types.File), new OFC_Entry(null, new_name, OFC_Types.File), OFC_Entry_check_param.Name);
            }
            else
            {

                if (OFC_Entry_get_index(e_way, e_name, OFC_Types.Folder) != -1)
                {
                    OFC_Entry_check(OFC_Entry_get_index(e_way, e_name, OFC_Types.Folder), new OFC_Entry(null, new_name, OFC_Types.File), OFC_Entry_check_param.Name);
                }
                else
                {
                    if (e_way == "")
                    {
                        for (int i = 0; i < OFC_FS_storage.Count; i++)
                        {
                            if (OFC_start_with(OFC_FS_storage[i].way, e_name) || (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].name == e_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                            {
                                OFC_Entry_check(i, new OFC_Entry(new_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(e_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < OFC_FS_storage.Count; i++)
                        {
                            if (OFC_start_with(OFC_FS_storage[i].way, e_way + "\\" + e_name) || (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].name == e_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                            {
                                OFC_Entry_check(i, new OFC_Entry(e_way + "\\" + new_name + OFC_cut_way(OFC_FS_storage[i].way, (short)(e_way + "\\" + e_name).Length), null, OFC_Types.File, null), OFC_Entry_check_param.Way);
                            }
                        }
                    }
                }
            }
        }
        public void OFC_Delete_All()
        {
            OFC_FS_storage.RemoveRange(0, OFC_FS_storage.Count);
        }
        public string[] OFC_get_names(string e_way, OFC_Types e_type)
        {
            string[] answer = new string[0];
            if (e_type == OFC_Types.File)
            {
                for (int i = 0; i < OFC_FS_storage.Count; i++)
                {
                    if (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].type == OFC_Types.File)
                    {
                        System.Array.Resize(ref answer, answer.Length + 1);
                        answer[answer.Length - 1] = OFC_FS_storage[i].name;
                    }
                }
            }
            else
            {
                if (e_way == "")
                {
                    string[] no_start_with = new string[0];
                    for (int i = 0; i < OFC_FS_storage.Count; i++)
                    {
                        if (OFC_FS_storage[i].way == "" && OFC_FS_storage[i].type == OFC_Types.Folder)
                        {
                            System.Array.Resize(ref answer, answer.Length + 1);
                            answer[answer.Length - 1] = OFC_FS_storage[i].name;
                        }
                        else
                        {
                            if (OFC_not_start_with(no_start_with, OFC_FS_storage[i].way) && !(OFC_FS_storage[i].way == ""))
                            {
                                System.Array.Resize(ref answer, answer.Length + 1);
                                answer[answer.Length - 1] = OFC_FS_storage[i].way.Split('\\')[0];
                                System.Array.Resize(ref no_start_with, no_start_with.Length + 1);
                                no_start_with[no_start_with.Length - 1] = answer[answer.Length - 1];
                            }
                        }
                    }
                }
                else
                {
                    string[] no_start_with = new string[0];
                    for (int i = 0; i < OFC_FS_storage.Count; i++)
                    {
                        if (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].type == OFC_Types.Folder)
                        {
                            System.Array.Resize(ref answer, answer.Length + 1);
                            answer[answer.Length - 1] = OFC_FS_storage[i].name;
                        }
                        else if (OFC_FS_storage[i].way.StartsWith(e_way + "\\"))
                        {
                            if (OFC_not_start_with(no_start_with, OFC_FS_storage[i].way))
                            {
                                System.Array.Resize(ref answer, answer.Length + 1);
                                answer[answer.Length - 1] = OFC_FS_storage[i].way.Remove(0, e_way.Length).Split('\\')[1];
                                System.Array.Resize(ref no_start_with, no_start_with.Length + 1);
                                no_start_with[no_start_with.Length - 1] = e_way + "\\" + answer[answer.Length - 1];
                            }
                        }
                    }
                }
            }
            return answer;
        }
        bool OFC_not_start_with(string[] no_ways, string way)
        {
            foreach (string i in no_ways)
            {
                if (OFC_start_with(way, i)) return false;
            }
            return true;
        }
        public void OFC_Delete_entry(string e_way, string e_name, OFC_Types e_type, bool save_dir)
        {
            if (e_type == OFC_Types.Folder)
            {
                if (e_way == "")
                {
                    for (int i = 0; i < OFC_FS_storage.Count; i++)
                    {
                        if ((OFC_start_with(OFC_FS_storage[i].way, e_name)) || (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].name == e_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                        {
                            OFC_FS_storage.RemoveAt(i--);
                        }
                    }
                }
                else
                {
                    if (OFC_is_latest_folder(e_way, e_name, OFC_Types.Folder) && save_dir)
                    {
                        int i; bool no_empty_folder = true;
                        for (i = 0; i < OFC_FS_storage.Count; i++)
                        {
                            if (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].name == e_name && OFC_FS_storage[i].type == OFC_Types.Folder)
                            {
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(e_way);
                                OFC_Entry_check(i, new OFC_Entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder, new string[0]), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name | OFC_Entry_check_param.Type | OFC_Entry_check_param.Data);
                                no_empty_folder = false;
                                break;
                            }
                            else if (OFC_start_with(OFC_FS_storage[i].way, e_way + "\\" + e_name))
                            {
                                string[] dir_full_way = new string[1];
                                dir_full_way = OFC_Split_way_name(e_way);
                                OFC_Entry_check(i, new OFC_Entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder, new string[0]), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name | OFC_Entry_check_param.Type | OFC_Entry_check_param.Data);
                                break;
                            }
                        }
                        for (; i < OFC_FS_storage.Count && no_empty_folder; i++)
                        {
                            if (OFC_start_with(OFC_FS_storage[i].way, e_way + "\\" + e_name))
                            {
                                OFC_FS_storage.RemoveAt(i--);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < OFC_FS_storage.Count; i++)
                        {
                            if (OFC_start_with(OFC_FS_storage[i].way, e_way + "\\" + e_name) || (OFC_FS_storage[i].way == e_way && OFC_FS_storage[i].name == e_name && OFC_FS_storage[i].type == OFC_Types.Folder))
                            {
                                OFC_FS_storage.RemoveAt(i--);
                            }
                        }
                    }
                }
            }
            else
            {
                if (OFC_is_latest_folder(e_way) && save_dir)
                {
                    string[] dir_full_way = new string[1];
                    dir_full_way = OFC_Split_way_name(e_way);
                    OFC_Entry_check(OFC_Entry_get_index(e_way, e_name, e_type), new OFC_Entry(dir_full_way[0], dir_full_way[1], OFC_Types.Folder, new string[0]), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name | OFC_Entry_check_param.Type | OFC_Entry_check_param.Data);
                }
                else
                {
                    OFC_FS_storage.RemoveAt(OFC_Entry_get_index(e_way, e_name, e_type));
                }
            }

        }
        string OFC_cut_way(string way, short cut_part_len)
        {
            return way.Remove(0, cut_part_len);
        }
        bool OFC_is_latest_folder(string e_full_way)
        {
            short find_count = 0;
            for (int i = 0; i < OFC_FS_storage.Count; i++)
            {
                if (OFC_start_with(OFC_FS_storage[i].way, e_full_way))
                {
                    find_count++;
                    if (find_count == 2)
                    {
                        break;
                    }
                }
            }
            if (find_count == 2) { return false; } else { return true; }
        }
        bool OFC_start_with(string way, string st_with)
        {
            if (way.StartsWith(st_with + "\\") || way == st_with)
            {
                return true;
            }
            return false;
        }
        bool OFC_is_latest_folder(string e_full_way, string no_name, OFC_Types no_type)
        {
            for (int i = 0; i < OFC_FS_storage.Count; i++)
            {
                if (OFC_start_with(OFC_FS_storage[i].way, e_full_way) && !(OFC_start_with(OFC_FS_storage[i].way, e_full_way + "\\" + no_name) || (OFC_FS_storage[i].way == e_full_way && OFC_FS_storage[i].name == no_name && OFC_FS_storage[i].type == no_type)))
                {
                    return false;
                }
            }
            return true;
        }
        public void OFC_Make_entry(string e_way, string e_name, OFC_Types e_type)
        {
            if (e_type == OFC_Types.Folder)
            {
                int index_1 = OFC_get_up_folder(e_way);
                if (index_1 != -1)
                {
                    OFC_Entry_check(index_1, new OFC_Entry(e_way, e_name, OFC_Types.File), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                }
                else
                {
                    OFC_Entry entry = new OFC_Entry(e_way, e_name, OFC_Types.Folder);
                    OFC_FS_storage.Add(entry);
                }
            }
            else
            {
                int index_1 = OFC_get_up_folder(e_way);
                if (index_1 != -1)
                {
                    OFC_Entry_check(index_1, new OFC_Entry(e_way, e_name, OFC_Types.File), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name | OFC_Entry_check_param.Type);
                }
                else
                {
                    OFC_Entry entry = new OFC_Entry(e_way, e_name, OFC_Types.File);
                    OFC_FS_storage.Add(entry);
                }
            }
        }
        public string[] OFC_export_to_arr()
        {
            string[] lines = new string[0];
            for (int i = 0; i < OFC_FS_storage.Count; i++)
            {
                System.Array.Resize(ref lines, lines.Length + 1);
                if (OFC_FS_storage[i].type == OFC_Types.File)
                {
                    if (OFC_FS_storage[i].data.Length == 0)
                    {
                        if (OFC_FS_storage[i].way == "")
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].name + ">>";
                        }
                        else
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].way + "\\" + OFC_FS_storage[i].name + ">>";
                        }
                    }
                    else if (OFC_FS_storage[i].data.Length == 1)
                    {
                        if (OFC_FS_storage[i].way == "")
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].name + ">" + OFC_Convert_string(OFC_FS_storage[i].data[0], false) + ">";
                        }
                        else
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].way + "\\" + OFC_FS_storage[i].name + ">" + OFC_Convert_string(OFC_FS_storage[i].data[0], false) + ">";
                        }
                    }
                    else
                    {
                        if (OFC_FS_storage[i].way == "")
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].name + ">" + OFC_Convert_string(OFC_FS_storage[i].data[0], false);
                        }
                        else
                        {
                            lines[lines.Length - 1] = "<" + OFC_FS_storage[i].way + "\\" + OFC_FS_storage[i].name + ">" + OFC_Convert_string(OFC_FS_storage[i].data[0], false);
                        }
                        for (ushort i_1 = 1; i_1 < OFC_FS_storage[i].data.Length; i_1++)
                        {
                            System.Array.Resize(ref lines, lines.Length + 1);
                            if (i_1 == OFC_FS_storage[i].data.Length - 1)
                            {
                                lines[lines.Length - 1] = OFC_Convert_string(OFC_FS_storage[i].data[i_1], false) + ">";
                            }
                            else
                            {
                                lines[lines.Length - 1] = OFC_Convert_string(OFC_FS_storage[i].data[i_1], false);
                            }
                        }
                    }
                }
                else
                {
                    if (OFC_FS_storage[i].way == "")
                    {
                        lines[lines.Length - 1] = "<" + OFC_FS_storage[i].name + "><>";
                    }
                    else
                    {
                        lines[lines.Length - 1] = "<" + OFC_FS_storage[i].way + "\\" + OFC_FS_storage[i].name + "><>";
                    }
                }
            }
            return lines;
        }
        public void OFC_Make_entry(string e_way, string e_name, OFC_Types e_type, string[] e_data)
        {
            if (e_type == OFC_Types.Folder)
            {
                int index_1 = OFC_get_up_folder(e_way);
                if (index_1 != -1)
                {
                    OFC_Entry_check(index_1, new OFC_Entry(e_way, e_name, OFC_Types.File), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name);
                }
                else
                {
                    OFC_Entry entry = new OFC_Entry(e_way, e_name, OFC_Types.Folder);
                    OFC_FS_storage.Add(entry);
                }
            }
            else
            {
                int index_1 = OFC_get_up_folder(e_way);
                if (index_1 != -1)
                {
                    OFC_Entry_check(index_1, new OFC_Entry(e_way, e_name, OFC_Types.File, e_data), OFC_Entry_check_param.Way | OFC_Entry_check_param.Name | OFC_Entry_check_param.Type | OFC_Entry_check_param.Data);
                }
                else
                {
                    OFC_Entry entry = new OFC_Entry(e_way, e_name, OFC_Types.File, e_data);
                    OFC_FS_storage.Add(entry);
                }
            }
        }
        string OFC_Convert_string(string input, bool restore)
        {
            if (restore)
            {
                input = input.Replace("/d", "/");
                input = input.Replace("/l", "<");
                input = input.Replace("/r", ">");
            }
            else
            {
                input = input.Replace("/", "/d");
                input = input.Replace("<", "/l");
                input = input.Replace(">", "/r");
            }
            return input;
        }
        string[] OFC_Split_way_name(string way_and_name)
        {
            string way = "", name = ""; string[] way_and_name_splitted = way_and_name.Split('\\');
            name = way_and_name_splitted[way_and_name_splitted.Length - 1];
            for (ushort i = 0; i < way_and_name_splitted.Length - 1; i++)
            {
                if (i == way_and_name_splitted.Length - 2)
                {
                    way += way_and_name_splitted[i];
                }
                else
                {
                    way += way_and_name_splitted[i] + "\\";
                }
            }
            way_and_name_splitted = null;
            return new string[2] { way, name };
        }
        int OFC_get_up_folder(string way)
        {
            string[] split_name = way.Split('\\');
            string[] split_way = new string[split_name.Length];
            split_way[0] = "";
            for (int i = 1; i < split_way.Length; i++)
            {
                split_way[i]= split_name[i-1];
            }
            for (int i = 2; i < split_way.Length; i++)
            {
                split_way[i] = split_way[i - 1] + "\\" + split_way[i];
            }
            for (int i = 0; i < OFC_FS_storage.Count; i++)
            {
                if (OFC_FS_storage[i].type == OFC_Types.Folder)
                {
                    for (int i1 = 0; i1 < split_way.Length; i1++)
                    {
                        if (OFC_FS_storage[i].way == split_way[i1] && OFC_FS_storage[i].name == split_name[i1])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        void OFC_Entry_check(int index, OFC_Entry new_entry_data, OFC_Entry_check_param param)
        {
            if (param.HasFlag(OFC_Entry_check_param.Way))
            {
                OFC_FS_storage[index].way = new_entry_data.way;
            }
            if (param.HasFlag(OFC_Entry_check_param.Name))
            {
                OFC_FS_storage[index].name = new_entry_data.name;
            }
            if (param.HasFlag(OFC_Entry_check_param.Type))
            {
                OFC_FS_storage[index].type = new_entry_data.type;
            }
            if (param.HasFlag(OFC_Entry_check_param.Data))
            {
                OFC_FS_storage[index].data = new_entry_data.data;
            }
        }
        public bool OFC_Entry_exist(string way, string name, OFC_Types e_type)
        {
            if (e_type == OFC_Types.File)
            {
                foreach (OFC_Entry i in OFC_FS_storage)
                {
                    if (i.way == way && i.name == name && i.type == OFC_Types.File)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (way == "")
                {
                    foreach (OFC_Entry i in OFC_FS_storage)
                    {
                        if ((i.way == way && i.name == name && i.type == OFC_Types.Folder) || (OFC_start_with(i.way, name)))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (OFC_Entry i in OFC_FS_storage)
                    {
                        if ((i.way == way && i.name == name && i.type == OFC_Types.Folder) || (OFC_start_with(i.way, way + "\\" + name)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        int OFC_Entry_get_index(string way, string name, OFC_Types e_type)
        {
            for (int i = 0; i < OFC_FS_storage.Count; i++)
            {
                if (OFC_FS_storage[i].way == way && OFC_FS_storage[i].name == name && OFC_FS_storage[i].type == e_type)
                {
                    return i;
                }
            }
            return -1;
        }
        public class OFC_ExistEntry_map_watch
        {
            OFC_ExistEntry_data_collection map; System.Collections.Generic.Dictionary<int, int> binder = new System.Collections.Generic.Dictionary<int, int>();
            public OFC_ExistEntry_map_watch(object exist_map)
            {
                map = exist_map as OFC_ExistEntry_data_collection; int index = 0;
                for (int i = 0; i < map.Collection.Count; i++)
                {
                    if (!(map.Collection[i].Poser_id == 1 && map.Collection[i].Type == OFC_Types.Folder))
                    {
                        binder.Add(index, i);
                        index++;
                    }
                }
            }
            public int Get_Lenght()
            {
                return binder.Count;
            }
            public OFC_ExistEntry_data Read_data(int index)
            {
                int answer;
                binder.TryGetValue(index, out answer);
                return map.Collection[answer];
            }
            public void Write_action(int index, OFC_ExistEntry_action action)
            {
                int answer;
                binder.TryGetValue(index, out answer);
                map.Collection[answer].Action = action;
            }
            public object Get_exist_map()
            {
                return map;
            }
        }
        public class OFC_ExistEntry_data
        {
            public OFC_ExistEntry_data(short poser_id, OFC_Types type, string e_way, string e_name, string d_way, string d_name)
            {
                Way = e_way; Name = e_name; Dest_entry_way = d_way; Dest_entry_name = d_name; Type = type; Poser_id = poser_id;
            }
            public readonly string Way, Name, Dest_entry_name, Dest_entry_way; public readonly short Poser_id; public readonly OFC_Types Type; public OFC_ExistEntry_action Action = OFC_ExistEntry_action.NoCopy;
        }
        class OFC_ExistEntry_data_collection
        {
            public OFC_ExistEntry_data_collection(System.Collections.Generic.List<OFC_ExistEntry_data> e_list)
            {
                Collection = e_list;
            }
            public System.Collections.Generic.List<OFC_ExistEntry_data> Collection;
        }
        public enum OFC_ExistEntry_action { NoCopy, Replace, SaveBoth }
        public enum OFC_Types { File, Folder }
        public enum OFC_Entry_check_param { Way = 1, Name = 2, Type = 4, Data = 8 }
        public class OFC_Entry
        {
            public OFC_Entry(string e_way, string e_name, OFC_Types e_type)
            {
                way = e_way; name = e_name; type = e_type;
            }
            public OFC_Entry(string e_way, string e_name, OFC_Types e_type, string[] e_data)
            {
                way = e_way; name = e_name; type = e_type; data = e_data;
            }
            public string way, name; public OFC_Types type;
            public string[] data = new string[0];
        }
    }
}
