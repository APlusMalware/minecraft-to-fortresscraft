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
                                north = north ?? world.GetSegment(segment.X, segment.Y, segment.Z + 16);
                                
                                if (north.maCubeData[i, j, 0].Type.IsOpen)
                                    flags += 0x08;
                            }
                            else if (k == 0)
                            {
                                south = south ?? world.GetSegment(segment.X, segment.Y, segment.Z - 16);
                                if (south.maCubeData[i, j, 15].Type.IsOpen)
                                    flags += 0x04;
                            }
                            if (i == 15)
                            {
                                east = east ?? world.GetSegment(segment.X + 16, segment.Y, segment.Z);
                                if (east.maCubeData[0, j, k].Type.IsOpen)
                                    flags += 0x10;
                            }
                            else if (i == 0)
                            {
                                west = west ?? world.GetSegment(segment.X - 16, segment.Y, segment.Z);
                                if (west.maCubeData[15, j, k].Type.IsOpen)
                                    flags += 0x20;
                            }
                            if (j == 15)
                            {
                                above = above ?? world.GetSegment(segment.X, segment.Y + 16, segment.Z);
                                if (above.maCubeData[i, 0, k].Type.IsOpen)
                                    flags += 0x01;
                            }
                            else if (j == 0)
                            {
                                below = below ?? world.GetSegment(segment.X, segment.Y - 16, segment.Z);
                                if (below.maCubeData[i, 15, k].Type.IsOpen)
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
