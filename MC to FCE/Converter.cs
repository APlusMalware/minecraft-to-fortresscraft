using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Synergy.FCU;

namespace MC_to_FCE
{
	public class Converter
	{
		private String _terrainDataPath;
		private String _destinationDirectory;
		private IMapper _mapper;

		public Converter(String terrainDataPath, String destinationDirectory, IMapper mapper)
		{
			_terrainDataPath = terrainDataPath;
			_destinationDirectory = destinationDirectory;
			_mapper = mapper;
		}

		public void Begin(String sourceDirectory, String mappingFilePath, IProgress<String> progress)
		{
			progress.Report("Preparing to convert...\n");
			cleanSegmentDirectory();

			IDictionary<UInt16, CubeType> cubeTypes = CubeType.LoadFromFile(_terrainDataPath);
			Segment.CubeList = cubeTypes;
			_mapper.FCECubes = cubeTypes;
			_mapper.FCEDirectory = _destinationDirectory;

			List<String> unfoundNames = _mapper.LoadNameMap(mappingFilePath);
			foreach (String name in unfoundNames)
				progress.Report("Minecraft block \"" + name + "\" was not found. \n");
			progress.Report("Starting world conversion... (Step 1/3)\n");

			Int64 startConvertTime = DateTime.Now.Ticks;
			World world = _mapper.ConvertWorld(sourceDirectory);
			Int64 endConvertTime = DateTime.Now.Ticks;
			if (_mapper.UnknownBlocks.Count > 0)
			{
				progress.Report("The following Minecraft blocks did not have a mapping: \n");
				foreach (KeyValuePair<UInt16, String> unknownBlock in _mapper.UnknownBlocks)
					progress.Report(" ID: " + unknownBlock.Key + " Name: " + unknownBlock.Value + "\n");
			}
			progress.Report("World conversion finished " + ((endConvertTime - startConvertTime) / 10000000D) + " seconds. Beginning flag pass... (Step 2/3)\n");

			FlagPass.CubeTypes = cubeTypes;
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

		private void cleanSegmentDirectory()
		{
			if (Directory.Exists(_destinationDirectory + "Segments"))
			{
				Parallel.ForEach(Directory.GetDirectories(_destinationDirectory + "Segments"), (directory) =>
				{
					String[] dashSeperated = directory.Split('-');
					if (dashSeperated.Length == 4)
					{
						String[] slashSeperated = dashSeperated[0].Split('\\');
						if (slashSeperated[slashSeperated.Length - 1] == "d")
							Directory.Delete(directory, true);
					}
				});
				Parallel.ForEach(Directory.GetFiles(_destinationDirectory + "Segments"), (file) =>
				{
					String[] dashSeperated = file.Split('-');
					if (dashSeperated.Length == 4)
					{
						String[] slashSeperated = dashSeperated[0].Split('\\');
						if (slashSeperated[slashSeperated.Length - 1] == "d")
							File.Delete(file);
					}
				});
			}
		}
		}
}
