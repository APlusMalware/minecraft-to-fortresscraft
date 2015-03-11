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
    public partial class MCToFCEForm : Form
    {
        private MinecraftConverter _converter;
        private World world; 

        public MCToFCEForm()
        {
            InitializeComponent();
        }


        private void MCToFCENamePathButton_click(object sender, EventArgs e)
        {
            if (MCToFCENamePathDialog.ShowDialog() == DialogResult.OK)
            {
                MCToFCENamePathInput.Text = MCToFCENamePathDialog.FileName;
            }
        }

        private void MinecraftWorldPathButton_click(object sender, EventArgs e)
        {
            if (MinecraftWorldPathDialog.ShowDialog() == DialogResult.OK)
            {
                MinecraftWorldPathInput.Text = MinecraftWorldPathDialog.FileName;
            }
        }

        private void FortressCraftWorldPathButton_click(object sender, EventArgs e)
        {
            if (FortressCraftWorldPathDialog.ShowDialog() == DialogResult.OK)
            {
                FortressCraftWorldPathInput.Text = FortressCraftWorldPathDialog.FileName;
            }
        }

        private void FortressCraftTerrainDataPathButton_Click(object sender, EventArgs e)
        {
            if (FortressCraftTerrainDataPathDialog.ShowDialog() == DialogResult.OK)
            {
                FortressCraftTerrainDataPathInput.Text = FortressCraftTerrainDataPathDialog.FileName;
            }
        }

        private void MCToFCEForm_Load(object sender, EventArgs e)
        {
            MCToFCENamePathInput.Text = Path.Combine(Directory.GetCurrentDirectory(), "MCNamesToFCENames.xml");
            MinecraftWorldPathInput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
            FortressCraftWorldPathInput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "ProjectorGames", "FortressCraft", "Worlds");
            FortressCraftTerrainDataPathInput.Text = Path.Combine((String)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 254200", "InstallLocation", null), @"32\Default\Data", @"TerrainData.xml");

            MCToFCENamePathDialog.InitialDirectory = MCToFCENamePathInput.Text;
            MinecraftWorldPathDialog.InitialDirectory = MinecraftWorldPathInput.Text;
            FortressCraftWorldPathDialog.InitialDirectory = FortressCraftWorldPathInput.Text;
            FortressCraftTerrainDataPathDialog.InitialDirectory = FortressCraftTerrainDataPathInput.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String fceDirectory = Directory.GetParent(FortressCraftWorldPathInput.Text).FullName + @"\";
            String mcDirectory = Directory.GetParent(MinecraftWorldPathInput.Text).FullName + @"\";
            String mcNamesToFCENamesDirectory = MCToFCENamePathInput.Text;
            String terainDataPath = FortressCraftTerrainDataPathInput.Text;
            if (Directory.Exists(fceDirectory + "Segments"))
                Directory.Delete(fceDirectory + "Segments", true);

            logBox.AppendText("Loading data files...\n");
            CubeType.LoadFCETerrainData(terainDataPath);
            _converter = new MinecraftConverter(fceDirectory);
            List<String> unfoundNames = _converter.LoadNameMap(mcNamesToFCENamesDirectory);
            foreach (String name in unfoundNames)
                logBox.AppendText("Minecraft block name \"" + name + "\" was not found. \n");

            logBox.AppendText("Starting world conversion... (Step 1/3)\n");
            Int64 startConvertTime = DateTime.Now.Ticks;
            world = _converter.ConvertWorld(mcDirectory);
            Int64 endConvertTime = DateTime.Now.Ticks;
            logBox.AppendText("World conversion finished " + ((endConvertTime - startConvertTime) / 10000000D) + " seconds. Beginning flag pass... (Step 2/3)\n");

            List<String> failed = FlagPass.FixCubeFlags(world);
            Int64 endFlagTime = DateTime.Now.Ticks;
            if (failed.Count > 0)
                logBox.AppendText("Could not fix the following segment files: \n");
            foreach (String fileName in failed)
                logBox.AppendText(fileName + "\n");
            logBox.AppendText("Flag pass finished in " + ((endFlagTime - endConvertTime) / 10000000D) + " seconds. Beginning world zip... (Step 3/3)\n");

            world.Zip();
            Int64 endZipTime = DateTime.Now.Ticks;
            logBox.AppendText("World zip finished in " + ((endZipTime - endFlagTime) / 10000000D) + " seconds.\n");
            logBox.AppendText("Conversion complete!\n");
        }
    }
}
