using Substrate;
using Substrate.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
namespace Synergy.FCU
{
    public class MinecraftConvert
    {
        public Dictionary<string, int> RockMap = new Dictionary<string, int>();
        public Dictionary<string, string> RockConvertMap = new Dictionary<string, string>();
        public string[] MCConvertMap;
        public Dictionary<string, long> MCUnknown = new Dictionary<string, long>();
        public string WorldLocation;
        public XmlDocument DocumentMaps;
        public static int EmptySegments = 0;
        public static int NonEmptySegments = 0;
        public int done;
        public int max;
        public static int TotalSegments
        {
            get
            {
                return MinecraftConvert.EmptySegments + MinecraftConvert.NonEmptySegments;
            }
        }
        public MinecraftConvert(string worldLocation)
        {
            this.WorldLocation = worldLocation;
            this.LoadRockData("cubedata.xml");
        }
        public void LoadRockData(string file)
        {
            this.DocumentMaps = new XmlDocument();
            this.DocumentMaps.Load(file);
            XmlNodeList elementsByTagName = this.DocumentMaps.GetElementsByTagName("Cubes");
            foreach (XmlNode xmlNode in elementsByTagName)
            {
                foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                {
                    if (xmlNode2.Name == "CubeData")
                    {
                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            this.RockMap.Add(xmlNode3.Name, int.Parse(xmlNode3.InnerText));
                        }
                    }
                    else
                    {
                        if (xmlNode2.Name == "ConvertData")
                        {
                            foreach (XmlNode xmlNode4 in xmlNode2.ChildNodes)
                            {
                                this.RockConvertMap.Add(xmlNode4.Name, xmlNode4.InnerText);
                            }
                        }
                    }
                }
            }
            FieldInfo[] fields = typeof(BlockType).GetFields();
            this.MCConvertMap = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                this.MCConvertMap[(int)fieldInfo.GetValue(null)] = fieldInfo.Name;
            }
        }
        public World ConvertWorld(string fcLocation)
        {
            if (!Directory.Exists(fcLocation))
            {
                Directory.CreateDirectory(fcLocation);
            }
            if (!Directory.Exists(Path.Combine(fcLocation, "Segments")))
            {
                Directory.CreateDirectory(Path.Combine(fcLocation, "Segments"));
            }
            string a = "ANVIL";
            World world;
            IChunkManager chunkManager;
            int num;
            int num2;
            while (true)
            {
                NbtWorld nbtWorld = null;
                if (a == "ANVIL")
                {
                    nbtWorld = AnvilWorld.Open(this.WorldLocation);
                }
                else
                {
                    if (a == "BETA")
                    {
                        nbtWorld = BetaWorld.Open(this.WorldLocation);
                    }
                }
                world = new World(nbtWorld.Level.LevelName, fcLocation);
                chunkManager = nbtWorld.GetChunkManager();
                num = nbtWorld.Level.Spawn.X >> 4;
                num2 = nbtWorld.Level.Spawn.Z >> 4;
                try
                {
                    this.max = chunkManager.Count<ChunkRef>();
                }
                catch
                {
                    a = "BETA";
                    continue;
                }
                break;
            }
            this.done = 0;
            foreach (ChunkRef current in chunkManager)
            {
                int num3 = num - current.X;
                int num4 = num2 - current.Z;
                long baseX = 4611686017890516944L + (long)(num3 * 16);
                long baseZ = 4611686017890516944L + (long)(num4 * 16);
                for (int i = 0; i < ((a == "ANVIL") ? 16 : 8); i++)
                {
                    long baseY = 4611686017890516944L + (long)(i * 16);
                    Segment segment = new Segment(world, baseX, baseY, baseZ);
                    Cube[, ,] array = new Cube[16, 16, 16];
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    for (int j = 0; j < 4096; j++)
                    {
                        AlphaBlock block = current.Blocks.GetBlock(15 - num5, num6 + i * 16, 15 - num7);
                        array[num5, num6, num7] = new Cube(this.ConvertMC2FCType(block), 0, 0, 13);
                        num5++;
                        if (num5 == 16)
                        {
                            num5 = 0;
                            num7++;
                            if (num7 == 16)
                            {
                                num7 = 0;
                                num6++;
                                if (num6 >= 16)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    segment.SetMappedCubeData(array);
                    string path = Path.Combine(fcLocation, "Segments", segment.GetSegmentFileName());
                    segment.WriteSegment(File.Open(path, FileMode.OpenOrCreate));
                }
                this.done++;
            }
            return world;
        }
        public ushort ConvertMC2FCType(AlphaBlock alphaBlock)
        {
            int iD = alphaBlock.ID;
            ushort result;
            if (alphaBlock.Data != 0 && this.RockConvertMap.ContainsKey(this.MCConvertMap[iD] + "-" + alphaBlock.Data))
            {
                result = (ushort)this.RockMap[this.RockConvertMap[this.MCConvertMap[iD] + "-" + alphaBlock.Data]];
            }
            else
            {
                if (this.RockConvertMap.ContainsKey(this.MCConvertMap[iD]))
                {
                    result = (ushort)this.RockMap[this.RockConvertMap[this.MCConvertMap[iD]]];
                }
                else
                {
                    if (this.RockConvertMap.ContainsKey(string.Concat(iD)))
                    {
                        result = (ushort)this.RockMap[this.RockConvertMap[string.Concat(iD)]];
                    }
                    else
                    {
                        if (!this.MCUnknown.ContainsKey(this.MCConvertMap[iD]))
                        {
                            this.MCUnknown[this.MCConvertMap[iD]] = 0L;
                        }
                        Dictionary<string, long> mCUnknown;
                        string key;
                        (mCUnknown = this.MCUnknown)[key = this.MCConvertMap[iD]] = mCUnknown[key] + 1L;
                        result = (ushort)this.RockMap[this.RockConvertMap["UNKNOWN"]];
                    }
                }
            }
            return result;
        }
    }
}
