using System;
using System.Collections.Generic;
using Xunit;
using FormatWith;
using FormatWithTests.FormatProvider;

using static FormatWithTests.Shared.TestStrings;

namespace FormatWithTests
{
    public class FormattableWithTests
    {
        [Fact]
        public void TestEmpty()
        {
            FormattableString formattableString = FormatEmpty.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(FormatEmpty, formattableString.Format);
            Assert.Equal(0, formattableString.ArgumentCount);
            Assert.Equal(FormatEmpty, formattableString.ToString());
        }

        [Fact]
        public void TestNoParams()
        {
            FormattableString formattableString = FormatNoParams.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(FormatNoParams, formattableString.Format);
            Assert.Equal(0, formattableString.ArgumentCount);
            Assert.Equal(FormatNoParams, formattableString.ToString());
        }

        [Fact]
        public void TestReplacement3()
        {
            FormattableString formattableString = Format3.FormattableWith(new { Replacement1 = Replacement1 });
            Assert.Equal(Format3Composite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(Format3Solution, formattableString.ToString());
        }

        [Fact]
        public void TestReplacement4()
        {
            FormattableString formattableString = Format4.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(Format4Composite, formattableString.Format);
            Assert.Equal(2, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(Replacement2, formattableString.GetArgument(1));
            Assert.Equal(Format4Solution, formattableString.ToString());
        }

        [Fact]
        public void TestNestedProperties()
        {
            FormattableString formattableString = Format5.FormattableWith(new { Foo = new { Replacement1 = Replacement1 } });
            Assert.Equal(Format5Composite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(Format5Solution, formattableString.ToString());
        }

        [Fact]
        public void TestFormatString()
        {
            FormattableString formattableString = Format6.FormattableWith(new { Replacement1 = Replacement1 });
            Assert.Equal(Format6Composite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));

            var upperCaseFormatProvider = new UpperCaseFormatProvider();
            
            Assert.Equal(Format6Solution, formattableString.ToString(upperCaseFormatProvider));
        }

        [Fact]
        public void TestCustomBraces()
        {
            string format = "abc{{Replacement1}<Replacement2>";
            string formatComposite = "abc{{{{Replacement1}}{0}";
            string formatSolution = $"abc{{{{Replacement1}}{Replacement2}";
            FormattableString formattableString = format.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ThrowException, null, '<', '>');
            Assert.Equal(formatComposite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement2, formattableString.GetArgument(0));
            Assert.Equal(formatSolution, formattableString.ToString());
        }

        [Fact]
        public void TestAsymmetricCustomBracesWithIgnore()
        {
            string format = "abc{Replacement1>{DoesntExist>";
            string formatComposite = "abc{0}{{DoesntExist>";
            string formatSolution = $"abc{Replacement1}{{DoesntExist>";

            FormattableString formattableString = format.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null, '{', '>');
            Assert.Equal(formatComposite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(formatSolution, formattableString.ToString());
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
                FormattableString formattableString = Format3.FormattableWith(replacementDictionary);
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
                FormattableString formattableString = Format4.FormattableWith(replacementDictionary);
            }
        }

        [Fact]
        public void SpeedTestBiggerAnonymous()
        {
            for (int i = 0; i < 1000000; i++)
            {
                FormattableString formattableString = Format4.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
        }
    }
}
