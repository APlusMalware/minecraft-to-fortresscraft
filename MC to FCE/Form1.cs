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
		private MinecraftConverter _converter;
		private World world;
		private String _fceDirectory;
		private String _mcDirectory;
		private String _mceNamesToFCENamesPath;
		private String _terrainDataPath;
		private Boolean _mapUnknownsToDetailBlock;

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
			_fceDirectory = Directory.GetParent(FortressCraftWorldPathInput.Text).FullName + Path.DirectorySeparatorChar;
			_mcDirectory = Directory.GetParent(MinecraftWorldPathInput.Text).FullName + Path.DirectorySeparatorChar;
			_mceNamesToFCENamesPath = MCToFCENamePathInput.Text;
			_mapUnknownsToDetailBlock = MapUnknownBlocksToDetailBlockInput.Checked;
			if (!File.Exists(_mceNamesToFCENamesPath))
			{
				MessageBox.Show("The specified Cube Mapping file could not be found.");
				return;
			}
			_terrainDataPath = FortressCraftTerrainDataPathInput.Text;
			if (!File.Exists(_terrainDataPath))
			{
				MessageBox.Show("The specified Terrain Data file could not be found.");
				return;
			}
			StartConvertButton.Enabled = false;
			var progress = new Progress<String>(s => logBox.AppendText(s));
			await Task.Factory.StartNew(() => startConvert(progress));
			StartConvertButton.Enabled = true;
		}

		private void startConvert(IProgress<String> progress)
		{
			if (Directory.Exists(_fceDirectory + "Segments"))
				Directory.Delete(_fceDirectory + "Segments", true);

			progress.Report("Loading data files...\n");
			IDictionary<UInt16, CubeType> cubeTypes = CubeType.LoadFromFile(_terrainDataPath);
			Segment.CubeList = cubeTypes;
			_converter = new MinecraftConverter(_fceDirectory, cubeTypes);
			_converter.UnknownsMapToDetail = _mapUnknownsToDetailBlock;
			List<String> unfoundNames = _converter.LoadNameMap(_mceNamesToFCENamesPath);
			foreach (String name in unfoundNames)
				progress.Report("Minecraft block name \"" + name + "\" was not found. \n");
			progress.Report("Starting world conversion... (Step 1/3)\n");

			Int64 startConvertTime = DateTime.Now.Ticks;
			world = _converter.ConvertWorld(_mcDirectory);
			Int64 endConvertTime = DateTime.Now.Ticks;
			progress.Report("World conversion finished " + ((endConvertTime - startConvertTime) / 10000000D) + " seconds. Beginning flag pass... (Step 2/3)\n");
			
			List<String> failed = FlagPass.FixWorld(world);
			Int64 endFlagTime = DateTime.Now.Ticks;
			if (failed.Count > 0)
				progress.Report("Could not fix the following segment files: \n");
			foreach (String fileName in failed)
				progress.Report(Path.GetFileName(fileName) + "\n");
			progress.Report("Flag pass finished in " + ((endFlagTime - endConvertTime) / 10000000D) + " seconds. Beginning world zip... (Step 3/3)\n");

			world.Zip();
			Int64 endZipTime = DateTime.Now.Ticks;
			progress.Report("World zip finished in " + ((endZipTime - endFlagTime) / 10000000D) + " seconds.\n");
			progress.Report("Conversion complete!\n");
		}
	}
}
