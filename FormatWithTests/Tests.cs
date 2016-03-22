using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FormatWith;

namespace FormatWithTests {
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Tests {
        public const string TestStringEmtpy = "";

        public const string TestStringNoParams = "Test string with no parameters";

        public const string testString3 = "abc{param1}def{{escaped1}}ghi{{{param2}}}jkl{{{{escaped2}}}}mno";

        [Fact]
        public void TrueTest() {
            // test to make sure testing framework is working (!)
            Assert.True(true);
        }

        [Fact]
        public void TestReplacementEmpty() {
            string replacement = TestStringEmtpy.FormatWith(new { param1 = "REPLACE1", param2 = "REPLACE2" });
            Assert.True(replacement == TestStringEmtpy);
        }

        [Fact]
        public void TestReplacementNoParams() {
            string replacement = TestStringNoParams.FormatWith(new { param1 = "REPLACE1", param2 = "REPLACE2" });
            Assert.True(replacement == TestStringNoParams);
        }

        [Fact]
        public void TestReplacement3() {
            string replacement = testString3.FormatWith(new { param1 = "REPLACE1", param2 = "" });
            Assert.True(replacement == "abcREPLACE1def{escaped1}ghi{}jkl{{escaped2}}mno");
        }
    }
}
