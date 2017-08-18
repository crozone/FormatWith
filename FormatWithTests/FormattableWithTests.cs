using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FormatWith;
using static FormatWithTests.TestStrings;

namespace FormatWithTests
{
    public class FormattableWithTests
    {
        [Fact]
        public void TestEmpty()
        {
            FormattableString formattableString = TestFormatEmpty.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(TestFormatEmpty, formattableString.Format);
            Assert.Equal(0, formattableString.ArgumentCount);
            Assert.Equal(TestFormatEmpty, formattableString.ToString());
        }

        [Fact]
        public void TestNoParams()
        {
            FormattableString formattableString = TestFormatNoParams.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(TestFormatNoParams, formattableString.Format);
            Assert.Equal(0, formattableString.ArgumentCount);
            Assert.Equal(TestFormatNoParams, formattableString.ToString());
        }

        [Fact]
        public void TestReplacement3()
        {
            FormattableString formattableString = TestFormat3.FormattableWith(new { Replacement1 = Replacement1});
            Assert.Equal(TestFormat3Composite, formattableString.Format);
            Assert.Equal(1, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(TestFormat3Solution, formattableString.ToString());
        }

        [Fact]
        public void TestReplacement4()
        {
            FormattableString formattableString = TestFormat4.FormattableWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.Equal(TestFormat4Composite, formattableString.Format);
            Assert.Equal(2, formattableString.ArgumentCount);
            Assert.Equal(Replacement1, formattableString.GetArgument(0));
            Assert.Equal(Replacement2, formattableString.GetArgument(1));
            Assert.Equal(TestFormat4Solution, formattableString.ToString());
        }
    }
}
