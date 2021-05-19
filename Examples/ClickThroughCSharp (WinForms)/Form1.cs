using Geode.Extension;
using System;
using System.Windows.Forms;

namespace ClickThroughCSharp
{
    public partial class Form1 : Form
    {
        GeodeExtension Ext;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Ext = new GeodeExtension("ClickThrough", "Geode examples.", "Lilith");
            Ext.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Ext.IsConnected)
            {
                Ext.SendToClientAsync(Ext.In.YouArePlayingGame, true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Ext.IsConnected)
            {
                Ext.SendToClientAsync(Ext.In.YouArePlayingGame, false);
            }
        }
    }
}
