using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Services;

namespace TrackAFaceWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var bridge = new TrackAFaceWinForms.Services.PythonBridge();
            MessageBox.Show(bridge.GetDiagnostic(), "Configuration Track-A-FACE");
        }
    }
}
