using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Egomotion.EgoXproject.Internal;

namespace Egomotion.EgoXprojectTests.Utils
{
    [TestFixture]
    public class StringUtilsTest
    {
        [Test]
        public void ArrayToStringTest()
        {
            string[] test = new string[] { "a", "b", "c"};
            string expected1 = "a, b, c";
            string expected2 = "a,b,c";
            string expected3 = "a b c";
            string expected4 = "afoobfooc";
            Assert.AreEqual(expected1, StringUtils.ArrayToString(test));
            Assert.AreEqual(expected2, StringUtils.ArrayToString(test, ","));
            Assert.AreEqual(expected3, StringUtils.ArrayToString(test, " "));
            Assert.AreEqual(expected4, StringUtils.ArrayToString(test, "foo"));
        }

        [Test]
        public void EmptyArrayToStringTest()
        {
            Assert.AreEqual("", StringUtils.ArrayToString(null));
            Assert.AreEqual("", StringUtils.ArrayToString(new string[] {}));
        }

        [Test]
        public void ContainsInvalidCharactersTest()
        {
            string valid = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM/_ŐőŒœŔŕŖŗŘřŚśŜŝŞş.";
            Assert.False(valid.ContainsInvalidCharacters());
            string[] invalid = new string[] {"\'", " ", "\"", "\0", "\a", "\b", "\f", "\n", "\r", "\t", "\v", "[", "]",
                                             "{", "}", ",", ":", ";", "~", "-", "+", "=", "!\", \"@\", \"#\", \"$\", \"%\", \"^\", \"&\", \"\", \"*\", \"(\", \")"
                                            };

            foreach (var entry in invalid)
            {
                Assert.True(entry.ContainsInvalidCharacters());
            }
        }

