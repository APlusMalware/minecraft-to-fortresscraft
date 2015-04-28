using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using Synergy.FCU;

namespace MC_to_FCE
{
	public partial class MCToFCEForm : Form
	{
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
			FortressCraftWorldPathInput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "ProjectorGames", "FortressCraft", "Worlds");
			MinecraftWorldPathInput.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
			MCToFCENamePathInput.Text = Path.Combine(Directory.GetCurrentDirectory(), "MCNamesToFCENames.xml");
			FortressCraftTerrainDataPathInput.Text = Path.Combine((String)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 254200", "InstallLocation", null), @"32\Default\Data", @"TerrainData.xml");

			FortressCraftWorldPathDialog.InitialDirectory = FortressCraftWorldPathInput.Text;

			MinecraftWorldPathDialog.InitialDirectory = MinecraftWorldPathInput.Text;

			MCToFCENamePathDialog.InitialDirectory = Directory.GetParent(MCToFCENamePathInput.Text).FullName;
			MCToFCENamePathDialog.FileName = Path.GetFileName(MCToFCENamePathInput.Text);

			FortressCraftTerrainDataPathDialog.InitialDirectory = Directory.GetParent(FortressCraftTerrainDataPathInput.Text).FullName;
			FortressCraftTerrainDataPathDialog.FileName = Path.GetFileName(FortressCraftTerrainDataPathInput.Text);
		}

		private async void StartConvertButton_click(object sender, EventArgs e)
		{
			Progress<String> progress = new Progress<String>(s => logBox.AppendText(s));

			Boolean useSpawnAsOrigin = UseSpawnAsOriginCheckBox.Checked;

			String fceDirectory = Directory.GetParent(FortressCraftWorldPathInput.Text).FullName + Path.DirectorySeparatorChar;
			String mcDirectory = Directory.GetParent(MinecraftWorldPathInput.Text).FullName + Path.DirectorySeparatorChar;
			String mcNamesToFCENames = MCToFCENamePathInput.Text;
			String terrainDataPath = FortressCraftTerrainDataPathInput.Text;

			if (!File.Exists(mcNamesToFCENames))
			{
				MessageBox.Show("The specified Cube Mapping file could not be found.");
				return;
			}
			if (!File.Exists(terrainDataPath))
			{
				MessageBox.Show("The specified Terrain Data file could not be found.");
				return;
			}
			if (!Directory.Exists(mcDirectory))
			{
				MessageBox.Show("The specified Minecraft world could not be found.");
				return;
			}
			if (!Directory.Exists(fceDirectory))
			{
				MessageBox.Show("The specified FortressCraft world could not be found.");
				return;
			}

			MinecraftMapper mapper = new MinecraftMapper(useSpawnAsOrigin);
			Converter converter = new Converter(terrainDataPath, fceDirectory, mapper);

			StartConvertButton.Enabled = false;
			await Task.Factory.StartNew(() => converter.Begin(mcDirectory, mcNamesToFCENames, progress));
			StartConvertButton.Enabled = true;
		}
	}
}
