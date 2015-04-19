using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System.IO;
using Substrate;
using Substrate.Core;
using Synergy.FCU;
using System.Globalization;
using System.Threading;

namespace MC_to_FCE
{
    public class MinecraftConverter
    {
        IDictionary<UInt16, CubeType> fceCubes;
		Dictionary<UInt32, Cube> mcIdDataToFCECube;
		Dictionary<UInt16, String> unknownBlocks;
        private String _fceDirectory;

        private Int64 _totalSegments;
        private Int64 _segmentsLeft;

		private CancellationTokenSource _tokenSource = new CancellationTokenSource();
		private ConcurrentQueue<Segment> _saveQueue;
        private Task _saveTask;
        private CancellationToken _token;


        public MinecraftConverter(String fceDirectory, IDictionary<UInt16, CubeType> cubeTypes)
        {
			mcIdDataToFCECube = new Dictionary<UInt32, Cube>();
            unknownBlocks = new Dictionary<UInt16, String>();
            _fceDirectory = fceDirectory;
            fceCubes = cubeTypes;
            _saveQueue = new ConcurrentQueue<Segment>();

        }

        public List<String> LoadNameMap(String filePath)
        {
            XmlDocument DocumentMaps = new XmlDocument();
            ICacheTable<BlockInfo> mcBlockTable = Substrate.BlockInfo.BlockTable;
            Dictionary<String, String> mcNameToFCEName = new Dictionary<String, String>();
            Dictionary<String, UInt16> mcNameToId = new Dictionary<String, UInt16>();
            List<String> unfoundNames = new List<String>();

            DocumentMaps.Load(filePath);
            XmlNodeList elementsByTagName = DocumentMaps.GetElementsByTagName("Blocks");

            foreach (var mcBlock in mcBlockTable)
            {
                if (mcBlock.Registered)
                {
                    mcNameToId.Add(mcBlock.Name.ToUpper().Replace('(', ' ').Replace(")", " ").Replace(" ", ""), (UInt16)mcBlock.ID);
                }
            }

            foreach (XmlNode root in elementsByTagName)
            {
                foreach (XmlNode mcName in root.ChildNodes)
                {
                    String mcCleanName = mcName.Name.Replace("_", "");
                    UInt16 mcId;
                    UInt32 mcIdShifted;
                    if (!mcNameToId.TryGetValue(mcCleanName, out mcId))
                    {
                        // If the name isn't found, try to parse it as an id number directly
                        if (!UInt16.TryParse(mcCleanName, out mcId))
                        {
                            unfoundNames.Add(mcName.Name);
                            continue;
                        }
                    }
                    mcIdShifted = (UInt32)mcId << 16;

                    foreach (XmlNode mcValue in mcName.ChildNodes)
                    {
                        List<SByte> mcData = new List<SByte>();
                        String fceName = "";
                        UInt16 fceData = 0;
						UInt16 fceId = 0;
						Byte orientation = 0;
						foreach (XmlNode node in mcValue.ChildNodes)
                        {
							if (node.Name == "Value")
								mcData.Add(SByte.Parse(node.InnerText));
							else if (node.Name == "FCEId")
								fceId = UInt16.Parse(node.InnerText);
							else if (node.Name == "FCEName")
								fceName = node.InnerText;
							else if (node.Name == "FCEData")
								fceData = UInt16.Parse(node.InnerText, NumberStyles.HexNumber);
							else if (node.Name == "Orientation")
							{
								Char a = node.InnerText[0];
								Char b = node.InnerText[1];
								switch (a)
								{
									case 'N':
									case 'n':
										orientation = Cube.NORTH_FACE;
										break;
									case 'S':
									case 's':
										orientation = Cube.SOUTH_FACE;
										break;
									case 'E':
									case 'e':
										orientation = Cube.EAST_FACE;
										break;
									case 'W':
									case 'w':
										orientation = Cube.WEST_FACE;
										break;
									case 'A':
									case 'a':
										orientation = Cube.ABOVE_FACE;
										break;
									case 'B':
									case 'b':
										orientation = Cube.BELOW_FACE;
										break;
								}
								orientation += (Byte)(Byte.Parse(b.ToString()) << 6);
							}
						}

						CubeType cubeType;
						if (!fceCubes.TryGetValue(fceId, out cubeType))
						{
							if (fceId >= CubeType.MIN_DETAIL_TYPEID && fceId <= CubeType.MAX_DETAIL_TYPEID)
							{
								cubeType = generateDetailBlock(fceId);
								fceCubes.Add(new KeyValuePair<UInt16, CubeType>(fceId, cubeType));
							}
							else
							{
								cubeType = fceCubes.FirstOrDefault(c => c.Value.Name == fceName).Value;
								if (cubeType == null)
									continue;
							}
						}

                        UInt32 fceIdData = (UInt32)cubeType.TypeId << 16 | fceData;
                        foreach (SByte mcDatum in mcData)
                        {
                            UInt32 mcIdData = mcIdShifted;
                            if (mcDatum > 0)
                                mcIdData |= (Byte)mcDatum;
                            else
                                mcIdData |= 0x8000;	// This bit flags if we don't care what the data is.
							mcIdDataToFCECube[mcIdData] = new Cube(cubeType.TypeId, orientation, fceData, 13);
                        }
                    }
                }
            }
            return unfoundNames;
        }

