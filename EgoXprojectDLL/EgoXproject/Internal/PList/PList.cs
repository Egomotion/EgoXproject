//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System;

namespace Egomotion.EgoXproject.Internal
{
    internal class PList
    {
        PListDictionary _dict = new PListDictionary();

        XDeclaration _decl = new XDeclaration("1.0", "UTF-8", null);
        XDocumentType _docType = new XDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
        //      XDocumentType _docTypeAlt = new XDocumentType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

        string _version = "1.0";
        string _path = "";
        XDocumentType _usedDocType;
        bool _loaded = false;

        public PList()
        {
            _loaded = false;
        }

        public PList(string pathToPListFile)
        {
            if (!Load(pathToPListFile))
            {
                throw new ArgumentException("Failed to load PList: " + pathToPListFile, nameof (pathToPListFile));
            }
        }


        public bool Load(string pathToPListFile)
        {
            _loaded = false;

            if (!File.Exists(pathToPListFile))
            {
                return false;
            }

            XDocument doc;
            XmlReader reader = null;

            try
            {
                reader = XmlReader.Create(pathToPListFile, ReaderSettings());
                doc = XDocument.Load(reader);
                reader.Close();
            }
            catch
            {
                if (reader != null)
                {
                    reader.Close();
                }

                Debug.LogError("EgoXproject: Could not parse plist " + pathToPListFile);
                return false;
            }

            if (CommonLoad(doc))
            {
                _path = pathToPListFile;
                return true;
            }
            else
            {
                Debug.LogError("EgoXproject: Error with plist file. See previous error messages for details. " + pathToPListFile);
                return false;
            }
        }

