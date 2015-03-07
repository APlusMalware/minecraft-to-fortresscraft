using Synergy.Xbox;
using System;
using System.IO;
namespace Synergy__Borrowed_.Compress
{
    public class RLEncoding
    {
        public static byte RLE_BYTE = 255;
        public static byte[] DecompressData(byte[] compressedData)
        {
            MemoryStream memoryStream = new MemoryStream();
            int num = -1;
            for (int i = 0; i < compressedData.Length; i += 2)
            {
                byte b = compressedData[i];
                byte b2 = compressedData[i + 1];
                if (b == RLEncoding.RLE_BYTE)
                {
                    num = (int)(b2 + 2);
                }
                else
                {
                    if (num != -1)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            memoryStream.WriteByte(b);
                            memoryStream.WriteByte(b2);
                        }
                        num = -1;
                    }
                    else
                    {
                        memoryStream.WriteByte(b);
                        memoryStream.WriteByte(b2);
                    }
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Dispose();
            return result;
        }
        public static byte[] CompressData(XboxCube[, ,] CubeData)
        {
            MemoryStream memoryStream = new MemoryStream();
            int num = 32768;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            byte b = CubeData[num2, num4, num3].Type;
            byte b2 = CubeData[num2, num4, num3].Flags;
            int num5 = 1;
            int i = 0;
            while (i < num - 1)
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
                        if (num4 >= 128)
                        {
                            throw new Exception("RLE Compress: Too many cubes");
                        }
                    }
                }
                byte b3;
                byte b4;
                if (CubeData[num2, num3, num4] != null)
                {
                    b3 = CubeData[num2, num3, num4].GetType();
                    b4 = CubeData[num2, num3, num4].GetFlags();
                }
                else
                {
                    b3 = 1;
                    b4 = 0;
                }
                if (b == b3 && b2 == b4 && num5 < 257)
                {
                    num5++;
                    i++;
                }
                else
                {
                    if (num5 > 1)
                    {
                        memoryStream.WriteByte(255);
                        memoryStream.WriteByte((byte)(num5 - 2));
                    }
                    memoryStream.WriteByte(b);
                    memoryStream.WriteByte(b2);
                    if (b == 255)
                    {
                        Console.WriteLine("Cant convert 255");
                    }
                    num5 = 1;
                    b = b3;
                    b2 = b4;
                    i++;
                }
            }
            if (num5 > 1)
            {
                memoryStream.WriteByte(255);
                memoryStream.WriteByte((byte)(num5 - 2));
            }
            memoryStream.WriteByte(b);
            memoryStream.WriteByte(b2);
            i++;
            byte[] result = memoryStream.ToArray();
            memoryStream.Dispose();
            return result;
        }
    }
}
