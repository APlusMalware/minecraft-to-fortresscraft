    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            if (Directory.Exists(textBox3.Text + "Segments"))
                Directory.Delete(textBox3.Text + "Segments", true);
            CubeType.LoadFCETerrainData();
            _converter = new MinecraftConverter(textBox3.Text);
            Int64 startConvertTime = DateTime.Now.Ticks;
            _converter.LoadNameMap(@"r:\documents\visual studio 2013\Projects\MC to FCE\MC to FCE\MCNamesToFCENames.xml");

            world = _converter.ConvertWorld(textBox2.Text);
            Int64 endConvertTime = DateTime.Now.Ticks;
            textBox4.Text = ((endConvertTime - startConvertTime) / 10000000D).ToString();

            List<String> failed = FlagPass.FixCubeFlags(world);
            Int64 endFlagTime = DateTime.Now.Ticks;
            textBox5.Text = ((endFlagTime - endConvertTime) / 10000000D).ToString();

            foreach (String fileName in failed)
                logBox.AppendText(failed + "/n");

            world.Zip();
            Int64 endZipTime = DateTime.Now.Ticks;
            textBox6.Text = ((endZipTime - endFlagTime) / 10000000D).ToString();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            world = new World("New World 2", textBox3.Text);
            CubeType.LoadFCETerrainData();
            Int64 endConvertTime = DateTime.Now.Ticks;
            FlagPass.FixCubeFlags(world);
            Int64 endFlagTime = DateTime.Now.Ticks;
            textBox5.Text = ((endFlagTime - endConvertTime) / 10000000D).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            world.Zip();
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
