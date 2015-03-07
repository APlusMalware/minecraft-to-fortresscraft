using System;
namespace Synergy__Borrowed_.FCU
{
    public class Cube
    {
        public byte Flags;
        public ushort Type;
        public ushort Extra;
        public ushort Lighting;
        public void SetFlags(byte flag)
        {
            this.Flags = flag;
        }
        public Cube Clone()
        {
            return new Cube(this.Type, this.Flags, this.Extra, this.Lighting);
        }
        public new ushort GetType()
        {
            return this.Type;
        }
        public byte GetFlags()
        {
            return this.Flags;
        }
        public bool IsAir()
        {
            return this.Type == 1 || this.Type == 0;
        }
        public Cube(ushort type, byte flags, ushort extraData, ushort lighting)
        {
            this.Type = type;
            this.Flags = flags;
            this.Extra = extraData;
            this.Lighting = lighting;
        }
        public static bool operator ==(Cube x, Cube y)
        {
            return x.Flags == y.Flags && x.Extra == y.Extra && x.Type == y.Type && x.Lighting == y.Lighting;
        }
        public static bool operator !=(Cube x, Cube y)
        {
            return !(x == y);
        }
    }
}
