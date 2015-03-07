using System;
namespace Synergy__Borrowed_.FCU
{
    public struct SegmentCoords
    {
        public long X;
        public long Y;
        public long Z;
        public SegmentCoords(long x, long y, long z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
