using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GuidGenerator : Editor
{
    [MenuItem("Window/Egomotion/GUID Generator")]
    static void Generate()
    {
        int count = 1000;
        List<string> guids = new List<string>(count);

        while (guids.Count < count)
        {
            string uid;

            do
            {
                uid = System.Guid.NewGuid().ToString();
                uid = uid.Replace("-", "");
                uid = uid.Substring(0, 24);
                uid = uid.ToUpper();
            }
            while (guids.Contains(uid));

            guids.Add(uid);
        }

        File.WriteAllLines("Assets/guids.txt", guids.ToArray());
    }

}
