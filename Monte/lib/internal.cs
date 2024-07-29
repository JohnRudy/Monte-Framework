#if WINDWOS
using System.Managment;
#endif

using System.Text;


namespace Monte.Lib
{
    // This is just generic bs things. Not to be used religiously but just to get by for some info
    internal static class InternalLib
    {
        internal static string GetCPUInfo()
        {
            StringBuilder sb = new();
#if WINDOWS
            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                sb.Append(item["Name"]);
            }
#endif
#if LINUX
            string[] lines = File.ReadAllLines("/proc/cpuinfo");
            foreach (string line in lines)
            {
                if (line.StartsWith("model name"))
                {
                    if (!sb.ToString().Contains("model name"))
                        sb.Append(line);
                }
            }
#endif
            return sb.ToString();
        }

        internal static string GetGPUInfo()
        {
            StringBuilder sb = new();
#if WINDOWS
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (var item in searcher.Get())
                {
                    sb.Append("Name: {0}", item["Name"]);
                    sb.Append("Driver Version: {0}", item["DriverVersion"]);
                    sb.Append("Status: {0}", item["Status"]);
                }
            }
            catch { }
#endif
#if LINUX
            try
            {
                if (Directory.Exists("/proc/driver/nvidia/gpus"))
                {
                    foreach (var dir in Directory.GetDirectories("/proc/driver/nvidia/gpus"))
                    {
                        string gpuInfo = File.ReadAllText(Path.Combine(dir, "information"));
                        sb.Append(gpuInfo);
                    }
                }
            }
            catch
            {
                sb.Append("Probably a AMD gpu. I dunno");
            }
            return sb.ToString();
        }
#endif
    }
}