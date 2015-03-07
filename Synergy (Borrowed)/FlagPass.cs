using System;
namespace Synergy__Borrowed_.FCU
{
    public class FlagPass
    {
        public Segment Segment;
        public Cube[, ,] SegmentMapData;
        public World World;
        public FlagPass(Segment segment, World world)
        {
            this.Segment = segment;
            this.World = world;
        }
        public void RunPass()
        {
            this.SegmentMapData = this.Segment.GetMappedCubeData();
            this.World.Segments.Clear();
            bool flag = true;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        Cube cube = this.SegmentMapData[i, j, k];
                        byte b = 0;
                        if (!cube.IsAir())
                        {
                            flag = false;
                            Cube block = this.World.GetBlock(this.Segment.X + (long)i, this.Segment.Y + (long)j, this.Segment.Z + (long)k - 1L);
                            Cube block2 = this.World.GetBlock(this.Segment.X + (long)i, this.Segment.Y + (long)j, this.Segment.Z + (long)k + 1L);
                            Cube block3 = this.World.GetBlock(this.Segment.X + (long)i + 1L, this.Segment.Y + (long)j, this.Segment.Z + (long)k);
                            Cube block4 = this.World.GetBlock(this.Segment.X + (long)i - 1L, this.Segment.Y + (long)j, this.Segment.Z + (long)k);
                            Cube block5 = this.World.GetBlock(this.Segment.X + (long)i, this.Segment.Y + (long)j + 1L, this.Segment.Z + (long)k);
                            Cube block6 = this.World.GetBlock(this.Segment.X + (long)i, this.Segment.Y + (long)j - 1L, this.Segment.Z + (long)k);
                            if (block5.IsAir())
                            {
                                b += 1;
                            }
                            if (block6.IsAir())
                            {
                                b += 2;
                            }
                            if (block.IsAir())
                            {
                                b += 4;
                            }
                            if (block2.IsAir())
                            {
                                b += 8;
                            }
                            if (block3.IsAir())
                            {
                                b += 16;
                            }
                            if (block4.IsAir())
                            {
                                b += 32;
                            }
                            cube.Flags = b;
                        }
                        else
                        {
                            cube.Flags = 0;
                        }
                    }
                }
            }
            this.Segment.IsEmpty = flag;
            if (flag)
            {
                MinecraftConvert.EmptySegments++;
            }
            else
            {
                MinecraftConvert.NonEmptySegments++;
            }
            this.Segment.HasFaces = !flag;
            this.Segment.maCubeData = this.SegmentMapData;
            this.SegmentMapData = null;
        }
    }
}
