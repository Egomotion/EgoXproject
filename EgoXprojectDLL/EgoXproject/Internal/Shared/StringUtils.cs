//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace Egomotion.EgoXproject.Internal
{
    internal static class StringUtils
    {
        /// <summary>
        /// Convert an array to a string with a specified delimeter
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="array">Array.</param>
        /// <param name="separator">Separator.</param>
        public static string ArrayToString(string[] array, string separator = ", ")
        {
            if (array == null || array.Length <= 0)
            {
                return "";
            }

            var sb = new StringBuilder();
            sb.Append(array[0]);

            for (int ii = 1; ii < array.Length; ++ii)
            {
                sb.Append(separator);
                sb.Append(array[ii]);
            }

            return sb.ToString();
        }

        static readonly string _invalidCharPattern = @"[^A-Za-z0-9\p{L}/_\.]";

        //true if it contains anything other than A-z, 0-9 / _ . and unicode characters
        public static bool ContainsInvalidCharacters(this string value)
        {
            return Regex.IsMatch(value, _invalidCharPattern);
        }

        public static string EncloseInQuotes(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }

            if (value.StartsWith("\""))
            {
                return value;
            }

            return "\"" + value + "\"";
        }

        public static string ToLiteralIfRequired(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }
            else if (value.ContainsInvalidCharacters())
            {
                return value.ToLiteral();
            }
            else
            {
                return value;
            }
        }

        public static string ToLiteral(this string value)
        {
            StringBuilder literal = new StringBuilder(value.Length + 2);
            literal.Append("\"");

            foreach (var c in value)
            {
                switch (c)
                {
                case '\'':
                    literal.Append(@"\'");
                    break;

                case '\"':
                    literal.Append("\\\"");
                    break;

                case '\\':
                    literal.Append(@"\\");
                    break;

                case '\0':
                    literal.Append(@"\0");
                    break;

                case '\a':
                    literal.Append(@"\a");
                    break;

                case '\b':
                    literal.Append(@"\b");
                    break;

                case '\f':
                    literal.Append(@"\f");
                    break;

                case '\n':
                    literal.Append(@"\n");
                    break;

                case '\r':
                    literal.Append(@"\r");
                    break;

                case '\t':
                    literal.Append(@"\t");
                    break;

                case '\v':
                    literal.Append(@"\v");
                    break;

                default:
                    //don't need to worry about unicode as source files are UTF-8
                    literal.Append(c);
                    break;
                }
            }

            literal.Append("\"");
            return literal.ToString();
        }

        public static string FromLiteral(this string value)
        {
            StringBuilder sb = new StringBuilder(value.Length);

            if (value.StartsWith("\"", System.StringComparison.InvariantCultureIgnoreCase) &&
                    value.EndsWith("\"", System.StringComparison.InvariantCultureIgnoreCase) &&
                    value.Length > 1)
            {
                value = value.Remove(value.Length - 1, 1).Remove(0, 1);
            }

            for (int ii = 0; ii < value.Length; ++ii)
            {
                char c = value[ii];

                if (c == '\\')
                {
                    ++ii;

                    if (ii >= value.Length)
                    {
                        //incomplete control sequence
                        Console.WriteLine("Missing escape sequence at end of string: " + value);
                        continue;
                    }

                    c = value[ii];

                    switch (c)
                    {
                    case '\'':
                        sb.Append('\'');
                        break;

                    case '\"':
                        sb.Append('\"');
                        break;

                    case '\\':
                        sb.Append('\\');
                        break;

                    case '0':
                        sb.Append('\0');
                        break;

                    case 'a':
                        sb.Append('\a');
                        break;

                    case 'b':
                        sb.Append('\b');
                        break;

                    case 'f':
                        sb.Append('\f');
                        break;

                    case 'n':
                        sb.Append('\n');
                        break;

                    case 'r':
                        sb.Append('\r');
                        break;

                    case 't':
                        sb.Append('\t');
                        break;

                    case 'v':
                        sb.Append('\v');
                        break;

                    default:
                        Console.WriteLine("Unrecognized escape sequence in string at position " + ii + ": " + value);
                        continue;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static List<string> StringListToList(string stringList)
        {
            List<string> values = new List<string>();
            stringList = stringList.FromLiteral();
            bool inQuotedEntry = false;
            int entryStart = 0;
            char currentChar;

            for (int ii = 0; ii < stringList.Length; ++ii)
            {
                currentChar = stringList[ii];

                if (currentChar == '\"')
                {
                    inQuotedEntry = !inQuotedEntry;
                }
                else if (currentChar == ' ' && !inQuotedEntry)
                {
                    var entry = stringList.Substring(entryStart, ii - entryStart);
                    values.Add(entry);
                    entryStart = ii + 1;
                }
            }

            //extract the final value
            if (entryStart < stringList.Length)
            {
                var entry = stringList.Substring(entryStart, stringList.Length - entryStart);
                values.Add(entry);
            }

            return values;
        }

        public static string ListToStringList(List<string> values)
        {
            StringBuilder result = new StringBuilder();

            if (values == null || values.Count <= 0)
            {
                return string.Empty.ToLiteral();
            }

            for (int ii = 0; ii < values.Count; ++ii)
            {
                if (ii > 0)
                {
                    result.Append(" ");
                }

                if (values[ii].Contains(" ") && !values[ii].StartsWith("\"", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Append("\"");
                    result.Append(values[ii]);
                    result.Append("\"");
                }
                else
                {
                    result.Append(values[ii]);
                }
            }

            return result.ToString().ToLiteral();
        }
    }
}
