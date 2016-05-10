using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FormatWith;
using System.Diagnostics;
using Xunit.Abstractions;

namespace FormatWithTests {
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Tests {

        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";

        public static readonly string TestFormatEmpty = "";

        public static readonly string TestFormatNoParams = "Test string with no parameters";

        public static readonly string testFormat3 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        public static readonly string testFormat3Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        [Fact]
        public void TrueTest() {
            // test to make sure testing framework is working (!)
            Assert.True(true);
        }

        [Fact]
        public void TestReplacementEmpty() {
            string replacement = TestFormatEmpty.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.True(replacement == TestFormatEmpty);
        }

        [Fact]
        public void TestReplacementNoParams() {
            string replacement = TestFormatNoParams.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.True(replacement == TestFormatNoParams);
        }

        [Fact]
        public void TestReplacement3() {
            string replacement = testFormat3.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.True(replacement == testFormat3Solution);
        }

        [Fact]
        public void TestUnexpectedOpenBracketError() {
            try {
                string replacement = "abc{Replacement1}{ {Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "Unexpected opening brace inside a parameter at position 19") {
                    Assert.True(true);
                    return;
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void TestUnexpectedCloseBracketError() {
            try {
                string replacement = "abc{Replacement1}{{Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "Unexpected closing brace at position 31") {
                    Assert.True(true);
                    return;
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void TestUnexpectedEndOfFormatString() {
            try {
                string replacement = "abc{Replacement1}{Replacement2".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "The format string ended before the parameter was closed. Position 30") {
                    Assert.True(true);
                    return;
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void TestKeyNotFoundError() {
            try {
                string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (KeyNotFoundException e) {
                if (e.Message == "The parameter \"DoesntExist\" was not present in the lookup dictionary") {
                    Assert.True(true);
                    return;
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void TestDefaultReplacementParameter() {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");

            if (replacement == "abcReplacement1FallbackValue") {
                Assert.True(true);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void TestIgnoreMissingParameter() {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

            if (replacement == "abcReplacement1{DoesntExist}") {
                Assert.True(true);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void TestCustomBraces() {
            string replacement = "abc<Replacement1><DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null, '<', '>');

            if (replacement == "abcReplacement1<DoesntExist>") {
                Assert.True(true);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void TestGetFormatParameters() {
            List<string> parameters = testFormat3.GetFormatParameters();
            if (parameters.Count != 2) Assert.True(false);
            if (parameters[0] != nameof(Replacement1)) {
                Assert.True(false);
                return;
            }
            if (parameters[1] != nameof(Replacement2)) {
                Assert.True(false);
                return;
            }
            Assert.True(true);
        }

        

        [Fact]
        public void SpeedTest() {
            for (int i = 0; i < 1000000; i++) {
                string replacement = testFormat3.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
                if (replacement != testFormat3Solution) {
                    Assert.True(false);
                }
            }
            Assert.True(true);
        }
    }
}
