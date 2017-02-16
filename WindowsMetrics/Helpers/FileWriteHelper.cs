using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsMetrics.Helpers
{
    public static class FileWriteHelper
    {
        public static void Write(string text, string path)
        {
            using (FileStream fs = File.Exists(path) ? File.Open(path, FileMode.Append) : File.Create(path))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(text);
                }
            }
        }
    }
}
