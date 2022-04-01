using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minst_MonoGame
{
    public static class Extensions
    {
        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static void ForEach<T>(this T[,] source, Action<int, int> action)
        {
            for (int w = 0; w < source.GetLength(0); w++)
            {
                for (int h = 0; h < source.GetLength(1); h++)
                {
                    action(w, h);
                }
            }
        }

        public class Image
        {
            public byte Label { get; set; }
            public float[] LabelArray 
            {

                get 
                {
                    var arr = new float[10];
                    arr[(int)Label] = 1;
                    return arr;
                }
                
                
            
            }
            public byte[,] Data { get; set; }
            public byte[] DataFlat { get; set; }
            public float[] DataFlat_NetInputs { get; set; }
        }

        public enum Location
        {
            TopLeft,
            TopRight,
            TopMiddle,
            BottomMiddle,
            BottomLeft,
            BottomRight,
            Middle
        }

    }
}
