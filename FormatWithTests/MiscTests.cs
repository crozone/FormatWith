using System.Collections.Generic;
using System.Linq;

using Xunit;
using FormatWith;

using static FormatWithTests.Shared.TestStrings;

namespace FormatWithTests
{
    public class MiscTests
    {
        [Fact]
        public void TestMethodPassing()
        {
            // test to make sure testing framework is working (!)
            Assert.True(true);
        }

        [Fact]
        public void TestGetFormatParameters()
        {
            List<string> parameters = Format4.GetFormatParameters().ToList();
            Assert.Equal(2, parameters.Count);
            Assert.Equal(nameof(Replacement1), parameters[0]);
            Assert.Equal(nameof(Replacement2), parameters[1]);
        }
    }
}
