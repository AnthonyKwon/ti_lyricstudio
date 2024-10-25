using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ti_Lyricstudio
{
    public partial class DemoWindow : Form
    {
        public DemoWindow()
        {
            InitializeComponent();
            ToolBar.Renderer = new ToolStripOverride();
        }
    }

    public class ToolStripOverride : ToolStripProfessionalRenderer
    {
        public ToolStripOverride() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }
}
