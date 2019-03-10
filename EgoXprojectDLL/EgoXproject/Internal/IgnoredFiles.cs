// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal static class IgnoredFiles
    {
        static string[] _defaultIgnoredFiles =
        {
            ".git",
            ".svn",
            ".hg",
            "*.meta",
            ".DS_Store",
            "Thumbs.db",
            "*.bak",
            "*~",
            "*.dll",
            "*.egoxc",
            "egoxproject.configurations",
            "egoxproject.settings",
            "egoxproject.version"
        };
        static List<string> _customIgnoredFiles = new List<string>();
        static List<string> _ignoredFiles = new List<string>();

        static IgnoredFiles()
        {
            _ignoredFiles = new List<string>(_defaultIgnoredFiles);
        }

        public static void SetIngnoredFiles(string[] ignored)
        {
            _customIgnoredFiles = new List<string>();
            _ignoredFiles = new List<string>(_defaultIgnoredFiles);

            foreach (var s in ignored)
            {
                Add(s);
            }
        }

        public static void Add(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || _ignoredFiles.Contains(fileName))
            {
                return;
            }

            _ignoredFiles.Add(fileName);
            _customIgnoredFiles.Add(fileName);
        }

        public static void Remove(string fileName)
        {
            if (System.Array.IndexOf(_defaultIgnoredFiles, fileName) > -1)
            {
                return;
            }

            _ignoredFiles.Remove(fileName);
            _customIgnoredFiles.Remove(fileName);
        }

        public static string[] DefaultList
        {
            get
            {
                return _defaultIgnoredFiles;
            }
        }

        public static string[] CustomList
        {
            get
            {
                return _customIgnoredFiles.ToArray();
            }
        }

        public static string[] IgnoreList
        {
            get
            {
                return _ignoredFiles.ToArray();
            }
        }

        public static bool ShouldIgnore(string fileName)
        {
            fileName = System.IO.Path.GetFileName(fileName);
            // Construct corresponding regular expression. Note Regex.Escape!
            RegexOptions options = RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase;
            Regex[] patterns = _ignoredFiles.Select(f => new Regex("^" + Regex.Escape(f).Replace("\\*", ".*") + "$", options)).ToArray();
            return patterns.Any(p => p.IsMatch(fileName));
        }
    }
}

