using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Synergy;
using Synergy.Installation;

namespace MC_to_FCE
{
	public class Converter
	{
		private readonly String _terrainDataPath;
		private readonly String _destinationDirectory;
		private readonly IMapper _mapper;

		public Converter(String terrainDataPath, String destinationDirectory, IMapper mapper)
		{
			_terrainDataPath = terrainDataPath;
			_destinationDirectory = destinationDirectory;
			_mapper = mapper;
		}

		public void Begin(String sourceDirectory, String mappingFilePath, IProgress<String> progress)
		{
			progress.Report("Preparing to convert...\n");
			CleanSegmentDirectory();

		    var cubeTypeSerializer = new XmlSerializer(typeof (CubeType));
		    IDictionary<UInt16, CubeType> cubeTypes;
            using (Stream stream = File.OpenRead(_terrainDataPath))
                cubeTypes = CubeType.CreateDictionary((CubeType[])cubeTypeSerializer.Deserialize(stream));
			_mapper.FCECubes = cubeTypes;
			_mapper.FCEDirectory = _destinationDirectory;

			List<String> unfoundNames = _mapper.LoadNameMap(mappingFilePath);
			foreach (String name in unfoundNames)
				progress.Report("Minecraft block \"" + name + "\" was not found. \n");
			progress.Report("Starting world conversion... (Step 1/3)\n");

			var mapStopwatch = Stopwatch.StartNew();
			World world = _mapper.ConvertWorld(sourceDirectory);
		    mapStopwatch.Stop();

			if (_mapper.UnknownBlocks.Count > 0)
			{
				progress.Report("The following Minecraft blocks did not have a mapping: \n");
				foreach (KeyValuePair<UInt16, String> unknownBlock in _mapper.UnknownBlocks)
					progress.Report(" ID: " + unknownBlock.Key + " Name: " + unknownBlock.Value + "\n");
			}
			progress.Report("World conversion finished " + (mapStopwatch.ElapsedMilliseconds / 1000D) + " seconds. Beginning flag pass... (Step 2/3)\n");

			FlagPass.CubeTypes = cubeTypes;
		    var flagPassStopwatch = Stopwatch.StartNew();
			List<String> failed = FlagPass.FixWorld(world);
		    flagPassStopwatch.Stop();

			if (failed.Count > 0)
				progress.Report("Could not fix the following segment files: \n");
			foreach (String fileName in failed)
				progress.Report(Path.GetFileName(fileName) + "\n");
			progress.Report("Flag pass finished in " + (flagPassStopwatch.ElapsedMilliseconds / 1000D) + " seconds. Beginning world zip... (Step 3/3)\n");

            var worldZipStopwatch = Stopwatch.StartNew();
            world.Zip();
		    worldZipStopwatch.Stop();

			progress.Report("World zip finished in " + (worldZipStopwatch.ElapsedMilliseconds / 1000D) + " seconds.\n");
			progress.Report("Conversion complete!\n");
		}

		private void CleanSegmentDirectory()
		{
		    if (!Directory.Exists(_destinationDirectory + "Segments"))
		        return;

			Parallel.ForEach(Directory.GetDirectories(_destinationDirectory + "Segments"), directory =>
			{
				String[] dashSeperated = directory.Split('-');
			    if (dashSeperated.Length != 4)
			        return;
				String[] slashSeperated = dashSeperated[0].Split('\\');
				if (slashSeperated[slashSeperated.Length - 1] == "d")
					Directory.Delete(directory, true);
			});
			Parallel.ForEach(Directory.GetFiles(_destinationDirectory + "Segments"), file =>
			{
				String[] dashSeperated = file.Split('-');
			    if (dashSeperated.Length != 4)
			        return;
				String[] slashSeperated = dashSeperated[0].Split('\\');
				if (slashSeperated[slashSeperated.Length - 1] == "d")
					File.Delete(file);
			});
		}
	}
}
