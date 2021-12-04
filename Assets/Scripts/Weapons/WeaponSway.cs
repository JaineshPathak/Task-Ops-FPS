using UnityEngine;

[RequireComponent(typeof(WeaponManager3))]
public class WeaponSway : MonoBehaviour
{
    private PlayerController playerController;
    private Transform wHolder;
    
    private Vector3 newLocation = Vector3.zero;
    private Vector3 defLocation;
    private Quaternion newRotation = Quaternion.identity;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        if ( (playerController.entity != null) && playerController.entity.isOwner)
        {
            wHolder = CameraController.instance.myWeaponHolder;
            defLocation = wHolder.transform.localPosition;
        }
    }

    void FixedUpdate()
    {
        if (playerController.entity.isOwner && (!PauseMenu.isOn && !playerController.state.IsDead))
        {
            float x = Input.GetAxis("Mouse X");
            float z = Input.GetAxis("Mouse Y");

            PerformSway(x, z);
        }
    }

    void PerformSway(float x, float z)
    {
        //Location sway

        if (!playerController.activeWeapon.ironSightFlag)
        {
            float swayX = -x * playerController.activeWeapon.swayAmount;
            float swayY = -z * playerController.activeWeapon.swayAmount;

            swayX = Mathf.Clamp(swayX, -playerController.activeWeapon.maxSwayAmount, playerController.activeWeapon.maxSwayAmount);
            swayY = Mathf.Clamp(swayY, -playerController.activeWeapon.maxSwayAmount, playerController.activeWeapon.maxSwayAmount);

            //wBase.swayAmount = Mathf.Clamp()
            newLocation = new Vector3(defLocation.x + swayX, defLocation.y + swayY, defLocation.z);
            wHolder.transform.localPosition = Vector3.Slerp(wHolder.transform.localPosition, newLocation, playerController.activeWeapon.swaySmoothness * Time.fixedDeltaTime);


            //Rotation sway
            float tiltZ = x * playerController.activeWeapon.tiltAngle;
            float tiltX = z * playerController.activeWeapon.tiltAngle;

            newRotation.eulerAngles = new Vector3(tiltX, 0, tiltZ);
            wHolder.transform.localRotation = Quaternion.Slerp(wHolder.transform.localRotation, newRotation, playerController.activeWeapon.smoothRotation * Time.fixedDeltaTime);
        }
    }
}
