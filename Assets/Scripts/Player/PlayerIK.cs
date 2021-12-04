using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIK : Bolt.EntityBehaviour<ISniperPlayerState>
{
    Animator anim;

    [SerializeField] Vector3 thePos;
    [SerializeField] GameObject prefab;
    [SerializeField] float pitchAmount = 1.5f;

    public override void Attached()
    {
        anim = GetComponent<Animator>();
        //head = anim.GetBoneTransform(HumanBodyBones.Head);
    }

    void OnAnimatorIK()
    {
        float pitchOffset = /*-state.camPitch*/ 0f * 0.01f;

        thePos = transform.position + transform.forward + new Vector3(0, pitchAmount + pitchOffset, 0);
        prefab.transform.position = thePos;

        //anim.SetLookAtWeight(1f);
        //anim.SetLookAtPosition(/*transform.position + transform.forward + new Vector3(0, 1.5f + pitchOffset, 0)*/ thePos);

        /*anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKPosition(AvatarIKGoal.RightHand, thePos);
        anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.EulerAngles(thePos));

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, thePos);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.EulerAngles(thePos));*/
    }
}
