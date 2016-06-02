using System;
using System.Collections.Generic;
using System.Linq;
using FormatWith;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormatWithNetCoreUnitTests {
    [TestClass]
    public class TestClass {
        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";

        public static readonly string TestFormatEmpty = "";

        public static readonly string TestFormatNoParams = "Test string with no parameters";

        public static readonly string testFormat3 = "test{Replacement1}";
        public static readonly string testFormat3Solution = $"testReplacement1";

        public static readonly string testFormat4 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        public static readonly string testFormat4Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        [TestMethod]
        public void TestMethodPassing() {
            // test to make sure testing framework is working (!)
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestReplacementEmpty() {
            string replacement = TestFormatEmpty.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.IsTrue(replacement == TestFormatEmpty);
        }

        [TestMethod]
        public void TestReplacementNoParams() {
            string replacement = TestFormatNoParams.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.IsTrue(replacement == TestFormatNoParams);
        }

        [TestMethod]
        public void TestReplacement3() {
            string replacement = testFormat4.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            Assert.IsTrue(replacement == testFormat4Solution);
        }

        [TestMethod]
        public void TestUnexpectedOpenBracketError() {
            try {
                string replacement = "abc{Replacement1}{ {Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "Unexpected opening brace inside a parameter at position 19") {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestUnexpectedCloseBracketError() {
            try {
                string replacement = "abc{Replacement1}{{Replacement2}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "Unexpected closing brace at position 31") {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestUnexpectedEndOfFormatString() {
            try {
                string replacement = "abc{Replacement1}{Replacement2".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (FormatException e) {
                if (e.Message == "The format string ended before the parameter was closed. Position 30") {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestKeyNotFoundError() {
            try {
                string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
            }
            catch (KeyNotFoundException e) {
                if (e.Message == "The parameter \"DoesntExist\" was not present in the lookup dictionary") {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestDefaultReplacementParameter() {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");

            if (replacement == "abcReplacement1FallbackValue") {
                Assert.IsTrue(true);
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestIgnoreMissingParameter() {
            string replacement = "abc{Replacement1}{DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

            if (replacement == "abcReplacement1{DoesntExist}") {
                Assert.IsTrue(true);
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestCustomBraces() {
            string replacement = "abc<Replacement1><DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null, '<', '>');

            if (replacement == "abcReplacement1<DoesntExist>") {
                Assert.IsTrue(true);
                return;
            }

            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestGetFormatParameters() {
            List<string> parameters = testFormat4.GetFormatParameters().ToList();
            if (parameters.Count != 2) Assert.IsTrue(false);
            if (parameters[0] != nameof(Replacement1)) {
                Assert.IsTrue(false);
                return;
            }
            if (parameters[1] != nameof(Replacement2)) {
                Assert.IsTrue(false);
                return;
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SpeedTest() {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>() {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            for (int i = 0; i < 1000000; i++) {
                string replacement = testFormat3.FormatWith(replacementDictionary);
                if (replacement != testFormat3Solution) {
                    Assert.IsTrue(false);
                }
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SpeedTestBigger() {
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>() {
                ["Replacement1"] = Replacement1,
                ["Replacement2"] = Replacement2
            };

            for (int i = 0; i < 1000000; i++) {
                string replacement = testFormat4.FormatWith(replacementDictionary);
                if (replacement != testFormat4Solution) {
                    Assert.IsTrue(false);
                }
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SpeedTestBiggerAnonymous() {
            for (int i = 0; i < 1000000; i++) {
                string replacement = testFormat4.FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });
                if (replacement != testFormat4Solution) {
                    Assert.IsTrue(false);
                }
            }
            Assert.IsTrue(true);
        }
    }
}
