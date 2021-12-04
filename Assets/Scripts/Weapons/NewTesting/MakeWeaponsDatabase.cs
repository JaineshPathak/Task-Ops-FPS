using UnityEngine;

#if UNITY_EDITOR //Editor only tag
using UnityEditor;
#endif //Editor only tag

public class MakeWeaponsDatabase
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Weapons Database")]
    public static void Create()
    {
        WeaponsDatabase asset = ScriptableObject.CreateInstance<WeaponsDatabase>();
        AssetDatabase.CreateAsset(asset, "Assets/NewWeaponsDatabase.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
}