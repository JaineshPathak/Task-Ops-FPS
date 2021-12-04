using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public Transform fpMuzzleFlash, tpMuzzleFlash;
    public Transform leftHandTarget;
    public Transform shootPoint, shootPointForward;
    public SkinnedMeshRenderer[] skinnedMeshesToDisable;
    public Vector3 weaponBackScale;

    public void EnableSkinMeshRenderers()
    {
        if (skinnedMeshesToDisable.Length > 0)
        {
            for (int i = 0; i < skinnedMeshesToDisable.Length; i++)
            {
                skinnedMeshesToDisable[i].enabled = true;
            }
        }
    }

    public void DisableSkinMeshRenderers()
    {
        if (skinnedMeshesToDisable.Length > 0)
        {
            for (int i = 0; i < skinnedMeshesToDisable.Length; i++)
            {
                skinnedMeshesToDisable[i].enabled = false;
            }
        }
    }
}
