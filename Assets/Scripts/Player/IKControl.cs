using UnityEngine;

[RequireComponent(typeof(WeaponManager3))]
public class IKControl : MonoBehaviour
{
    private Transform leftHTarget;
    [SerializeField] private float leftHandWeight = 1f;

    private Animator anim;
    private WeaponManager3 wManager;
    private WeaponBase wBase;

	// Use this for initialization
	void Start ()
    {
        wManager = transform.root.GetComponent<WeaponManager3>();
        wBase = wManager.activeWeapon;
        anim = GetComponent<Animator>();

        leftHTarget = wBase.tpWeaponInfo.leftHandTarget;
	}

    private void OnAnimatorIK(int layerIndex)
    {
        if(anim.isActiveAndEnabled)
        {
            wBase = wManager.activeWeapon;

            leftHandWeight = wBase.WeaponIsReloading() ? 0 : 1f;

            leftHTarget = wBase.tpWeaponInfo.leftHandTarget;

            if (leftHTarget != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHTarget.position);

                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHTarget.rotation);
            }
        }
    }
}
