namespace UniversalInstaller
{
    static class Variables
    {
        const float CURRENT_VERSION = 1.0F;
        public static readonly byte[] Encryption_IV = new byte[] { 128, 249, 91, 4, 207, 204, 113, 130, 233, 89, 174, 204, 124, 84, 226, 36 };
        public static readonly byte[] Encryption_KEY = new byte[] { 42, 113, 20, 67, 13, 195, 115, 6, 128, 12, 99, 67, 192, 236, 149, 40 };
        public static readonly string[] SUPPORT_VERSIONS = new string[] { "1,0" };
        public static readonly string MY_WAY = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
        public static string[] components = new string[5], prog_str = new string[3] { null, "", "" }, al_commands = new string[0]; public static ushort[] time_func = new ushort[6];
        public static OFC_FS local_database = new OFC_FS();
        public static System.Collections.Generic.List<Server_entry> servers = new System.Collections.Generic.List<Server_entry>();
        public static System.Collections.Generic.List<Program_entry> programs = new System.Collections.Generic.List<Program_entry>();
        public class Server_entry
        {
            public ushort number; public string ip, user_name, password;
            public Server_entry(ushort number, string ip, string user_name, string password)
            {
                this.number = number; this.ip = ip; this.user_name = user_name; this.password = password;
            }
        }
        public class Program_entry
        {
            public string name; public string[] always_commands = new string[0], p_data = new string[6]; public bool[] props = new bool[9]; public float version;
            public Program_entry(string name, float version, string[] p_data, bool[] props, string[] always_commands)
            {
                this.name = name; this.version = version; this.p_data = p_data; this.props = props; this.always_commands = always_commands;
            }
            public Program_entry(){}
        }
    }
}
