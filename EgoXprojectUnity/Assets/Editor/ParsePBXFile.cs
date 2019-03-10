using UnityEngine;
using UnityEditor;
using System.Collections;
using Egomotion.EgoXproject.Internal;
using System.IO;
public static class ParsePBXFile
{
    [MenuItem("Window/EgoXproject Dev/Parse PBX File")]
    static void Parse()
    {
        var fileName = EditorUtility.OpenFilePanel("Open PBX File", "", "pbxproj");

        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        PBXProjParser parser = new PBXProjParser();
        var dic = parser.Parse(fileName);
        Debug.Log(dic.Count);
    }

    [MenuItem("Window/EgoXproject Dev/Load PBX File")]
    static void LoadProj()
    {
        var fileName = EditorUtility.OpenFilePanel("Open PBX File", "", "pbxproj");

        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        PBXProj proj = new PBXProj();

        if (proj.Load(fileName))
        {
            Debug.Log("OK");
            //proj
        }
        else
        {
            Debug.Log("Failed to load");
        }
    }

    [MenuItem("Window/EgoXproject Dev/Load and Save copy of PBX File")]
    static void LoadAndSaveCopy()
    {
        var fileName = EditorUtility.OpenFilePanel("Open PBX File", "", "pbxproj");

        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        PBXProj proj = new PBXProj();

        if (proj.Load(fileName))
        {
            Debug.Log("OK");
            File.Copy(fileName, fileName + ".orig");

            if (proj.Save())
            {
                File.Move(fileName, fileName + ".copy");
                File.Move(fileName + ".orig", fileName);
            }

            //proj
        }
        else
        {
            Debug.Log("Failed to load");
        }
    }
}
