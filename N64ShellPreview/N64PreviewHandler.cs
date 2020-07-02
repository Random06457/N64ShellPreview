using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpPreviewHandler;

namespace N64ShellPreview
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".z64", ".n64")]
    [DisplayName("N64 Preview Handler")]
    [PreviewHandler]
    public class N64PreviewHandler : SharpPreviewHandler
    {
        public N64PreviewHandler() : base()
        {
            Application.EnableVisualStyles();
        }
        protected override PreviewHandlerControl DoPreview()
        {
            var handler = new N64PreviewHandlerControl();

            if (!string.IsNullOrEmpty(SelectedFilePath))
                handler.DoPreview(SelectedFilePath);

            return handler;
        }
    }
}
