using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64ShellPreview
{
    public static class Utils
    {
        public static uint BomSwap(uint a) => (a << 24) | ((a & 0xFF00) << 8) | ((a & 0xFF0000) >> 8) | (a >> 24);
        public static string BytesToHex(byte[] data, string separator = " ") => BitConverter.ToString(data).Replace("-", separator);
    }
}
