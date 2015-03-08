using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Synergy.FCU;

namespace MC_to_FCE
{
    public class FlagPass
    {
        private World _world;
        private Dictionary<SegmentCoords, Segment> _cache;

        public FlagPass(World world)
        {
            _world = world;
        }
        public static void FixCubeFlags(World world)
        {
            String[] directories = Directory.GetDirectories(world.SegmentPath);
            Parallel.ForEach(directories, subDirectory =>
            {
                FlagPass pass = new FlagPass(world);
                pass.Run(subDirectory);
            });
        }

        public void Run(String subDirectory)
        {
            String[] files = Directory.GetFiles(subDirectory);
            foreach (String fileName in files)
            {
                Segment segment = _world.GetSegment(new SegmentCoords(Path.GetFileNameWithoutExtension(fileName)));
                segment.FullFileName = fileName;
                fixOuterFlags(_world, segment);

                segment.WriteSegment(File.Open(fileName, FileMode.OpenOrCreate));
            }
        }

        private void fixOuterFlags(World world, Segment segment)
        {
            Cube[,,] cubeMap = segment.GetMappedCubeData();

            Segment north = null;
            Segment south = null;
            Segment east = null;
            Segment west = null;
            Segment above = null;
            Segment below = null;

            for (Byte i = 0; i < 16; i++)
            {
                for (Byte j = 0; j < 16; j++)
                {
                    for (Byte k = 0; k < 16; k++)
                    {
                        Cube cube = cubeMap[i, j, k];
                        if (!cube.IsAir())
                        {
                            Byte flags = cube.Flags;

                            if (k == 15)
                            {
                                north = north ?? world.GetSegment(segment.X, segment.Y, segment.Z + 1);
                                
                                if (CubeType.Cubes[north.maCubeData[i, j, 0].Type].IsOpen)
                                    flags += 0x08;
                            }
                            else if (k == 0)
                            {
                                south = south ?? world.GetSegment(segment.X, segment.Y, segment.Z - 1);
                                if (CubeType.Cubes[south.maCubeData[i, j, 15].Type].IsOpen)
                                    flags += 0x04;
                            }
                            if (i == 15)
                            {
                                east = east ?? world.GetSegment(segment.X + 1, segment.Y, segment.Z);
                                if (CubeType.Cubes[(east.maCubeData[0, j, k].Type)].IsOpen)
                                    flags += 0x10;
                            }
                            else if (i == 0)
                            {
                                west = west ?? world.GetSegment(segment.X - 1, segment.Y, segment.Z);
                                if (CubeType.Cubes[(west.maCubeData[15, j, k].Type)].IsOpen)
                                    flags += 0x20;
                            }
                            if (j == 15)
                            {
                                above = above ?? world.GetSegment(segment.X, segment.Y + 1, segment.Z);
                                if (CubeType.Cubes[(above.maCubeData[i, 0, k].Type)].IsOpen)
                                    flags += 0x01;
                            }
                            else if (j == 0)
                            {
                                below = below ?? world.GetSegment(segment.X, segment.Y - 1, segment.Z);
                                if (CubeType.Cubes[(below.maCubeData[i, 15, k].Type)].IsOpen)
                                    flags += 0x02;
                            }

                            cube.Flags = flags;
                        }
                        else
                            cube.Flags = 0;
                    }
                }
            }

            segment.maCubeData = cubeMap;
            cubeMap = null;
        }
    }
}
