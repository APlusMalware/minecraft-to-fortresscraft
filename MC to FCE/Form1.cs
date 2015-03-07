using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Synergy.FCU;

namespace MC_to_FCE
{
    public partial class Form1 : Form
    {
        private MinecraftConverter _converter;
        private World world; 

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Int64 start = DateTime.Now.Ticks;
            _converter = new MinecraftConverter(textBox3.Text);
            System.IO.Directory.Delete(textBox3.Text + "Segments", true);
            _converter.LoadFCETerrainData();
            _converter.LoadNameMap(@"r:\documents\visual studio 2013\Projects\MC to FCE\MC to FCE\MCNamesToFCENames.xml");
            world = _converter.ConvertWorld(textBox2.Text);
            _converter.FixCubeFlags(world);
            Int64 end = DateTime.Now.Ticks;
            _converter.BeginZip(world);
            textBox4.Text = ((end - start) / 10000000D).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _converter.FixCubeFlags(world);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _converter.BeginZip(world);
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
