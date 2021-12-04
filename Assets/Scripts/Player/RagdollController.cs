using UnityEngine;
using RootMotion.FinalIK;

public class RagdollController : MonoBehaviour
{
    private Collider[] ragColliders;
    private Rigidbody[] rigidbodies;

    private GameObject playerGraphics;
    private Animator anim;
    private AimIK aimIK;

	public void SetupRagdolls(GameObject _playerGraphics)
    {
        playerGraphics = _playerGraphics;

        anim = playerGraphics.GetComponent<Animator>();
        aimIK = playerGraphics.GetComponent<AimIK>();

        rigidbodies = playerGraphics.GetComponentsInChildren<Rigidbody>();
        ragColliders = playerGraphics.GetComponentsInChildren<Collider>();

        DisableRagdolls();
    }

    public void EnableRagdolls()
    {
        for (int i = 0; i < ragColliders.Length; i++)
        {
            if(ragColliders[i].isTrigger && ragColliders[i].enabled)
                ragColliders[i].isTrigger = false;
        }

        for (int i = 0; i < rigidbodies.Length; i++)
            rigidbodies[i].isKinematic = false;

        anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
        anim.enabled = false;

        aimIK.enabled = false;
    }

    public void DisableRagdolls()
    {
        for (int i = 0; i < ragColliders.Length; i++)
        {
            if (!ragColliders[i].isTrigger && ragColliders[i].enabled)
                ragColliders[i].isTrigger = true;
        }

        for (int i = 0; i < rigidbodies.Length; i++)
            rigidbodies[i].isKinematic = true;

        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.enabled = true;

        aimIK.enabled = true;
    }
}
