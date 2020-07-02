using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpShell.SharpPreviewHandler;
using System.IO;
using System.Threading;

namespace N64ShellPreview
{
    public partial class N64PreviewHandlerControl : PreviewHandlerControl
    {
        public N64PreviewHandlerControl()
        {
            InitializeComponent();
        }

        public void DoPreview(string selectedFilePath)
        {
            StringWriter writer = new StringWriter();

            byte[] data = File.ReadAllBytes(selectedFilePath);
            N64Rom rom = new N64Rom(data);

            writer.WriteLine($"Endianness : {rom.Endiannes}");
            writer.WriteLine($"Clock Rate : {rom.GetClockRateString()}");
            writer.WriteLine($"Entrypoint : {rom.GetEntrypointString()}");
            writer.WriteLine($"Release Address : {rom.GetReleaseAddress()}");
            writer.WriteLine($"Libultra Version : {rom.GetLibultraVersion()}");
            writer.WriteLine($"CRC1 : {rom.GetCRC1String()}");
            writer.WriteLine($"CRC2 : {rom.GetCRC2String()}");
            writer.WriteLine($"Name : {rom.GetRomName()}");
            writer.WriteLine($"Game Code : {rom.GetGameCode()}");
            writer.WriteLine($"Rom Version : 0x{rom.GetRomVersion():X2}");
            writer.WriteLine($"CIC Chip : {rom.GetCicString()}");
            if (rom.GetCic() == 0)
                writer.WriteLine($"BootStrap MD5 : {rom.GetBootStrapMd5()}");

            textBox1.Text = writer.ToString();
        }
    }
}
