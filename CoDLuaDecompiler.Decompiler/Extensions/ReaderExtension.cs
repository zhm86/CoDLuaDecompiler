using System;
using System.IO;

namespace CoDLuaDecompiler.Decompiler.Extensions
{
    public static class ReaderExtension
    {
        private const long SIGN_EXTEND_MASK = -1L;
        private const int INT64_BITSIZE = sizeof(long) * 8;

        public static long ReadLEB128(this BinaryReader stream)
        {
            return ReadLEB128(stream, out _);
        }

        public static long ReadLEB128(this BinaryReader stream, out int bytes)
        {
            bytes = 0;

            long value = 0;
            var shift = 0;
            bool more = true, signBitSet = false;

            while (more)
            {
                var next = stream.ReadByte();
                if (next < 0) throw new InvalidOperationException("Unexpected end of stream");

                var b = next;
                bytes += 1;

                more = (b & 0x80) != 0; // extract msb
                signBitSet = (b & 0x40) != 0; // sign bit is the msb of a 7-bit byte, so 0x40

                var chunk = b & 0x7fL; // extract lower 7 bits
                value |= chunk << shift;
                shift += 7;
            }

            ;

            // extend the sign of shorter negative numbers
            if (shift < INT64_BITSIZE && signBitSet) value |= SIGN_EXTEND_MASK << shift;

            return value;
        }

        public static ulong ReadULEB128(this BinaryReader stream)
        {
            return ReadULEB128(stream, out _);
        }

        public static ulong ReadULEB128(this BinaryReader stream, out int bytes)
        {
            bytes = 0;

            ulong value = 0;
            var shift = 0;
            var more = true;

            while (more)
            {
                var next = stream.ReadByte();
                if (next < 0) throw new InvalidOperationException("Unexpected end of stream");

                var b = next;
                bytes += 1;

                more = (b & 0x80) != 0; // extract msb
                var chunk = b & 0x7fUL; // extract lower 7 bits
                value |= chunk << shift;
                shift += 7;
            }

            return value;
        }

        public static Tuple<bool, ulong> ReadULEB128_from33bit(this BinaryReader stream)
        {
            var first_byte = stream.ReadByte();

            var isNumberBit = Convert.ToBoolean(first_byte & 0x1);
            ulong value = (ulong) (first_byte >> 1);

            if (value >= 0x40)
            {
                var bitshift = -1;
                value &= 0x3f;

                while (true)
                {
                    var bte = stream.ReadByte();
                    bitshift += 7;
                    value |= (ulong)(bte & 0x7f) << bitshift;

                    if (bte < 0x80)
                        break;
                }
            }

            return new Tuple<bool, ulong>(isNumberBit, (ulong) value);
        }
    }
}