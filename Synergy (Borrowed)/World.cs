using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
namespace Synergy__Borrowed_.FCU
{
    public class World
    {
        public static int returnedBlank = 0;
        public string Name;
        public string Location;
        public Dictionary<SegmentCoords, Segment> Segments;
        public int FoldersDone;
        public int FoldersLeft;
        public World(string name, string location)
        {
            this.Name = name;
            this.Location = location;
            this.Segments = new Dictionary<SegmentCoords, Segment>();
        }
        public Stream GetStream(SegmentCoords coords)
        {
            string path = Path.Combine(this.Location, "Segments", Segment.GetSegmentFileName(this.Location, coords));
            Stream result;
            if (!File.Exists(path))
            {
                result = null;
            }
            else
            {
                result = File.Open(path, FileMode.OpenOrCreate);
            }
            return result;
        }
        public Segment GetSegment(SegmentCoords coords)
        {
            Segment result;
            if (this.Segments.ContainsKey(coords))
            {
                result = this.Segments[coords];
            }
            else
            {
                Segment segment = new Segment(this, coords.X, coords.Y, coords.Z);
                segment.FullFileName = Path.Combine(this.Location, "Segments", Segment.GetSegmentFileName(this.Location, coords));
                Stream stream = this.GetStream(coords);
                if (stream == null)
                {
                    World.returnedBlank++;
                    result = Segment.BlankSegment;
                }
                else
                {
                    segment.LoadSegment(stream);
                    if (this.Segments.Count == 50)
                    {
                        this.Segments.Remove(this.Segments.Keys.ElementAt(0));
                    }
                    this.Segments.Add(coords, segment);
                    result = segment;
                }
            }
            return result;
        }
        public Cube GetBlock(long x, long y, long z)
        {
            long num = x % 16L;
            long num2 = y % 16L;
            long num3 = z % 16L;
            long x2 = x - num;
            long y2 = y - num2;
            long z2 = z - num3;
            Segment segment = this.GetSegment(new SegmentCoords(x2, y2, z2));
            return checked(segment.GetMappedCubeData()[(int)((IntPtr)num), (int)((IntPtr)num2), (int)((IntPtr)num3)]);
        }
        public SegmentCoords GetSegCoordsFromFileName(string fileName)
        {
            if (fileName.EndsWith(".dat"))
            {
                fileName = fileName.Substring(0, fileName.IndexOf("."));
            }
            string[] array = fileName.Split(new char[]
			{
				'-'
			});
            long x = long.Parse(array[1], NumberStyles.AllowHexSpecifier) * 16L;
            long y = long.Parse(array[2], NumberStyles.AllowHexSpecifier) * 16L;
            long z = long.Parse(array[3], NumberStyles.AllowHexSpecifier) * 16L;
            return new SegmentCoords(x, y, z);
        }
        public void ReloadAndSave()
        {
            string[] directories = Directory.GetDirectories(Path.Combine(this.Location, "Segments"));
            int num = 0;
            string[] array = directories;
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                string[] files = Directory.GetFiles(path);
                string[] array2 = files;
                for (int j = 0; j < array2.Length; j++)
                {
                    string text = array2[j];
                    Segment segment = this.GetSegment(this.GetSegCoordsFromFileName(Path.GetFileNameWithoutExtension(text)));
                    segment.FullFileName = text;
                    segment.WriteSegment(File.Open(text, FileMode.OpenOrCreate));
                }
                num++;
                Console.WriteLine(string.Concat(new object[]
				{
					"Reloaded folder (",
					num,
					"/",
					directories.Length,
					")/"
				}));
            }
        }
        public void RunFlagPass(bool status = false)
        {
            string[] directories = Directory.GetDirectories(Path.Combine(this.Location, "Segments"));
            int num = 0;
            string[] array = directories;
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                string[] files = Directory.GetFiles(path);
                string[] array2 = files;
                for (int j = 0; j < array2.Length; j++)
                {
                    string text = array2[j];
                    Segment segment = this.GetSegment(this.GetSegCoordsFromFileName(Path.GetFileNameWithoutExtension(text)));
                    segment.FullFileName = text;
                    segment.DoFlagPass(this);
                    segment.WriteSegment(File.Open(text, FileMode.OpenOrCreate));
                }
                num++;
                if (status)
                {
                    Console.WriteLine(string.Concat(new object[]
					{
						"Flag passing segment folder (",
						num,
						"/",
						directories.Length,
						")/"
					}));
                    this.FoldersDone = num;
                    this.FoldersLeft = directories.Length;
                }
            }
        }
    }
}
