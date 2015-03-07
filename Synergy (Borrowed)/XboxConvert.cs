using Synergy__Borrowed_.Xbox;
using System;
using System.Collections.Generic;
using System.IO;
namespace Synergy__Borrowed_.FCU
{
    public class XboxConvert
    {
        public XboxWorld XboxWorld;
        public XboxConvert(string fileLoc, bool xboxPackage)
        {
            this.XboxWorld = new XboxWorld(fileLoc, xboxPackage);
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
            World world = new World("ConvertedMCWorld", fcLocation);
            Console.WriteLine("Central node X is " + 4611686017890517424L);
            Console.WriteLine("Central node Y is " + 4611686017890517424L);
            int num = 0;
            List<XboxWorld.NodeCoords> list = this.XboxWorld.ListAllPossibleNodes();
            foreach (XboxWorld.NodeCoords current in list)
            {
                XboxNode node = this.XboxWorld.GetNode(current.X, current.Y);
                int num2 = 32 - node.NodeX;
                int num3 = 32 - node.NodeY;
                long baseX = 4611686017890516944L + (long)(num2 * 16);
                long baseZ = 4611686017890516944L + (long)(num3 * 16);
                for (int i = 0; i < 8; i++)
                {
                    long baseY = 4611686017890516944L + (long)(i * 16);
                    Segment segment = new Segment(world, baseX, baseY, baseZ);
                    Cube[, ,] array = new Cube[16, 16, 16];
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    for (int j = 0; j < 4096; j++)
                    {
                        XboxCube xboxCube = node.CubeData[15 - num4, 15 - num5, 15 - num6];
                        array[num4, num5, num6] = new Cube((ushort)xboxCube.Type, 0, 0, 13);
                        num4++;
                        if (num4 == 16)
                        {
                            num4 = 0;
                            num6++;
                            if (num6 == 16)
                            {
                                num6 = 0;
                                num5++;
                                if (num5 >= 16)
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
                num++;
                Console.WriteLine(string.Concat(new object[]
				{
					"Converted ",
					num,
					" out of ",
					list.Count,
					" chunks"
				}));
                this.XboxWorld.Clear();
            }
            return world;
        }
    }
}
