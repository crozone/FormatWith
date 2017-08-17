using System;
using Xunit;
using FormatWith;
using System.Collections.Generic;
using System.Linq;

namespace FormatWithTests
{
    public class UnitTest
    {
        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";

        public static readonly string TestFormatEmpty = "";

        public static readonly string TestFormatNoParams = "Test string with no parameters";

        public static readonly string testFormat3 = "test{Replacement1}";
        public static readonly string testFormat3Solution = $"testReplacement1";

        public static readonly string testFormat4 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        public static readonly string testFormat4Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        [Fact]
        public void TestMethodPassing()
        {
            // test to make sure testing framework is working (!)
            Assert.True(true);
        }

        [Fact]
        public void TestReplacementEmpty()
        {
            string replacement = TestFormatEmpty.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(TestFormatEmpty, replacement);
        }

        [Fact]
        public void TestReplacementNoParams()
        {
            string replacement = TestFormatNoParams.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(TestFormatNoParams, replacement);
        }

        [Fact]
        public void TestReplacement3()
        {
            string replacement = testFormat4.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(testFormat4Solution, replacement);
        }

        [Fact]
        public void TestUnexpectedOpenBracketError()
        {
            try
            {
                string replacement = "abc{Replacement1}{ {Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e)
            {
                Assert.Equal("Unexpected opening brace inside a parameter at position 19", e.Message);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void TestUnexpectedCloseBracketError()
        {
            try
            {
                string replacement = "abc{Replacement1}{{Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e)
            {
                Assert.Equal("Unexpected closing brace at position 31", e.Message);
                return;
            }
            Assert.True(false);
        }

        [Fact]
        public void TestUnexpectedEndOfFormatString()
        {
            try
            {
                string replacement = "abc{Replacement1}{Replacement2".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e)
            {
                Assert.Equal("The format string ended before the parameter was closed. Position 30", e.Message);
                return;
            }
            Assert.True(false);
        }

        [Fact]
        public void TestKeyNotFoundError()
        {
            try
            {
                string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (KeyNotFoundException e)
            {
                Assert.Equal("The parameter \"DoesntExist\" was not present in the lookup dictionary", e.Message);
                return;
            }
            Assert.True(false);
        }

        [Fact]
        public void TestDefaultReplacementParameter()
        {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");
            Assert.Equal("abcReplacement1FallbackValue", replacement);
        }

        [Fact]
        public void TestIgnoreMissingParameter()
        {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);
            Assert.Equal("abcReplacement1{DoesntExist}", replacement);
        }

        [Fact]
        public void TestCustomBraces()
        {
            string replacement = "abc<Replacement1><DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null, '<', '>');
            Assert.Equal("abcReplacement1<DoesntExist>", replacement);
        }

        [Fact]
        public void TestGetFormatParameters()
        {
            List<string> parameters = testFormat4.GetFormatParameters().ToList();
            Assert.Equal(parameters.Count, 2);
            Assert.Equal(nameof(Replacement1), parameters[0]);
            Assert.Equal(nameof(Replacement2), parameters[1]);
        }

        [Fact]
        public void SpeedTest()
        {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>()
            {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            for (int i = 0; i < 1000000; i++)
            {
                string replacement = testFormat3.FormatWith(replacementDictionary);
            }
        }

        [Fact]
        public void SpeedTestBigger()
        {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>()
            {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            for (int i = 0; i < 1000000; i++)
            {
                string replacement = testFormat4.FormatWith(replacementDictionary);
            }
        }

        [Fact]
        public void SpeedTestBiggerAnonymous()
        {
            for (int i = 0; i < 1000000; i++)
            {
                string replacement = testFormat4.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
        }
    }
}
