using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKTest : MonoBehaviour
{
    Animator anim;
    [SerializeField] Transform lookAtObj;
    [SerializeField] Transform lookAtObjHandle;

    [SerializeField] Transform rightElbowObj;
	
    // Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
	}

    private void OnAnimatorIK(int layerIndex)
    {
        if(lookAtObj != null)
        {
            anim.SetLookAtWeight(1f);
            anim.SetLookAtPosition(lookAtObj.position);

            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
            
        }
    }
}
