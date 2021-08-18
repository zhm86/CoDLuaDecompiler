using System;
using System.Text;

namespace CoDLuaDecompiler.Decompiler.Extensions
{
    public static class StringBuilderExtension
    {
        public static String AddIndent(this String str, bool skipFront = false)
        {
            if (!skipFront)
                str = str.Insert(0, "\t");
            return str.Replace("\n", "\n\t");
        }
    }
}