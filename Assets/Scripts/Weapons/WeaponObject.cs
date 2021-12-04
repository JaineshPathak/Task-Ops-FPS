using UnityEngine;

[System.Serializable]
public class WeaponObject : ScriptableObject
{
    [Header("Prefabs Settings")]
    public string weaponName;
    public GameObject fpWeaponPrefab;
    public GameObject tpWeaponPrefab;

    [Header("First Person Bob Settings")]
    public float runSideBobSpeed = 0.2f;
    public float runSideBobAmount = 0.07f;
    public Vector3 runVectRotation;

    [Header("Muzzle Flash Settings")]
    public GameObject fpMuzzlePoint;
    public GameObject tpMuzzlePoint;

    [Header("Animations Settings")]
    public string fireAnimationName;
    public string boltAnimationName;
    public string reloadAnimationName;
    public string selectAnimationName;

    public string firingParameter;
    public string reloadingParameter;
    public string selectParameter;

    [Header("Firing Settings")]
    public float fireRate;
}
