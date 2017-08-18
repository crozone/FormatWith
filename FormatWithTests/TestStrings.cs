using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWithTests
{
    public static class TestStrings
    {
        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";

        public static readonly string TestFormatEmpty = "";

        public static readonly string TestFormatNoParams = "Test string with no parameters";

        public static readonly string TestFormat3 = "test{Replacement1}";
        public static readonly string TestFormat3Composite = "test{0}";
        public static readonly string TestFormat3Solution = $"test{Replacement1}";

        public static readonly string TestFormat4 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";
        public static readonly string TestFormat4Composite = "abc{0}def{{escaped1}}ghi{{{1}}}jkl{{{{escaped2}}}}mno";
        public static readonly string TestFormat4Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";
    }
}
