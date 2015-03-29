using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Synergy.FCU;
using System.Collections.Concurrent;

namespace MC_to_FCE
{
    public class FlagPass
    {
        private World _world;
        private Dictionary<SegmentCoords, Segment> _cache;
        private static ConcurrentBag<String> _retry;

        public FlagPass(World world)
        {
            _world = world;
        }
        public static List<String> FixCubeFlags(World world)
        {
            String[] directories = Directory.GetDirectories(world.SegmentPath);
            List<String> permaFailed = new List<String>();
            _retry = new ConcurrentBag<String>();

            Parallel.ForEach(directories, subDirectory =>
            {
                FlagPass pass = new FlagPass(world);
                pass.Run(subDirectory);
            });
            
            // Retry failed segments one last time sequentially and return any failed segments
            foreach (String fileName in _retry)
            {
                SegmentCoords coords = new SegmentCoords(Path.GetFileNameWithoutExtension(fileName));
                FlagPass pass = new FlagPass(world);
                try
                {
                    Segment segment = world.GetSegment(coords);
                    segment.FullFileName = fileName;
                    pass.fixOuterFlags(world, segment);

                    segment.WriteSegment(File.Open(fileName, FileMode.OpenOrCreate));
                }
                catch (IOException ex)
                {
                    permaFailed.Add(fileName);
                }
            }
            _retry = null;
            return permaFailed;
        }

        public void Run(String subDirectory)
        {
            String[] files = Directory.GetFiles(subDirectory);
            List<String> retry = new List<String>();
            foreach (String fileName in files)
            {
                SegmentCoords coords = new SegmentCoords(Path.GetFileNameWithoutExtension(fileName));
                try
                {
                    Segment segment = _world.GetSegment(coords);
                    segment.FullFileName = fileName;
                    fixOuterFlags(_world, segment);

                    segment.WriteSegment(File.Open(fileName, FileMode.OpenOrCreate));
                }
                catch (IOException ex)
                {
                    // On IOException add segment to retry list for this directory
                    retry.Add(fileName);
                }
            }

            // Retry failed passes in this directory
            foreach (String fileName in retry)
            {
                SegmentCoords coords = new SegmentCoords(Path.GetFileNameWithoutExtension(fileName));
                try
                {
                    Segment segment = _world.GetSegment(coords);
                    segment.FullFileName = fileName;
                    fixOuterFlags(_world, segment);

                    segment.WriteSegment(File.Open(fileName, FileMode.OpenOrCreate));
                }
                catch (IOException ex)
                {
                    // On IOException add segment to static retry list
                    _retry.Add(fileName);
                }
            }
        }

        private void fixOuterFlags(World world, Segment segment)
        {
            Cube[,,] cubeMap = segment.CubeData;

            Segment north = null;
            Segment south = null;
            Segment east = null;
            Segment west = null;
            Segment above = null;
            Segment below = null;

			Boolean northChecked = false;
			Boolean southChecked = false;
			Boolean eastChecked = false;
			Boolean westChecked = false;
			Boolean aboveChecked = false;
			Boolean belowChecked = false;
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
								if (!northChecked)
									north = north ?? world.GetSegment(segment.X, segment.Y, segment.Z + 16);
								northChecked = true;

								if (north != null && north.CubeData[i, j, 0].Type.IsOpen)
                                    flags += 0x08;
                            }
                            else if (k == 0)
                            {
								if (!southChecked)
									south = south ?? world.GetSegment(segment.X, segment.Y, segment.Z - 16);
								southChecked = true;

								if (south != null && south.CubeData[i, j, 15].Type.IsOpen)
                                    flags += 0x04;
                            }
                            if (i == 15)
                            {
								if (!eastChecked)
									east = east ?? world.GetSegment(segment.X + 16, segment.Y, segment.Z);
								eastChecked = true;

								if (east != null && east.CubeData[0, j, k].Type.IsOpen)
                                    flags += 0x10;
                            }
                            else if (i == 0)
                            {
								if (!westChecked)
									west = west ?? world.GetSegment(segment.X - 16, segment.Y, segment.Z);
								westChecked = true;

								if (west != null && west.CubeData[15, j, k].Type.IsOpen)
                                    flags += 0x20;
                            }
                            if (j == 15)
                            {
								if (!aboveChecked)
									above = above ?? world.GetSegment(segment.X, segment.Y + 16, segment.Z);
								aboveChecked = true;

								if (above != null && above.CubeData[i, 0, k].Type.IsOpen)
                                    flags += 0x01;
                            }
                            else if (j == 0)
                            {
								if (!belowChecked)
									below = below ?? world.GetSegment(segment.X, segment.Y - 16, segment.Z);
								belowChecked = true;

								if (below != null && below.CubeData[i, 15, k].Type.IsOpen)
                                    flags += 0x02;
                            }

                            cube.Flags = flags;
                        }
                        else
                            cube.Flags = 0;
                    }
                }
            }

            segment.CubeData = cubeMap;
            cubeMap = null;
        }
    }
}
