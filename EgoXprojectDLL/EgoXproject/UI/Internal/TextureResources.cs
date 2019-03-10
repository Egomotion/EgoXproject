//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------
//using System.IO;
//using System.Reflection;
//using UnityEngine;
//using System.Collections.Generic;

using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.UI.Internal
{
    internal class TextureResources
    {
        Dictionary<string, Texture2D> _resources = new Dictionary<string, Texture2D>();

        //explictly load to ensure it is done at the right time
        public void Load()
        {
            LoadAll();
        }

        //explictly unload to ensure it is done at the right time
        public void Unload()
        {
            foreach (var tex in _resources.Values)
            {
                if (tex != null)
                {
                    UnityEngine.Object.DestroyImmediate(tex);
                }
            }

            _resources.Clear();
        }

        public Texture2D Plus
        {
            get
            {
                return GetSkinDependentTexture("plus");
            }
        }

        public Texture2D Minus
        {
            get
            {
                return GetSkinDependentTexture("minus");
            }
        }

        public Texture2D Edit
        {
            get
            {
                return GetSkinDependentTexture("edit");
            }
        }

        public Texture2D Help
        {
            get
            {
                return GetSkinDependentTexture("help");
            }
        }

        public Texture2D Warning
        {
            get
            {
                return GetTexture("warning");
            }
        }

        public Texture2D Refresh
        {
            get
            {
                return GetSkinDependentTexture("refresh");
            }
        }

        public Texture2D TextAreaBorder
        {
            get
            {
                return GetTexture("textAreaBorder");
            }
        }

        public Texture2D TopBorder
        {
            get
            {
                return GetTexture("topBorder");
            }
        }

        Texture2D GetSkinDependentTexture(string name)
        {
            name += (EditorGUIUtility.isProSkin ? "-dark" : "-light");
            return GetTexture(name);
        }

        Texture2D GetTexture(string name)
        {
            Texture2D tex = null;
            _resources.TryGetValue(name, out tex);
            return tex;
        }


        void LoadAll()
        {
            _resources.Clear();
            LoadTexturesInResourceFile("Resources.txt");

            if (EditorGUIUtility.isProSkin)
            {
                LoadTexturesInResourceFile("Resources-dark.txt");
            }
            else
            {
                LoadTexturesInResourceFile("Resources-light.txt");
            }
        }

        void LoadTexturesInResourceFile(string fileName)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream stream = myAssembly.GetManifestResourceStream("Egomotion.EgoXproject.Resources." + fileName);

            if (stream == null)
            {
                return;
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string entry = reader.ReadLine().Trim();

                    if (entry.StartsWith("//"))
                    {
                        continue;
                    }

                    string[] elements = entry.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                    if (elements.Length != 3)
                    {
                        continue;
                    }

                    int w = 0, h = 0;

                    if (!int.TryParse(elements[1], out w))
                    {
                        continue;
                    }

                    if (!int.TryParse(elements[2], out h))
                    {
                        continue;
                    }

                    var tex = LoadTexture(elements[0], w, h);

                    if (tex != null)
                    {
                        _resources.Add(elements[0], tex);
                    }
                }
            }
        }

        Texture2D LoadTexture(string resourceName, int width, int height)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Egomotion.EgoXproject.Resources." + resourceName + ".png");

            if (myStream == null)
            {
                return null;
            }

            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.LoadImage(ReadToEnd(myStream));
            myStream.Close();

            if (texture == null)
            {
                Debug.LogError("EgoXproject: Missing Dll resource: " + resourceName);
            }

            return texture;
        }

        byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];
                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();

                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte) nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;

                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}
