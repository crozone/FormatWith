using System;
using Xunit;
using FormatWith;
using System.Collections.Generic;
using System.Linq;
using static FormatWithTests.TestStrings;
using System.Text;

namespace FormatWithTests
{
    public class FormatWithTests
    {
        [Fact]
        public void TestEmpty()
        {
            string replacement = TestFormatEmpty.FormatWith(new { Replacement1, Replacement2 });
            Assert.Equal(TestFormatEmpty, replacement);
        }

        [Fact]
        public void TestNoParams()
        {
            string replacement = TestFormatNoParams.FormatWith(new { Replacement1, Replacement2 });
            Assert.Equal(TestFormatNoParams, replacement);
        }

        [Fact]
        public void TestReplacement3()
        {
            string replacement = TestFormat4.FormatWith(new { Replacement1, Replacement2 });
            Assert.Equal(TestFormat4Solution, replacement);
        }

        [Fact]
        public void TestParameterFormat()
        {
            string replacement = TestFormat7.FormatWith(new { Today = TestFormat7Date });
            Assert.Equal(TestFormat7Solution, replacement);
        }

        [Fact]
        public void TestNestedPropertiesReplacements()
        {
            string replacement = TestFormat5.FormatWith(new { Foo = new { Replacement1 } });
            Assert.Equal(TestFormat5Solution, replacement);
        }

        [Fact]
        public void TestUnexpectedOpenBracketError()
        {
            try
            {
                string replacement = "abc{Replacement1}{ {Replacement2}".FormatWith(new { Replacement1, Replacement2 });
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
                string replacement = "abc{Replacement1}{{Replacement2}".FormatWith(new { Replacement1, Replacement2 });
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
                string replacement = "abc{Replacement1}{Replacement2".FormatWith(new { Replacement1, Replacement2 });
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
                string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1, Replacement2 });
            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom<KeyNotFoundException>(ex);
                return;
            }
            Assert.True(false);
        }

        [Fact]
        public void TestDefaultReplacementParameter()
        {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1, Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");
            Assert.Equal("abcReplacement1FallbackValue", replacement);
        }

        [Fact]
        public void TestIgnoreMissingParameter()
        {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1, Replacement2 }, MissingKeyBehaviour.Ignore);
            Assert.Equal("abcReplacement1{DoesntExist}", replacement);
        }

        [Fact]
        public void TestCustomBraces()
        {
            string replacement = "abc<Replacement1><DoesntExist>".FormatWith(new { Replacement1, Replacement2 }, MissingKeyBehaviour.Ignore, null, '<', '>');
            Assert.Equal("abcReplacement1<DoesntExist>", replacement);
        }

        [Fact]
        public void TestAsymmetricCustomBraces()
        {
            string replacement = "abc{Replacement1>{DoesntExist>".FormatWith(new { Replacement1, Replacement2 }, MissingKeyBehaviour.Ignore, null, '{', '>');
            Assert.Equal("abcReplacement1{DoesntExist>", replacement);
        }

        [Fact]
        public void TestCustomHandler1()
        {
            string replacement = "Hey, {make this uppercase!} Thanks.".FormatWith(
                (parameter, format) => new ReplacementResult<string>(true, parameter.ToUpper())
                );

            Assert.Equal("Hey, MAKE THIS UPPERCASE! Thanks.", replacement);
        }

        [Fact]
        public void TestCustomHandler2()
        {
            string replacement = "<abcDEF123:reverse>, <abcDEF123:uppercase>, <abcDEF123:lowercase>.".FormatWith(
                (parameter, format) =>
                {
                    return format switch
                    {
                        "uppercase" => new ReplacementResult<string>(true, parameter.ToUpper()),
                        "lowercase" => new ReplacementResult<string>(true, parameter.ToLower()),
                        "reverse" => new ReplacementResult<string>(true, new string(parameter.Reverse().ToArray())),
                        _ => new ReplacementResult<string>(false, parameter),
                    };
                },
                MissingKeyBehaviour.ReplaceWithFallback,
                "Fallback",
                '<',
                '>'
                );

            Assert.Equal("321FEDcba, ABCDEF123, abcdef123.", replacement);
        }


        [Fact]
        public void TestHugeInput()
        {
            StringBuilder inputBuilder = new StringBuilder();
            StringBuilder expectedBuilder = new StringBuilder();
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>();

            for (int i = 0; i < 100_000; i++) {
                string parameter = "key_" + i;
                string value = "val_" + i;

                replacementDictionary[parameter] = value;

                inputBuilder.Append($"{{{parameter}}} blah \r\n");
                expectedBuilder.Append($"{value} blah \r\n");
            }

            string replacement = inputBuilder.ToString().FormatWith(replacementDictionary);

            Assert.Equal(expectedBuilder.ToString(), replacement);
        }

        [Fact]
        public void TestSpanInputStringBuilder()
        {
            HandlerAction handlerAction = static (key, format, result) =>
            {
                if (key.Equals("Replacement1".AsSpan(), StringComparison.Ordinal))
                {
                    result(Replacement1.AsSpan());
                }
            };

            FallbackAction fallbackAction = static (result) =>
            {
                result("FallbackValue".AsSpan());
            };

            string replacement = "abc{Replacement1}{DoesntExist}".AsSpan().FormatWith(handlerAction, MissingKeyBehaviour.ReplaceWithFallback, fallbackAction);
            Assert.Equal("abcReplacement1FallbackValue", replacement);
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
                TestFormat3.FormatWith(replacementDictionary);
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

            for (int i = 0; i < 1_000_000; i++)
            {
                TestFormat4.FormatWith(replacementDictionary);
            }
        }

        [Fact]
        public void SpeedTestBiggerAnonymous()
        {
            for (int i = 0; i < 1_000_000; i++)
            {
                TestFormat4.FormatWith(new { Replacement1, Replacement2 });
            }
        }

        [Fact]
        public void SpeedTestBiggerMultithreaded()
        {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>()
            {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            Enumerable.Range(1, 1_000_000).AsParallel().ForAll(i => TestFormat4.FormatWith(replacementDictionary));
        }

        [Fact]
        public void SpeedTestHugeMultithreaded()
        {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>()
            {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            Enumerable.Range(1, 10_000_000).AsParallel().ForAll(i => TestFormat4.FormatWith(replacementDictionary));
        }
    }
}
