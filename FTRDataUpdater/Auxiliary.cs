namespace FTPDataUpdater
{
    using System;
    using System.Runtime.InteropServices;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    static class Auxiliary
    {
        public static string Construct_str_by_file_arr(string[] lines)
        {
            string result = "\"" + lines[0].Substring(lines[0].LastIndexOf('\\') + 1) + "\"";
            for (ushort i = 1; i < lines.Length; i++)
            {
                result += ", \"" + lines[i].Substring(lines[i].LastIndexOf('\\') + 1) + "\"";
            }
            return result;
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
                if (split_arr[0]== key)
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
            string []lines = text.Split('\n');
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
            for (ushort i=0;i< lines.Length; i++) { text += lines[i] + "\r\n"; }
            return text;
        }
        public static string Get_text_from_lines(string[] lines)
        {
            string text = "";
            for (ushort i = 0; i < lines.Length; i++) { text += lines[i] + "\n"; }
            return text.Remove(text.Length - 1, 1);
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
        public static DateTime GetNetworkTime()
        {
            const string ntpServer = "pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 3000;
                socket.Send(ntpData);
                socket.Receive(ntpData);
            }
            const byte serverReplyTime = 40;
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }
    }
}