        public bool LoadFromString(string content)
        {
            _loaded = false;
            XDocument doc;
            XmlReader reader = null;

            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            try
            {
                reader = XmlReader.Create(new StringReader(content), ReaderSettings());
                doc = XDocument.Load(reader);
                reader.Close();
            }
            catch
            {
                if (reader != null)
                {
                    reader.Close();
                }

                Debug.LogError("EgoXproject: Could not parse plist");
                return false;
            }

            if (CommonLoad(doc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool CommonLoad(XDocument doc)
        {
            if (!IsValidDelaration(doc.Declaration))
            {
                Debug.LogError("EgoXproject: Not a supported plist file");
                return false;
            }

            XElement plist = doc.Element("plist");

            if (!IsSupportedVersion(plist.Attribute("version").Value))
            {
                Debug.LogError("EgoXproject: Unsupported plist version");
                return false;
            }

            XElement dict = plist.Element("dict");

            if (dict == null)
            {
                Debug.LogError("EgoXproject: No Root dictionary found");
                return false;
            }

            var dictElements = dict.Elements();
            _dict = ParseDictionary(dictElements);
            RecordDocType(doc.DocumentType);
            _loaded = true;
            return true;
        }

        XmlReaderSettings ReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            settings.XmlResolver = null;
            return settings;
        }

        public PListDictionary Root
        {
            get
            {
                return _dict;
            }
            set
            {
                _dict = value;
            }
        }

        public bool HasPath
        {
            get
            {
                return !string.IsNullOrEmpty(_path);
            }
        }

        public string SavePath
        {
            get
            {
                return _path;
            }
        }

        public bool Save()
        {
            if (HasPath)
            {
                return Save(_path);
            }
            else
            {
                Debug.LogError("EgoXproject: No path specifed");
                return false;
            }
        }

        bool IsValidPath(string pathToCheck)
        {
            if (string.IsNullOrEmpty(pathToCheck))
            {
                return false;
            }

            try
            {
                if (Directory.Exists(pathToCheck))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            try
            {
                string fileName = Path.GetFileName(pathToCheck);

                if (string.IsNullOrEmpty(fileName))
                {
                    return false;
                }

                if (fileName.StartsWith(" ", System.StringComparison.InvariantCultureIgnoreCase) ||
                        fileName.EndsWith(" ", System.StringComparison.InvariantCultureIgnoreCase) ||
                        fileName.StartsWith(".", System.StringComparison.InvariantCultureIgnoreCase) ||
                        fileName.Contains(":"))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            try
            {
                string dir = Path.GetDirectoryName(pathToCheck);

                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            //TODO: do a more complete check?
            return true;
        }

        public bool Save(string pathToSaveFile, bool overwrite = false)
        {
            if (!IsValidPath(pathToSaveFile))
            {
                Debug.LogError("EgoXproject: Invalid Save Path: " + pathToSaveFile);
                return false;
            }

            if (!overwrite)
            {
                if (_path != pathToSaveFile && File.Exists(pathToSaveFile))
                {
                    Debug.LogError("EgoXproject: File exists: " + pathToSaveFile);
                    return false;
                }
            }

            XDocument doc = new XDocument(_decl);
            bool dtd = false;

            if (_loaded)
            {
                if (_usedDocType != null)
                {
                    doc.AddFirst(_usedDocType);
                    dtd = true;
                }
            }
            else
            {
                doc.AddFirst(_docType);
                dtd = true;
            }

            XElement plist = new XElement("plist");
            plist.SetAttributeValue("version", _version);
            plist.Add(_dict.Xml());
            doc.Add(plist);

            var stringWriter = new Utf8StringWriter();
            doc.Save(stringWriter);
            var content = stringWriter.GetStringBuilder().ToString();
            //http://stackoverflow.com/questions/10303315/convert-stringwriter-to-string
            string[] separator = { System.Environment.NewLine };
            string[] lines = content.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            //Remove the spurious [] that get added to DOCTYPE. Grrr.
            if (dtd)
            {
                for (int ii = 0; ii < lines.Length; ii++)
                {
                    var line = lines[ii];

                    if (line.Contains("<!DOCTYPE") && line.Contains("[]"))
                    {
                        lines[ii] = line.Replace("[]", "");
                        break;
                    }
                }
            }

            File.WriteAllLines(pathToSaveFile, lines);
            //TODO this is a crap check.
            bool bSaved = File.Exists(pathToSaveFile);

            if (bSaved)
            {
                _path = pathToSaveFile;
            }

            return bSaved;
        }

        //http://stackoverflow.com/questions/3871738/force-xdocument-to-write-to-string-with-utf-8-encoding
        class Utf8StringWriter : StringWriter
        {
            public override System.Text.Encoding Encoding
            {
                get
                {
                    return System.Text.Encoding.UTF8;
                }
            }
        }

        #region validation

        bool IsValidDelaration(XDeclaration decl)
        {
            if (_decl.Encoding.ToLower() != decl.Encoding.ToLower() ||
                    _decl.Version != decl.Version ||
                    !string.IsNullOrEmpty(decl.Standalone))
            {
                return false;
            }

            return true;
        }

        void RecordDocType(XDocumentType docType)
        {
            if (docType != null)
            {
                //specifying the copied doctype like this caused empty [] to be added
                //_usedDocType = new XDocumentType(docType);
                //but like this it is fine!
                _usedDocType = new XDocumentType(docType.Name, docType.PublicId, docType.SystemId, (string.IsNullOrEmpty(docType.InternalSubset) ? null : docType.InternalSubset));
            }
            else
            {
                _usedDocType = null;
            }
        }

        bool IsSupportedVersion(string version)
        {
            return _version == version;
        }

        #endregion

        #region parsing

        PListDictionary ParseDictionary(IEnumerable<XElement> elements)
        {
            PListDictionary dict = new PListDictionary();

            if (elements != null)
            {
                for (int ii = 0; ii < elements.Count(); ii += 2)
                {
                    XElement key = elements.ElementAt(ii);
                    XElement val = elements.ElementAt(ii + 1);
                    var element = ParseValue(val);

                    if (element != null)
                    {
                        dict[key.Value] = element;
                    }
                }
            }

            return dict;
        }

        IPListElement ParseValue(XElement value)
        {
            string key = value.Name.ToString();

            switch (key)
            {
            case "string":
                return new PListString(value.Value);

            case "integer":
                int i;

                if (int.TryParse(value.Value, out i))
                {
                    return new PListInteger(i);
                }

                return null;

            case "real":
                float f;

                if (float.TryParse(value.Value, out f))
                {
                    return new PListReal(f);
                }

                return null;

            case "true":
            case "false":
                return new PListBoolean((key == "true"));

            case "date":
                return new PListDate(value.Value);

            case "data":
                return new PListData(value.Value);

            case "dict":
                return ParseDictionary(value.Elements());

            case "array":
                return ParseArray(value.Elements());

            default:
                //throw new ArgumentException("Unsupported");
                return null;
            }
        }

        PListArray ParseArray(IEnumerable<XElement> elements)
        {
            PListArray arrayElement = new PListArray();

            foreach (XElement e in elements)
            {
                IPListElement value = ParseValue(e);

                if (value != null)
                {
                    arrayElement.Add(value);
                }
            }

            return arrayElement;
        }

        #endregion

        public override string ToString()
        {
            if (_dict == null)
            {
                return null;
            }
            else
            {
                return _dict.ToString();
            }
        }
    }
}
