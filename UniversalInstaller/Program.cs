using System;

namespace UniversalInstaller
{
    static class Program
    {
        public static readonly string[] SUPPORT_VERSIONS = new string[] { "1,0" }; 
        public static FTP ftp_client = null; public static string ftp_way_str = null;
        static void Main()
        {
            System.Threading.Mutex nmp = new System.Threading.Mutex(true, "{9ea772e0-7ff4-4977-a3a1-410532607420}", out bool runned);
            if (runned)
            {
                Code_1.Load_database();
                if (Variables.servers.Count == 0 && Variables.local_database.OFC_get_names("Progs", OFC_FS.OFC_Types.Folder).Length == 0) return;
                Code_1.Correct_database();
                System.Windows.Forms.MessageBox.Show(Code_1.First_try_connect(Auxiliary.Get_server_by_id(1)).ToString());

                //System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.FileName = Constant.MY_WAY + "\\" + Constant.MY_NAME;
                //proc.Start();
                //Encryption.EncryptStringToBytes_Aes("123", Constant.Encryption_KEY, Constant.Encryption_IV);
                //Encryption.DecryptStringFromBytes_Aes(Encoding.Default.GetBytes(Auxiliary.Get_text_from_lines(filesys[0].OFC_FileData_read(rtb_way, rtb_name))), Encryption.Encryption_KEY, Encryption.Encryption_IV);
                //Encoding.Default.GetString(Encryption.EncryptStringToBytes_Aes(richTextBox1.Text, Encryption.Encryption_KEY, Encryption.Encryption_IV))
            }
        }
        
    }
}
