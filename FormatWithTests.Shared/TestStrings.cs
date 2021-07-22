using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWithTests.Shared
{
    public static class TestStrings
    {
        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";

        public static readonly Dictionary<string, string> ReplacementDictionary = new Dictionary<string, string>()
        {
            ["Replacement1"] = Replacement1,
            ["Replacement2"] = Replacement2
        };

        public static readonly object ReplacementObject = new { Replacement1, Replacement2 };

        public static readonly string FormatEmpty = "";

        public static readonly string FormatNoParams = "Test string with no parameters";

        public static readonly string Format3 = "test{Replacement1}";
        public static readonly string Format3Composite = "test{0}";
        public static readonly string Format3Solution = $"test{Replacement1}";

        public static readonly string Format4 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";
        public static readonly string Format4Composite = "abc{0}def{{escaped1}}ghi{{{1}}}jkl{{{{escaped2}}}}mno";
        public static readonly string Format4Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        public static readonly string Format5 = "abc{Foo.Replacement1}";
        public static readonly string Format5Composite = "abc{0}";
        public static readonly string Format5Solution = $"abc{Replacement1}";

        public static readonly string Format6 = "abc{Replacement1:upper}";
        public static readonly string Format6Composite = "abc{0:upper}";
        public static readonly string Format6Solution = $"abc{Replacement1.ToUpper()}";

        public static readonly string FormatWithPadding = "abc{   Replacement1      }";
        public static readonly string FormatWithPaddingComposite = "abc{   0      }";
        public static readonly string FormatWithPaddingSolution = $"abc{   Replacement1      }";

        public static readonly string FormatDateWithFormat = "Today is {Today:YYYYMMDD HH:mm}";
        public static readonly string FormatDateWithFormatComposite = "Today is {0:YYYYMMDD HH:mm}";
        public static readonly DateTime FormatDateWithFormatDateValue = new DateTime(2018, 10, 30, 17, 25, 0);
        public static readonly string FormatDateWithFormatSolution = $"Today is {FormatDateWithFormatDateValue:YYYYMMDD HH:mm}";

        public static readonly string FormatHugeStringManyParametersInput;
        public static readonly string FormatHugeStringManyParametersResult;
        public static readonly Dictionary<string, string> FormatHugeStringManyParametersReplacementDictionary;

        public static readonly string FormatBigStringMostlyTextInput;
        public static readonly string FormatBigStringMostlyTextResult;

        static TestStrings()
        {
            (FormatHugeStringManyParametersInput, FormatHugeStringManyParametersResult, FormatHugeStringManyParametersReplacementDictionary) = GetHugeStringManyParameters();
            (FormatBigStringMostlyTextInput, FormatBigStringMostlyTextResult) = GetBigStringMostlyText();
        }

        private static (string, string, Dictionary<string, string>) GetHugeStringManyParameters()
        {
            StringBuilder inputBuilder = new StringBuilder();
            StringBuilder resultBuilder = new StringBuilder();
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>();

            for (int i = 0; i < 100_000; i++)
            {
                string parameter = "key_" + i;
                string value = "val_" + i;

                replacementDictionary[parameter] = value;

                inputBuilder.Append($"{{{parameter}}} blah \r\n");
                resultBuilder.Append($"{value} blah \r\n");
            }

            return (inputBuilder.ToString(), resultBuilder.ToString(), replacementDictionary);
        }

        private static (string, string) GetBigStringMostlyText()
        {
            StringBuilder inputBuilder = new StringBuilder();
            StringBuilder resultBuilder = new StringBuilder();

            inputBuilder.Append("{Replacement1}");
            resultBuilder.Append(Replacement1);

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            for (int i = 0; i < 1000; i++)
            {
                string word = words[i % words.Length];
                inputBuilder.Append(word);
                resultBuilder.Append(word);
            }

            inputBuilder.Append("{Replacement2}");
            resultBuilder.Append(Replacement2);

            for (int i = 0; i < 1000; i++)
            {
                string word = words[i % words.Length];
                inputBuilder.Append(word);
                resultBuilder.Append(word);
            }

            return (inputBuilder.ToString(), resultBuilder.ToString());
        }
    }
}
