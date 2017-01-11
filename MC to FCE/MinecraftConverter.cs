using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using Substrate;
using Substrate.Core;
using System.Globalization;
using System.Threading;
using Synergy;
using Synergy.Installation;

namespace MC_to_FCE
{
    public class MinecraftMapper : IMapper
    {
		private readonly Dictionary<UInt32, Cube> _mcIdDataToFCECube;

        private Int64 _totalSegments;
        private Int64 _segmentsLeft;

		private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
		private readonly ConcurrentQueue<Segment> _saveQueue;
        private Task _saveTask;
        private CancellationToken _token;

		public Boolean UseSpawnAsOrigin { get; set; }
		public IDictionary<UInt16, String> UnknownBlocks { get; set; }

		public IDictionary<UInt16, CubeType> FCECubes { get; set; }
		public String FCEDirectory { get; set; }

		public MinecraftMapper(Boolean useSpawnAsOrigin)
        {
			_mcIdDataToFCECube = new Dictionary<UInt32, Cube>();
            UnknownBlocks = new Dictionary<UInt16, String>();
            _saveQueue = new ConcurrentQueue<Segment>();
			UseSpawnAsOrigin = useSpawnAsOrigin;
        }

        public List<String> LoadNameMap(String filePath)
        {
            Dictionary<String, UInt16> mcNameToId = BlockInfo.BlockTable.Where(info => info.Registered).ToDictionary(info => info.Name, info => (UInt16)info.ID);
            var unfoundNames = new List<String>();

            var document = XDocument.Load(filePath);
            var root = document.Element("MinecraftBlocks");
            if (root == null)
                return unfoundNames;
            
            foreach (var mcBlock in root.Elements("Block"))
            {
                var mcNameElement = mcBlock.Element("MCName");
                String mcName = mcNameElement?.Value ?? "";
                UInt16 mcId;
                if (!mcNameToId.TryGetValue(mcName, out mcId))
                {
                    // If the name isn't found, use the id number
                    var mcIdElement = mcBlock.Element("MCId");
                    if (mcIdElement == null || !UInt16.TryParse(mcIdElement.Value, out mcId))
                    {
                        unfoundNames.Add(mcName + " : " + mcBlock.Element("MCId"));
                        continue;
                    }
                }
                UInt32 mcIdShifted = (UInt32)mcId << 16;

                foreach (var mcValue in mcBlock.Elements("MCValue"))
                {
                    var mcData = new List<SByte>();
					if (mcValue.Elements("Value").Any())
					{
					    mcData.AddRange(mcValue.Elements("Value").Select(e => SByte.Parse(e.Value)));
					}
					else
						continue;

                    var nameElement = mcValue.Element("FCEName");
                    var dataElement = mcValue.Element("FCEData");
                    var idElement = mcValue.Element("FCEId");
                    var orientationElement = mcValue.Element("Orientation");

                    String fceName = nameElement?.Value ?? String.Empty;
                    UInt16 fceData = UInt16.Parse(dataElement?.Value ?? "0", NumberStyles.HexNumber);

					UInt16 fceId = UInt16.Parse(idElement?.Value ?? "0");
					Byte orientation = 0;

					if (orientationElement != null)
					{
						Char a = orientationElement.Value[0];
						Char b = orientationElement.Value[1];
						switch (a)
						{
							case 'N':
							case 'n':
								orientation = Cube.NorthFace;
								break;
							case 'S':
							case 's':
								orientation = Cube.SouthFace;
								break;
							case 'E':
							case 'e':
								orientation = Cube.EastFace;
								break;
							case 'W':
							case 'w':
								orientation = Cube.WestFace;
								break;
							case 'A':
							case 'a':
								orientation = Cube.AboveFace;
								break;
							case 'B':
							case 'b':
								orientation = Cube.BelowFace;
								break;
						}
						orientation += (Byte)(Byte.Parse(b.ToString()) << 6);
					}

					CubeType cubeType;
					if (!FCECubes.TryGetValue(fceId, out cubeType))
					{
						if (fceId >= CubeType.MinDetailTypeId && fceId <= CubeType.MaxDetailTypeId)
						{
							cubeType = GenerateDetailBlock(fceId);
							FCECubes.Add(new KeyValuePair<UInt16, CubeType>(fceId, cubeType));
						}
						else
						{
							cubeType = FCECubes.FirstOrDefault(c => c.Value.Name == fceName).Value;
							if (cubeType == null)
								continue;
						}
					}

					// Check if there are any negative minecraft data values and skip all others if there are
					if (mcData.Any(data => data < 0))
					{
						// Set all id-data combinations equal to this value
						for (UInt16 i = 0; i < 16; i++)
						{
							UInt32 mcIdData = mcIdShifted;
							mcIdData |= i;
							_mcIdDataToFCECube[mcIdData] = new Cube(cubeType.TypeId, orientation, fceData, 13);
						}
					}
					else
					{
						foreach (UInt32 mcIdData in mcData.Select(data => mcIdShifted | (Byte)data))
						{
							_mcIdDataToFCECube[mcIdData] = new Cube(cubeType.TypeId, orientation, fceData, 13);
						}
					}
                }
            }
            return unfoundNames;
        }