        public World ConvertWorld(String mcDirectory)
        {
            String segmentDirectory = Path.Combine(_fceDirectory, "Segments");
            if (!Directory.Exists(_fceDirectory))
            {
                Directory.CreateDirectory(_fceDirectory);
            }
            if (!Directory.Exists(Path.Combine(_fceDirectory, segmentDirectory)))
            {
                Directory.CreateDirectory(segmentDirectory);
            }

			NbtWorld nbtWorld;
			String worldName;
			IChunkManager chunkManager;
			Boolean anvil = true;
			Int32 spawnChunkX;
            Int32 spawnChunkZ;

            nbtWorld = AnvilWorld.Open(mcDirectory);
            worldName = nbtWorld.Level.LevelName;
            chunkManager = nbtWorld.GetChunkManager();
            try
            {
                // Try to test for mc world type
                // Don't know how this is supposed to work, but it presumably throws an exception
                // on a non-Anvil world.
                chunkManager.Count<ChunkRef>();
            }
            catch
			{
				anvil = false;
				nbtWorld = BetaWorld.Open(mcDirectory);
				worldName = nbtWorld.Level.LevelName;
				chunkManager = nbtWorld.GetChunkManager();
			}	
			spawnChunkX = nbtWorld.Level.Spawn.X >> 4;
			spawnChunkZ = nbtWorld.Level.Spawn.Z >> 4;

            World fceWorld = new World(worldName, _fceDirectory);
            _totalSegments = chunkManager.LongCount() * 16;
            _segmentsLeft = _totalSegments;
            startSaveThread(fceWorld);
            foreach (ChunkRef chunk in chunkManager)
            {
                // If the save thread is too slow, wait until it has caught up before adding to it to prevent high ram usage
                while (_saveQueue.Count > 5000)
                {
                    Thread.Sleep(500);
                }

                Int32 spawnOffsetX = spawnChunkX - chunk.X;
                Int32 spawnOffsetZ = spawnChunkZ - chunk.Z;

                Int64 baseX = 4611686017890516944L + (Int64)(spawnOffsetX * 16);
                // Minecraft has different x/y directions so we must reverse z so the world isn't mirrored
                Int64 baseZ = 4611686017890516944L - (Int64)(spawnOffsetZ * 16);
                for (Int32 i = 0; i < (anvil ? 16 : 8); i++)
                {
                    Int64 baseY = 4611686017890516944L + (Int64)(i * 16) + 48;
                    Segment segment = new Segment(fceWorld, baseX, baseY, baseZ);
                    Cube[,,] array = new Cube[16, 16, 16];
                    for (Byte x = 0; x < 16; x++)
                    {
                        for (Byte y = 0; y < 16; y++)
                        {
                            for (Byte z = 0; z < 16; z++)
                            {
                                // Minecraft has different x/y directions so we must reverse z so the world isn't mirrored
                                AlphaBlock block = chunk.Blocks.GetBlock(15 - z, y + i * 16, x);
                                UInt32 mcIdData = (UInt32)block.ID << 16 | (UInt16)block.Data;
								
								Cube cube;
								if (!mcIdDataToFCECube.TryGetValue(mcIdData, out cube))
								{
									if (!mcIdDataToFCECube.TryGetValue((mcIdData | 0x8000) & 0xFFFF8000, out cube))
									{
                                        cube = new Cube(1, 0, 0, 0);
										if (!unknownBlocks.ContainsKey((UInt16)block.ID))
										{
											unknownBlocks.Add((UInt16)block.ID, block.Info.Name);
										}
									}
								}
								array[z, y, x] = cube;
                            }
                        }
                    }

                    segment.CubeData = array;
                    _segmentsLeft--;
                    _saveQueue.Enqueue(segment);
				}
				// Pad the area above the converted world with 11 blank segments to prevent world gen from occuring
				// Possibly replace this in the future with simply shifting the world up 
				for (Int32 i = (anvil ? 16 : 8); i < 27; i++)
				{
					// This is the center of the world. Why it's this is a complete mystery.
					Int64 baseY = 4611686017890516944L + (Int64)(i * 16) + 48;
					Segment padding = new Segment(fceWorld, baseX, baseY, baseZ);
					padding.CubeData = Segment.GetBlankSegment().CubeData;
					padding.IsEmpty = true;
					_saveQueue.Enqueue(padding);
				}
			}
            Task.WaitAll(_saveTask);

            return fceWorld;
        }

        private CubeType generateDetailBlock(UInt16 typeId)
        {
            return new CubeType(typeId, "Detail" + typeId, null, null, null, new List<ValueEntry>(), null, "PrimaryLayer", 2, 2, 2, 2, new List<Stage>(), null, true, false, true, false, false, false, false, true, false, null, "Dirt", "Dirt", "Dirt", new List<String>());
        }

        public void checkSubDir(World world, SegmentCoords coords)
        {
			SubdirectoryCoords subCoords = new SubdirectoryCoords(coords);
            string subDir = "d-" + subCoords.X.ToString("X") + "-" + subCoords.Y.ToString("X") + "-" + subCoords.Z.ToString("X");
            string path = world.SegmentPath + subDir;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void startSaveThread(World world)
		{
			ConcurrentQueue<Segment> queue = _saveQueue;
            _token = _tokenSource.Token;
            _saveTask = Task.Factory.StartNew(() =>
            {
                Segment segment;
                
                while (_segmentsLeft > 0 || queue.Count > 0)
                {
                    if (_token.IsCancellationRequested)
                        break;
                    if (queue.TryDequeue(out segment))
                    {
                        checkSubDir(world, segment.Coords);
                        FileStream fs = File.Open(Path.Combine(world.SegmentPath, segment.GetSegmentFileName()), FileMode.Create);
                        segment.WriteSegment(fs);
                        fs.Dispose();
                    }
                }
            }, _tokenSource.Token);
        }
    }
}
