using System;
using System.IO;
using System.Windows.Forms;

namespace MC_to_FCE
{
    static class Program
    {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(String[] args)
		{
			if (args.Length > 3)
			{
				ProcessArguments(args);
				Console.Read();
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MCToFCEForm());
			}
        }

		private static void ProcessArguments(String[] args)
		{
			String fceDirectory = args[0];
			String mcDirectory = args[1];
			String terrainDataPath = args[2];
			String nameMapPath = args[3];
			Boolean useSpawnAsOrigin = !(args.Length > 4 && args[4] == "-o");

			var progress = new Progress<String>(Console.WriteLine);

			if (!File.Exists(nameMapPath))
			{
				MessageBox.Show("The specified Cube Mapping file could not be found.");
				return;
			}
			if (!File.Exists(terrainDataPath))
			{
				MessageBox.Show("The specified Terrain Data file could not be found. ");
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

			var mapper = new MinecraftMapper(useSpawnAsOrigin);
			var converter = new Converter(terrainDataPath, fceDirectory, mapper);
			
			converter.Begin(mcDirectory, nameMapPath, progress);
		}
    }
}
