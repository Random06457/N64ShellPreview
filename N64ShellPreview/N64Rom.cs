using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N64ShellPreview
{
    public enum N64Endiannes
    {
        BE,
        LE,
    }


    [Serializable]
    public class N64RomException : Exception
    {
        public N64RomException() { }
        public N64RomException(string message) : base(message) { }
        public N64RomException(string message, Exception inner) : base(message, inner) { }
        protected N64RomException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class N64Rom
    {
        private byte[] _rom;
        public N64Endiannes Endiannes { get; private set; }


        public N64Rom(byte[] rom)
        {
            _rom = rom;
            if (_rom[0] == 0x80 && _rom[1] == 0x37)
                Endiannes = N64Endiannes.BE;
            else if (_rom[0] == 0x37 && _rom[1] == 0x80)
            {
                Endiannes = N64Endiannes.LE;
                ByteSwap();
            }
            else
                throw new N64RomException("Invalid ROM");

        }

        private void ByteSwap()
        {
            for (int i = 0; i < _rom.Length; i+= 2)
            {
                byte b1 = _rom[i + 0], b2 = _rom[i + 1];
                _rom[i + 0] = b2;
                _rom[i + 1] = b1;
            }
        }


        public uint GetClockRate() => Utils.BomSwap(BitConverter.ToUInt32(_rom, 0x4)) & 0xFFFFFFF0;
        public string GetClockRateString()
        {
            uint clockRate = GetClockRate();
            return clockRate == 0 ? "Default" : clockRate + "Hz";
        }


        public uint GetEntrypoint() => Utils.BomSwap(BitConverter.ToUInt32(_rom, 0x8));
        public uint GetEntrypointFixed()
        {
            uint entrypoint = GetEntrypoint();
            int cic = GetCic();
            if (cic == 6103)
                entrypoint -= 0x100000;
            if (cic == 6106)
                entrypoint -= 0x200000;
            return entrypoint;
        }
        public string GetEntrypointString()
        {
            uint addr = GetEntrypoint();
            uint addrFixed = GetEntrypointFixed();
            return addr == addrFixed ? $"{addr:X8}" : $"{addr:X8}->{addrFixed:X8}";
        }

        public uint GetReleaseAddress() => Utils.BomSwap(BitConverter.ToUInt32(_rom, 0xC));

        public string GetBootStrapMd5() => Utils.BytesToHex(MD5.Create().ComputeHash(GetBootStrap()), "");
        public int GetCic()
        {
            string md5 = GetBootStrapMd5();
            switch (md5)
            {
                case "900B4A5B68EDB71F4C7ED52ACD814FC5": return 6101;
                case "E24DD796B2FA16511521139D28C8356B": return 6102;
                case "319038097346E12C26C3C21B56F86F23": return 6103;
                case "FF22A296E55D34AB0A077DC2BA5F5796": return 6105;
                case "6460387749AC0BD925AA5430BC7864FE": return 6106;
                default: return 0;
            }
        }
        public string GetCicString()
        {
            int cic = GetCic();

            switch (cic)
            {
                case 6101: return "CIC-NUS-6101/7102";
                case 6102: return "CIC-NUS-6102/7101";
                case 6103: return "CIC-NUS-6103/7103";
                case 6105: return "CIC-NUS-6105/7105";
                case 6106: return "CIC-NUS-6106/7106";
                default: return "Unknown CIC";
            }
        }

        public char GetLibultraVersion() => (char)_rom[0xF];

        public uint GetCRC1() => Utils.BomSwap(BitConverter.ToUInt32(_rom, 0x10));
        public string GetCRC1String()
        {
            uint crc = GetCRC1();
            int cic = GetCic();
            if (cic == 0) return $"{crc:X8}";
            var sum = N64CheckSum.Compute(_rom, cic);
            return sum.Item1 == crc ? $"{crc:X8} (VALID)" : $"{crc:X8} (INVALID)";
        }
        public uint GetCRC2() => Utils.BomSwap(BitConverter.ToUInt32(_rom, 0x14));
        public string GetCRC2String()
        {
            uint crc = GetCRC2();
            int cic = GetCic();
            if (cic == 0) return $"{crc:X8}";
            var sum = N64CheckSum.Compute(_rom, cic);
            return sum.Item2 == crc ? $"{crc:X8} (VALID)" : $"{crc:X8} (INVALID)";
        }

        public string GetRomName() => Encoding.ASCII.GetString(_rom, 0x20, 0x14);

        public string GetGameCode() => Encoding.ASCII.GetString(_rom, 0x3B, 4);

        public byte GetRomVersion() => _rom[0x3F];

        public byte[] GetBootStrap()
        {
            byte[] bootstrap = new byte[0xFC0];
            Buffer.BlockCopy(_rom, 0x40, bootstrap, 0, bootstrap.Length);
            return bootstrap;
        }

    }
}
