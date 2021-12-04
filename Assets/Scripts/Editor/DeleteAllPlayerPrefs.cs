using UnityEngine;
using UnityEditor;

public class DeleteAllPlayerPrefs : EditorWindow
{
    [MenuItem("Window/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPref()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted!");
    }
}