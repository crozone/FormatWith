using System;

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

        public static readonly string TestFormat5 = "abc{Foo.Replacement1}";
        public static readonly string TestFormat5Composite = "abc{0}";
        public static readonly string TestFormat5Solution = $"abc{Replacement1}";

        public static readonly string TestFormat6 = "abc{Replacement1:upper}";
        public static readonly string TestFormat6Composite = "abc{0:upper}";
        public static readonly string TestFormat6Solution = $"abc{Replacement1.ToUpper()}";
        
        public static readonly string TestFormat7 = "Today is {Today:YYYYMMDD HH:mm}";
        public static readonly string TestFormat7Composite = "Today is {0:YYYYMMDD HH:mm}";
        public static readonly DateTime TestFormat7Date = new DateTime(2018, 10, 30, 17, 25, 0);
        public static readonly string TestFormat7Solution = $"Today is {TestFormat7Date:YYYYMMDD HH:mm}";
    }
}
