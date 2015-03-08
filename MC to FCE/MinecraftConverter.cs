using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System.IO;
using Substrate;
using Substrate.Core;
using Synergy.FCU;
using System.Globalization;
using System.IO.Compression;

namespace MC_to_FCE
{
    public class MinecraftConverter
    {
        IDictionary<UInt16, CubeType> fceCubes;
        Dictionary<UInt32, UInt32> mcIdDataToFCEIdData = new Dictionary<UInt32, UInt32>();
        Dictionary<UInt16, String> unknownBlocks = new Dictionary<UInt16, String>();

        public Boolean UnknownsMapToDetail { get; set; }
        private String _fceDirectory;
        
        public MinecraftConverter(String fceDirectory)
        {
            _fceDirectory = fceDirectory;
            fceCubes = CubeType.Cubes;
        }

        public void LoadNameMap(String filePath)
        {
            Dictionary<String, String> mcNameToFCEName = new Dictionary<String, String>();
            var DocumentMaps = new XmlDocument();
            DocumentMaps.Load(filePath);
            XmlNodeList elementsByTagName = DocumentMaps.GetElementsByTagName("Blocks");

            var mcBlockTable = Substrate.BlockInfo.BlockTable;
            Dictionary<String, UInt16> mcNameToId = new Dictionary<String, UInt16>();
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
                    if (!mcNameToId.ContainsKey(mcCleanName))
                        continue;

                    UInt32 mcId = mcNameToId[mcCleanName];
                    UInt32 mcIdShifted = mcId << 16;

                    foreach(XmlNode mcValue in mcName.ChildNodes)
                    {
                        List<SByte> mcData = new List<SByte>();
                        String fceName = "";
                        UInt16 fceData = 0;
                        foreach (XmlNode node in mcValue.ChildNodes)
                        {
                            if (node.Name == "Value")
                                mcData.Add(SByte.Parse(node.InnerText));
                            else if (node.Name == "FCEName")
                                fceName = node.InnerText;
                            else if (node.Name == "FCEData")
                                fceData = UInt16.Parse(node.InnerText, NumberStyles.HexNumber);
                        }
                        CubeType cube = fceCubes.FirstOrDefault(c => c.Value.Name == fceName).Value;
                        if (cube == null)
                             continue;
                        UInt32 fceIdData = (UInt32)cube.TypeId << 16 | fceData;
                        foreach (SByte mcDatum in mcData)
                        {
                            UInt32 mcIdData = mcIdShifted;
                            if (mcDatum > 0)
                                mcIdData |= (Byte)mcDatum;
                            else
                                mcIdData |= 0x8000; // This bit flags if we don't care what the data is.
                            mcIdDataToFCEIdData[mcIdData] = fceIdData;
                        }
                    }
                }
            }
        }

        public World ConvertWorld(String mcDirectory)
        {
            String segmentDirectory = Path.Combine(_fceDirectory, "Segments");
            String worldName;
            Boolean anvil = true;
            if (!Directory.Exists(_fceDirectory))
            {
                Directory.CreateDirectory(_fceDirectory);
            }
            if (!Directory.Exists(Path.Combine(_fceDirectory, segmentDirectory)))
            {
                Directory.CreateDirectory(segmentDirectory);
            }

            IChunkManager chunkManager;

            Int32 spawnChunkX;
            Int32 spawnChunkZ;

            while(true)
            {
                NbtWorld nbtWorld;
                if(anvil)
                    nbtWorld = AnvilWorld.Open(mcDirectory);
                else
                    nbtWorld = BetaWorld.Open(mcDirectory);
                worldName = nbtWorld.Level.LevelName;
                chunkManager = nbtWorld.GetChunkManager();

                spawnChunkX = nbtWorld.Level.Spawn.X >> 4;
                spawnChunkZ = nbtWorld.Level.Spawn.Z >> 4;
                try
                {
                    // Try something to test for mc world type
                    // Maybe this works? Don't know how the original code is supposed to work,
                    // and it won't compile
                    chunkManager.GetEnumerator();

                    break;
                }
                catch
                {
                    anvil = false;
                    continue;
                }
            }

            World fceWorld = new World(worldName, _fceDirectory);
            
            foreach(ChunkRef chunk in chunkManager)
            {
                Int32 spawnOffsetX = spawnChunkX - chunk.X;
                Int32 spawnOffsetZ = spawnChunkZ - chunk.Z;

                Int64 baseX = 4611686017890516944L + (Int64)(spawnOffsetX * 16);
                // Minecraft has different x/y directions so we must reverse z so the world isn't mirrored
                Int64 baseZ = 4611686017890516944L - (Int64)(spawnOffsetZ * 16);
                for (Byte i = 0; i < (anvil ? 16 : 8); i++)
                {
                    Int64 baseY = 4611686017890516944L + (Int64)(i * 16) + 48;
                    Segment segment = new Segment(fceWorld, baseX, baseY, baseZ);
                    Cube[, ,] array = new Cube[16, 16, 16];
                    for (Byte x = 0; x < 16; x++)
                    {
                        for(Byte y = 0; y < 16; y++)
                        {
                            for (Byte z = 0; z < 16; z++)
                            {
                                // Minecraft has different x/y directions so we must reverse z so the world isn't mirrored
                                AlphaBlock block = chunk.Blocks.GetBlock(15 - z, y + i * 16, x);
                                UInt32 mcIdData = (UInt32)block.ID << 16 | (UInt16)block.Data;

                                UInt32 fceIdData;
                                if (!mcIdDataToFCEIdData.TryGetValue(mcIdData, out fceIdData))
                                {
                                    if (!mcIdDataToFCEIdData.TryGetValue((mcIdData | 0x8000) & 0xFFFF8000, out fceIdData))
                                    {
                                        fceIdData = 1 << 16;
                                        if (!unknownBlocks.ContainsKey((UInt16)block.ID))
                                        {
                                            unknownBlocks.Add((UInt16)block.ID, block.Info.Name);
                                        }
                                    }
                                }
                                UInt16 fceType = (UInt16)(fceIdData >> 16);
                                UInt16 fceData = (UInt16)(fceIdData);
                                array[z, y, x] = new Cube(fceType, 0, fceData, 13);
                            }
                        }
                    }

                    segment.SetMappedCubeData(array);

                    fixInnerFlags(fceWorld, segment);

                    checkSubDir(fceWorld, segment.GetCoords());
                    FileStream fs = File.Open(Path.Combine(segmentDirectory, segment.GetSegmentFileName()), FileMode.Create);
                    segment.WriteSegment(fs);
                    fs.Dispose();
                }
            }

            return fceWorld;
        }

        public void checkSubDir(World world, SegmentCoords coords)
        {
            long num = coords.X >> 8;
            long num2 = coords.Y >> 8;
            long num3 = coords.Z >> 8;
            string subDir = "d-" + num.ToString("X") + "-" + num2.ToString("X") + "-" + num3.ToString("X");
            string path = world.SegmentPath + subDir;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void fixInnerFlags(World world, Segment segment)
        {
            Cube[, ,] cubeMap = segment.GetMappedCubeData();
            Cube north, south, east, west, above, below;
            Boolean empty = true;
            for (Byte i = 0; i < 16; i++)
            {
                for (Byte j = 0; j < 16; j++)
                {
                    for (Byte k = 0; k < 16; k++)
                    {
                        Cube cube = cubeMap[i, j, k];
                        if (!cube.IsAir())
                        {
                            Byte flags = 0;
                            empty = false;

                            if (k < 15)
                            {
                                north = cubeMap[i, j, k + 1];
                                if (CubeType.Cubes[north.Type].IsOpen)
                                    flags += 0x08;
                            }
                            if (k > 0)
                            {
                                south = cubeMap[i, j, k - 1];
                                if (CubeType.Cubes[south.Type].IsOpen)
                                    flags += 0x04;
                            }
                            if (i < 15)
                            {
                                east = cubeMap[i + 1, j, k];
                                if (CubeType.Cubes[east.Type].IsOpen)
                                    flags += 0x10;
                            }
                            if (i > 0)
                            {
                                west = cubeMap[i - 1, j, k];
                                if (CubeType.Cubes[west.Type].IsOpen)
                                    flags += 0x20;
                            }
                            if (j < 15)
                            {
                                above = cubeMap[i, j + 1, k];
                                if (CubeType.Cubes[above.Type].IsOpen)
                                    flags += 0x01;
                            }
                            if (j > 0)
                            {
                                below = cubeMap[i, j - 1, k];
                                if (CubeType.Cubes[below.Type].IsOpen)
                                    flags += 0x02;
                            }

                            cube.Flags = flags;
                        }
                        else
                            cube.Flags = 0;
                    }
                }
            }

            segment.IsEmpty = empty;
            segment.HasFaces = !empty;
            segment.maCubeData = cubeMap;
            cubeMap = null;
        }
    }
}
