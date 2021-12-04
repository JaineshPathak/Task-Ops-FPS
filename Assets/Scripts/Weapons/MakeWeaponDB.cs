using UnityEngine;

#if UNITY_EDITOR //Editor only tag
using UnityEditor;
#endif //Editor only tag

public class MakeWeaponDB
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Weapon DB")]
    public static void Create()
    {
        WeaponDB asset = ScriptableObject.CreateInstance<WeaponDB>();
        AssetDatabase.CreateAsset(asset, "Assets/NewWeaponDB.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
}