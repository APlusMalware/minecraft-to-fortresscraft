using System;
using System.IO;
namespace Synergy__Borrowed_.FCU
{
    public class Segment
    {
        public const int SegmentX = 16;
        public const int SegmentY = 16;
        public const int SegmentZ = 16;
        public const int SegmentX1 = 15;
        public const int SegmentY1 = 15;
        public const int SegmentZ1 = 15;
        public const int SegmentYShift = 8;
        public const int SegmentZShift = 4;
        public const int SegmentVolume = 4096;
        public long X;
        public long Y;
        public long Z;
        public long FileX;
        public long FileY;
        public long FileZ;
        public World World;
        private int _blockCount;
        public bool HasFaces;
        public bool IsEmpty;
        public bool IsBlank = false;
        public Cube[, ,] maCubeData;
        public string FullFileName = "";
        private static Segment _blankSegment;
        public static Segment BlankSegment
        {
            get
            {
                if (Segment._blankSegment == null)
                {
                    Segment._blankSegment = new Segment(null, -1L, -1L, -1L);
                    Cube[, ,] array = new Cube[16, 16, 16];
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            for (int k = 0; k < 16; k++)
                            {
                                array[i, j, k] = new Cube(1, 0, 0, 0);
                            }
                        }
                    }
                    Segment._blankSegment.SetMappedCubeData(array);
                    Segment._blankSegment.IsBlank = true;
                    Segment._blankSegment.IsEmpty = true;
                }
                return Segment._blankSegment;
            }
        }
        public Segment(World world, long baseX, long baseY, long baseZ)
        {
            this.X = baseX;
            this.Y = baseY;
            this.Z = baseZ;
            this.FileX = baseX;
            this.FileY = baseY;
            this.FileZ = baseZ;
            this.World = world;
            this.maCubeData = new Cube[16, 16, 16];
        }
        public void LoadSegment(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            string a = binaryReader.ReadString();
            if (a != "FCU")
            {
                throw new Exception(string.Concat(new object[]
				{
					"Segment (",
					this.X,
					", ",
					this.Y,
					", ",
					this.Z,
					") file ID is not of FCU"
				}));
            }
            this._blockCount = binaryReader.ReadInt32();
            this.FileX = binaryReader.ReadInt64();
            this.FileY = binaryReader.ReadInt64();
            this.FileZ = binaryReader.ReadInt64();
            if (Segment.GetSegmentFileName(this.World.Location, new SegmentCoords(this.X, this.Y, this.Z)) != Segment.GetSegmentFileName(this.World.Location, new SegmentCoords(this.FileX, this.FileY, this.FileZ)))
            {
                throw new Exception(string.Concat(new object[]
				{
					"Segment (",
					this.X,
					", ",
					this.Y,
					", ",
					this.Z,
					") does not contain the correct coordinates"
				}));
            }
            long position = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = position;
            for (int i = 0; i < this._blockCount; i++)
            {
                int num = binaryReader.ReadInt32();
                int num2 = binaryReader.ReadInt32();
                if (num == 0)
                {
                    this.LoadCubeDataBlock(binaryReader, num2);
                }
                else
                {
                    if (num == 2)
                    {
                        this.LoadLightingBlock(binaryReader, num2);
                    }
                    else
                    {
                        if (num == 3)
                        {
                            this.LoadBiomeBlock(binaryReader, num2);
                        }
                        else
                        {
                            if (num == 1)
                            {
                                if (num2 != 1)
                                {
                                    throw new Exception("Version mismatch!");
                                }
                                this.LoadEntityBlock(binaryReader, false, num2);
                            }
                        }
                    }
                }
            }
            binaryReader.Close();
        }
        private void LoadCubeDataBlock(BinaryReader reader, int version)
        {
            this.HasFaces = reader.ReadBoolean();
            int num = reader.ReadInt32();
            if (num == 0)
            {
                this.IsEmpty = true;
            }
            else
            {
                long position = reader.BaseStream.Position;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                reader.BaseStream.Position = position;
                while (reader.BaseStream.Position < position + (long)num)
                {
                    if (num2 == 4096)
                    {
                        break;
                    }
                    ushort num6 = reader.ReadUInt16();
                    byte b = reader.ReadByte();
                    int num7 = 1;
                    ushort extraData;
                    ushort lighting;
                    if (num6 == 65535)
                    {
                        num7 = (int)(b + 2);
                        num6 = reader.ReadUInt16();
                        b = reader.ReadByte();
                        extraData = reader.ReadUInt16();
                        lighting = reader.ReadUInt16();
                    }
                    else
                    {
                        extraData = reader.ReadUInt16();
                        lighting = reader.ReadUInt16();
                    }
                    if (num6 == 65535)
                    {
                        throw new Exception("Corrupted file - found cube type MAX");
                    }
                    for (int i = 0; i < num7; i++)
                    {
                        this.maCubeData[num3, num5, num4] = new Cube(num6, b, extraData, lighting);
                        num3++;
                        if (num3 == 16)
                        {
                            num3 = 0;
                            num4++;
                            if (num4 == 16)
                            {
                                num4 = 0;
                                num5++;
                                if (num5 >= 16)
                                {
                                }
                            }
                        }
                    }
                    num2 += num7;
                }
            }
        }
        public SegmentCoords GetCoords()
        {
            return new SegmentCoords(this.X, this.Y, this.Z);
        }
        public static string GetSegmentSubDir(string worldLocation, SegmentCoords coords)
        {
            long num = coords.X >> 8;
            long num2 = coords.Y >> 8;
            long num3 = coords.Z >> 8;
            string text = string.Format("d-{0:X}-{1:X}-{2:X}", num, num2, num3);
            string path = Path.Combine(worldLocation, "Segments", text);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return text;
        }
        public static string GetTranslatedCoords(SegmentCoords coords)
        {
            long num = coords.X / 16L;
            long num2 = coords.Y / 16L;
            long num3 = coords.Z / 16L;
            return string.Format("s-{0:X}-{1:X}-{2:X}.dat", num, num2, num3);
        }
        public static string GetSegmentFileName(string worldLocation, SegmentCoords coords)
        {
            long num = coords.X / 16L;
            long num2 = coords.Y / 16L;
            long num3 = coords.Z / 16L;
            return Path.Combine(Segment.GetSegmentSubDir(worldLocation, coords), string.Format("s-{0:X}-{1:X}-{2:X}.dat", num, num2, num3));
        }
        public string GetSegmentFileName()
        {
            return Segment.GetSegmentFileName(this.World.Location, new SegmentCoords(this.X, this.Y, this.Z));
        }
        public void WriteSegment(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write("FCU");
            binaryWriter.Write(3);
            binaryWriter.Write(this.FileX);
            binaryWriter.Write(this.FileY);
            binaryWriter.Write(this.FileZ);
            this.WriteCubeDataBlock(binaryWriter);
            this.WriteLightingBlock(binaryWriter);
            this.WriteEntityBlock(binaryWriter);
            binaryWriter.Close();
        }
        private void WriteEntityBlock(BinaryWriter writer)
        {
            writer.Write(1);
            writer.Write(1);
            writer.Write(0);
        }
        private void WriteLightingBlock(BinaryWriter writer)
        {
            writer.Write(2);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(true);
            writer.Write(true);
            writer.Write(true);
        }
        private void LoadLightingBlock(BinaryReader reader, int version)
        {
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadBoolean();
            reader.ReadBoolean();
            reader.ReadBoolean();
        }
        private void LoadBiomeBlock(BinaryReader reader, int version)
        {
        }
        private void LoadEntityBlock(BinaryReader reader, bool fromNetwork, int version)
        {
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                int num2 = reader.ReadInt32();
                int num3 = reader.ReadInt32();
                if (fromNetwork)
                {
                    int num4 = reader.ReadInt32();
                }
                long num5 = reader.ReadInt64();
                long num6 = reader.ReadInt64();
                long num7 = reader.ReadInt64();
            }
            if (num > 0)
            {
            }
        }
        public void DoFlagPass(World world)
        {
            FlagPass flagPass = new FlagPass(this, world);
            flagPass.RunPass();
        }
        public void SetMappedCubeData(Cube[, ,] cubeData)
        {
            this.maCubeData = cubeData;
        }
        public Cube[, ,] GetMappedCubeData()
        {
            Cube[, ,] mappedCubeData;
            if (this.IsEmpty && !this.IsBlank)
            {
                mappedCubeData = Segment.BlankSegment.GetMappedCubeData();
            }
            else
            {
                mappedCubeData = this.maCubeData;
            }
            return mappedCubeData;
        }
        private void WriteCubeDataBlock(BinaryWriter writer)
        {
            writer.Write(0);
            writer.Write(0);
            writer.Write(this.HasFaces);
            if (this.IsEmpty)
            {
                writer.Write(0);
            }
            else
            {
                long position = writer.BaseStream.Position;
                writer.Write(1);
                int length = this.maCubeData.Length;
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                Cube cube = this.maCubeData[num2, num4, num3];
                int num5 = 1;
                int i = 1;
                while (i < length)
                {
                    num2++;
                    if (num2 == 16)
                    {
                        num2 = 0;
                        num3++;
                        if (num3 == 16)
                        {
                            num3 = 0;
                            num4++;
                            if (num4 >= 16)
                            {
                                throw new Exception("Writing too many cubes");
                            }
                        }
                    }
                    Cube cube2 = this.maCubeData[num2, num4, num3];
                    if (cube2 == cube && num5 < 257)
                    {
                        num5++;
                        i++;
                    }
                    else
                    {
                        if (num5 > 1)
                        {
                            writer.Write(65535);
                            writer.Write((byte)(num5 - 2));
                            num += 3;
                        }
                        writer.Write(cube.Type);
                        writer.Write(cube.Flags);
                        writer.Write(cube.Extra);
                        writer.Write(cube.Lighting);
                        num += 7;
                        if (cube.Type == 65535)
                        {
                            throw new Exception("Cannot save cube type MAX");
                        }
                        num5 = 1;
                        cube = cube2;
                        i++;
                    }
                }
                if (num5 > 1)
                {
                    writer.Write(65535);
                    writer.Write((byte)(num5 - 2));
                    num += 3;
                }
                writer.Write(cube.Type);
                writer.Write(cube.Flags);
                writer.Write(cube.Extra);
                writer.Write(cube.Lighting);
                num += 7;
                long position2 = writer.BaseStream.Position;
                writer.BaseStream.Seek(position, SeekOrigin.Begin);
                writer.Write(num);
                writer.BaseStream.Seek(position2, SeekOrigin.Begin);
                if (cube.Type == 65535)
                {
                    throw new Exception("cannot save cube type MAX");
                }
                if (i != length)
                {
                    throw new Exception("compression failed. BUG!");
                }
                if (position2 - (position + 4L) != (long)num)
                {
                    throw new Exception("memwriting failed. BUG!");
                }
            }
        }
    }
}
