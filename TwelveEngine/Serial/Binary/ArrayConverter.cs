using System;

namespace TwelveEngine.Serial.Binary {
    internal static class ArrayConverter {
        internal static byte[] ToBytes(int[] array) {
            var byteArray = new byte[array.Length * sizeof(int)];
            Buffer.BlockCopy(array,0,byteArray,0,byteArray.Length);
            return byteArray;
        }
        internal static int[] ToIntArray(byte[] bytes) {
            var intArray = new int[bytes.Length / sizeof(int)];
            Buffer.BlockCopy(bytes,0,intArray,0,bytes.Length);
            return intArray;
        }
        internal static byte[] ToBytes(int[,] array2D) {
            var itemCount = array2D.GetLength(0) * array2D.GetLength(1);
            var byteArray = new byte[itemCount * sizeof(int)];
            Buffer.BlockCopy(array2D,0,byteArray,0,byteArray.Length);
            return byteArray;
        }
        internal static int[,] ToIntArray2D(byte[] bytes,int width) {
            var itemCount = bytes.Length / sizeof(int);
            var rows = itemCount / width;
            var array2D = new int[width,rows];
            Buffer.BlockCopy(bytes,0,array2D,0,bytes.Length);
            return array2D;
        }
    }
}
