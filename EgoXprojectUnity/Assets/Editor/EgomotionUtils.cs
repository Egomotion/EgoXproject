using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EgomotionUtils
{
    [MenuItem("Window/Egomotion/Clear Player Prefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