        [Test]
        public void EncloseInQuotesTest()
        {
            string test = "my test string";
            string expected = "\"my test string\"";
            var result = test.EncloseInQuotes();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EmptyEncloseInQuotesTest()
        {
            string test = "";
            string expected = "\"\"";
            var result = test.EncloseInQuotes();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AlreadyEnclosedInQuotesTest()
        {
            string test = "\"my test string\"";
            string expected = "\"my test string\"";
            var result = test.EncloseInQuotes();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void QuoteStringWithQuotesTest()
        {
            string s = "te\"st\"";
            string res = s.EncloseInQuotes();
            Assert.AreEqual(res, "\"te\"st\"\"");
        }

        [Test]
        public void ToLiteralIfRequiredTest()
        {
            string[] test = new string[]
            {
                "Path/With Spaces/foo",
                "$(ARCHS_STANDARD_INCLUDING_64_BIT)",
                "gnu++0x",
                "libc++",
                "iPhone Developer",
                "DEBUG=1",
                "$(inherited)",
                "1,2",
                "TestApp/TestApp-Prefix.pch",
                "TestApp/TestApp-Info.plist",
                "$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp",
                "$(SDKROOT)/Developer/Library/Frameworks",
                "Image@2x.png",
                "$(OTHER_CFLAGS)",
                "",
                "a te\"st\" \a \b \f \n \r \t \v \' \" \\ string",
                "\\\\",
                "\"abc def\"",
                "\"\\\"ghi jkl\\\"\"",
                "Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş",
                "Path/WithoutSpaces/foo",
                "NO",
                "Path/To/Framwork.framework",
                "Framwork.framework",
                "YES",
                "YES_ERROR",
                "gnu99",
                "7.0",
                "Debug",
                "Image.png",
                "Classes/iPhone_target_Prefix.pch",
            };
            string[] expected = new string[]
            {
                "\"Path/With Spaces/foo\"",
                "\"$(ARCHS_STANDARD_INCLUDING_64_BIT)\"",
                "\"gnu++0x\"",
                "\"libc++\"",
                "\"iPhone Developer\"",
                "\"DEBUG=1\"",
                "\"$(inherited)\"",
                "\"1,2\"",
                "\"TestApp/TestApp-Prefix.pch\"",
                "\"TestApp/TestApp-Info.plist\"",
                "\"$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp\"",
                "\"$(SDKROOT)/Developer/Library/Frameworks\"",
                "\"Image@2x.png\"",
                "\"$(OTHER_CFLAGS)\"",
                "\"\"",
                "\"a te\\\"st\\\" \\a \\b \\f \\n \\r \\t \\v \\\' \\\" \\\\ string\"",
                "\"\\\\\\\\\"",
                "\"\\\"abc def\\\"\"",
                "\"\\\"\\\\\\\"ghi jkl\\\\\\\"\\\"\"",
                "\"Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş\"",
                "Path/WithoutSpaces/foo",
                "NO",
                "Path/To/Framwork.framework",
                "Framwork.framework",
                "YES",
                "YES_ERROR",
                "gnu99",
                "7.0",
                "Debug",
                "Image.png",
                "Classes/iPhone_target_Prefix.pch",
            };

            for (int ii = 0; ii < test.Length; ++ii)
            {
                string result = test[ii].ToLiteralIfRequired();
                Assert.AreEqual(expected[ii], result);
            }
        }

        [Test]
        public void ToLiteralTest()
        {
            string[] test = new string[]
            {
                "Path/With Spaces/foo",
                "$(ARCHS_STANDARD_INCLUDING_64_BIT)",
                "gnu++0x",
                "libc++",
                "iPhone Developer",
                "DEBUG=1",
                "$(inherited)",
                "1,2",
                "TestApp/TestApp-Prefix.pch",
                "TestApp/TestApp-Info.plist",
                "$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp",
                "$(SDKROOT)/Developer/Library/Frameworks",
                "Image@2x.png",
                "$(OTHER_CFLAGS)",
                "",
                "a te\"st\" \a \b \f \n \r \t \v \' \" \\ string",
                "\\\\",
                "\"abc def\"",
                "\"\\\"ghi jkl\\\"\"",
                "Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş",
            };
            string[] expected = new string[]
            {
                "\"Path/With Spaces/foo\"",
                "\"$(ARCHS_STANDARD_INCLUDING_64_BIT)\"",
                "\"gnu++0x\"",
                "\"libc++\"",
                "\"iPhone Developer\"",
                "\"DEBUG=1\"",
                "\"$(inherited)\"",
                "\"1,2\"",
                "\"TestApp/TestApp-Prefix.pch\"",
                "\"TestApp/TestApp-Info.plist\"",
                "\"$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp\"",
                "\"$(SDKROOT)/Developer/Library/Frameworks\"",
                "\"Image@2x.png\"",
                "\"$(OTHER_CFLAGS)\"",
                "\"\"",
                "\"a te\\\"st\\\" \\a \\b \\f \\n \\r \\t \\v \\\' \\\" \\\\ string\"",
                "\"\\\\\\\\\"",
                "\"\\\"abc def\\\"\"",
                "\"\\\"\\\\\\\"ghi jkl\\\\\\\"\\\"\"",
                "\"Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş\"",
            };

            for (int ii = 0; ii < test.Length; ++ii)
            {
                string result = test[ii].ToLiteral();
                Assert.AreEqual(expected[ii], result);
            }
        }

        [Test]
        public void FromLiteralTest()
        {
            string[] test = new string[]
            {
                "\"Path/With Spaces/foo\"",
                "\"$(ARCHS_STANDARD_INCLUDING_64_BIT)\"",
                "\"gnu++0x\"",
                "\"libc++\"",
                "\"iPhone Developer\"",
                "\"DEBUG=1\"",
                "\"$(inherited)\"",
                "\"1,2\"",
                "\"TestApp/TestApp-Prefix.pch\"",
                "\"TestApp/TestApp-Info.plist\"",
                "\"$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp\"",
                "\"$(SDKROOT)/Developer/Library/Frameworks\"",
                "\"Image@2x.png\"",
                "\"$(OTHER_CFLAGS)\"",
                "\"\"",
                "\"a te\\\"st\\\" \\a \\b \\f \\n \\r \\t \\v \\\' \\\" \\\\ string\"",
                "\"\\\\\\\\\"",
                "\"\\\"abc def\\\"\"",
                "\"\\\"\\\\\\\"ghi jkl\\\\\\\"\\\"\"",
                "\"Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş\"",
                "Path/WithoutSpaces/foo",
                "NO",
                "Path/To/Framwork.framework",
                "Framwork.framework",
                "YES",
                "YES_ERROR",
                "gnu99",
                "7.0",
                "Debug",
                "Image.png",
                "Classes/iPhone_target_Prefix.pch",
            };
            string[] expected = new string[]
            {
                "Path/With Spaces/foo",
                "$(ARCHS_STANDARD_INCLUDING_64_BIT)",
                "gnu++0x",
                "libc++",
                "iPhone Developer",
                "DEBUG=1",
                "$(inherited)",
                "1,2",
                "TestApp/TestApp-Prefix.pch",
                "TestApp/TestApp-Info.plist",
                "$(BUILT_PRODUCTS_DIR)/TestApp.app/TestApp",
                "$(SDKROOT)/Developer/Library/Frameworks",
                "Image@2x.png",
                "$(OTHER_CFLAGS)",
                "",
                "a te\"st\" \a \b \f \n \r \t \v \' \" \\ string",
                "\\\\",
                "\"abc def\"",
                "\"\\\"ghi jkl\\\"\"",
                "Ő ő Œ œ Ŕ ŕ Ŗ ŗ Ř ř Ś ś Ŝ ŝ Ş ş",
                "Path/WithoutSpaces/foo",
                "NO",
                "Path/To/Framwork.framework",
                "Framwork.framework",
                "YES",
                "YES_ERROR",
                "gnu99",
                "7.0",
                "Debug",
                "Image.png",
                "Classes/iPhone_target_Prefix.pch",
            };

            for (int ii = 0; ii < test.Length; ++ii)
            {
                string result = test[ii].FromLiteral();
                Assert.AreEqual(expected[ii], result);
            }
        }

        [Test]
        public void StringListToListTest()
        {
            string test = "\"dff \\\"sd/s f/d\\\" sd/sd \\\"\\\\\\\\\\\\\\\\\\\" Ʀ \\\"ab cd\\\"\"";
            var expected = new System.Collections.Generic.List<string>
            {
                "dff",
                "\"sd/s f/d\"",
                "sd/sd",
                "\"\\\\\\\\\"",
                "Ʀ",
                "\"ab cd\""
            };
            var result = StringUtils.StringListToList(test);

            for (int ii = 0; ii < expected.Count; ++ii)
            {
                Assert.AreEqual(expected[ii], result[ii]);
            }
        }

        [Test]
        public void ListToStringListTest()
        {
            var test = new System.Collections.Generic.List<string>
            {
                "dff",
                "\"sd/s f/d\"",
                "sd/sd",
                "\"\\\\\\\\\"",
                "\\\\\\\\",
                "Ʀ",
                "ab cd"
            };
            string expected = "\"dff \\\"sd/s f/d\\\" sd/sd \\\"\\\\\\\\\\\\\\\\\\\" \\\\\\\\\\\\\\\\ Ʀ \\\"ab cd\\\"\"";
            string result = StringUtils.ListToStringList(test);
            Assert.AreEqual(expected, result);
        }
    }
}
