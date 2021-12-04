using UnityEngine;
using WeaponBobbing;

[RequireComponent(typeof(WeaponManager3))]
public class WeaponBobAssistant : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerMotor pMotor;
    private WeaponBobCSGO wBob;
    private Transform wHolder;

    private float defSideBobSpeed;
    private float defSideBobAmount;
    private Quaternion defRotation = Quaternion.identity;
    private Quaternion runRotation = Quaternion.identity;

    bool bRunning
    {
        get { return playerController.state.IsRunning; }
    }

    bool bCrouching
    {
        get { return playerController.state.IsCrouching; }
    }

    void Start ()
    {
        playerController = GetComponent<PlayerController>();
        pMotor = GetComponent<PlayerMotor>();

        if (playerController.entity.isOwner)
        {
            wHolder = CameraController.instance.myWeaponHolder;
            wBob = wHolder.GetComponent<WeaponBobCSGO>();

            defRotation = wHolder.localRotation;
            defSideBobSpeed = wBob.sideBobbingSpeed;
            defSideBobAmount = wBob.sideBobbingAmount;
        }

        //wBob = wHolder.GetComponent<WeaponBobCSGO>();
	}

    void FixedUpdate()
    {
        if (playerController.entity.isOwner && (!PauseMenu.isOn && !playerController.state.IsDead))
            PerformBobbing();
	}

    void PerformBobbing()
    {
        if (wBob.isActiveAndEnabled)
        {
            if (pMotor.CheckGrounded())
            {
                if (bRunning)
                {
                    //Player is on ground and running
                    wBob.sideBobbingSpeed = playerController.activeWeapon.runSideBobSpeed;
                    wBob.sideBobbingAmount = playerController.activeWeapon.runSideBobAmount;

                    if (!playerController.activeWeapon.WeaponIsReloading() && !playerController.activeWeapon.WeaponIsFiring())
                    {
                        runRotation.eulerAngles = new Vector3(playerController.activeWeapon.runVectRotation.x, playerController.activeWeapon.runVectRotation.y, playerController.activeWeapon.runVectRotation.z);
                        wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, runRotation, 10 * Time.fixedDeltaTime);
                    }
                    else
                        wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, defRotation, 10 * Time.fixedDeltaTime);
                }
                else if(bCrouching)
                {
                    wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, defRotation, 10 * Time.fixedDeltaTime);

                    if (playerController.activeWeapon.ironSightFlag)
                    {
                        wBob.sideBobbingSpeed = playerController.activeWeapon.aimSideBobSpeed;
                        wBob.sideBobbingAmount = playerController.activeWeapon.aimSideBobAmount;
                    }
                    else
                    {
                        wBob.sideBobbingSpeed = playerController.activeWeapon.crouchSideBobSpeed;
                        wBob.sideBobbingAmount = playerController.activeWeapon.crouchSideBobAmount;
                    }
                }
                else if(playerController.activeWeapon.ironSightFlag)
                {
                    wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, defRotation, 10 * Time.fixedDeltaTime);

                    wBob.sideBobbingSpeed = playerController.activeWeapon.aimSideBobSpeed;
                    wBob.sideBobbingAmount = playerController.activeWeapon.aimSideBobAmount;
                }
                else
                {
                    //Player is on ground and not running
                    wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, defRotation, 10 * Time.fixedDeltaTime);
                    wBob.sideBobbingSpeed = defSideBobSpeed;
                    wBob.sideBobbingAmount = defSideBobAmount;
                }
            }
            else
            {
                //Player is in air
                wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, defRotation, 10 * Time.fixedDeltaTime);
                wBob.sideBobbingSpeed = 0;
                wBob.sideBobbingAmount = 0;
            }
        }
    }
}
