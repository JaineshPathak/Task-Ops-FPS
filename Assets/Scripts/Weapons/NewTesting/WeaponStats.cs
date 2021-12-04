using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("Prefabs Settings")]
    public string weaponName;
    public GameObject fpPrefab;
    public GameObject tpPrefab;

    [Header("First Person Bob Settings")]
    public float runSideBobSpeed = 0.2f;
    public float runSideBobAmount = 0.07f;
    public Vector3 runVectRotation;

    [Header("Muzzle Flash Settings")]
    public GameObject fpMuzzlePoint;
    public GameObject tpMuzzlePoint;

    [Header("Animations Settings")]
    public string firingParameter;
    public string reloadingParameter;
    public string selectParameter;

    [Header("Firing Settings")]
    public float fireRate;
    public int bodyDamage;
    public int headShotDamage;
}
