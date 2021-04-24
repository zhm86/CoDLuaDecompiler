namespace CoDHVKDecompiler.Common.Extensions
{
    public static class ByteExtensions
    {
        public static byte GetBit(this byte bit, long input)
        {
            return (byte) ((input >> bit) & 1);
        }
    }
}