        public World ConvertWorld(String mcDirectory)
        {
            String segmentDirectory = Path.Combine(FCEDirectory, "Segments");
            if (!Directory.Exists(FCEDirectory))
            {
                Directory.CreateDirectory(FCEDirectory);
            }
            if (!Directory.Exists(Path.Combine(FCEDirectory, segmentDirectory)))
            {
                Directory.CreateDirectory(segmentDirectory);
            }
            
			Boolean anvil = true;

            NbtWorld nbtWorld = AnvilWorld.Open(mcDirectory);
            String worldName = nbtWorld.Level.LevelName;
            IChunkManager chunkManager = nbtWorld.GetChunkManager();
            try
            {
                // Try to test for mc world type
                // Don't know how this is supposed to work, but it presumably throws an exception
                // on a non-Anvil world.
                chunkManager.Count();
            }
            catch
			{
				anvil = false;
				nbtWorld = BetaWorld.Open(mcDirectory);
				worldName = nbtWorld.Level.LevelName;
				chunkManager = nbtWorld.GetChunkManager();
			}
            Int32 spawnChunkX = nbtWorld.Level.Spawn.X >> 4;
            Int32 spawnChunkZ = nbtWorld.Level.Spawn.Z >> 4;

            WorldSettings settings = new WorldSettings();
            settings.Name = worldName;
            var fceWorld = World.Create(FCEDirectory, settings);
            var segmentManager = fceWorld.SegmentManager;
            _totalSegments = chunkManager.LongCount() * (anvil ? 16 : 8);
            _segmentsLeft = _totalSegments;
            StartSaveThread(fceWorld);
            foreach (ChunkRef chunk in chunkManager)
            {
                // If the save thread is too slow, wait until it has caught up before adding to it to prevent high ram usage
                while (_saveQueue.Count > 5000)
                {
                    Thread.Sleep(500);
                }

				if (chunk.Blocks == null)
				{
					_segmentsLeft -= (anvil ? 16 : 8);
                    continue;
				}

                Int32 spawnOffsetX = UseSpawnAsOrigin ? spawnChunkX - chunk.X : -chunk.X;
                Int32 spawnOffsetZ = UseSpawnAsOrigin ? spawnChunkZ - chunk.Z : -chunk.Z;
				
                // Minecraft has different x/y directions so we must reverse z so the world isn't mirrored
				var chunkCoords = new SegmentCoords(spawnOffsetX, 0, -spawnOffsetZ) + SegmentCoords.WorldCenter;
                for (Int32 i = 0; i < (anvil ? 16 : 8); i++)
                {
                    SegmentCoords segCoords = chunkCoords + SegmentCoords.Above * i;
                    var segment = new Segment(segmentManager, segCoords);
                    var array = new Cube[16, 16, 16];
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
								if (!_mcIdDataToFCECube.TryGetValue(mcIdData, out cube))
								{
                                    cube = new Cube(1, 0, 0, 0);
									if (!UnknownBlocks.ContainsKey((UInt16)block.ID))
									{
										UnknownBlocks.Add((UInt16)block.ID, block.Info.Name);
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
					var padding = new Segment(segmentManager, chunkCoords + SegmentCoords.Above * i);
					padding.CubeData = Segment.GetBlankSegment().CubeData;
					padding.IsEmpty = true;
					_saveQueue.Enqueue(padding);
				}
			}
            Task.WaitAll(_saveTask);

            return fceWorld;
        }

        private static CubeType GenerateDetailBlock(UInt16 typeId)
        {
            return new CubeType
            {
                 TypeId = typeId,
                Name = "Detail" + typeId,
                Values = new List<ValueEntry>(),
                LayerType = "PrimaryLayer",
                TopTexture = 2,
                SideTexture = 2,
                BottomTexture = 2,
                GuiTexture = 2,
                Stages = new List<Stage>(),
                IsSolid = true,
                IsHollow = true,
                HasObject = true,
                AudioWalkType = "Dirt",
                AudioBuildType = "Dirt",
                AudioDestroyType = "Dirt",
                Tags = new List<String>()
            };
        }

        public void CheckSubDir(World world, SegmentCoords coords)
        {
			var subCoords = new SubdirectoryCoords(coords);
            string subDir = "d-" + subCoords.X.ToString("X") + "-" + subCoords.Y.ToString("X") + "-" + subCoords.Z.ToString("X");
            string path = world.SegmentManager.SegmentPath + subDir;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void StartSaveThread(World world)
		{
            _token = _tokenSource.Token;
            _saveTask = Task.Factory.StartNew(() =>
            {
                while (_segmentsLeft > 0 || _saveQueue.Count > 0)
                {
                    Segment segment;
                    if (_token.IsCancellationRequested)
                        break;
                    if (!_saveQueue.TryDequeue(out segment))
                        continue;
                    CheckSubDir(world, segment.Coords);
                    using (var fs = File.Open(Path.Combine(world.SegmentManager.SegmentPath, segment.GetSegmentFileName()), FileMode.Create))
                    {
                        segment.WriteSegment(fs);
                    }
                }
            }, _tokenSource.Token);
        }
    }
}
