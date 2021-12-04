using UnityEngine;

[RequireComponent(typeof(WeaponManager3))]
public class PlayerShoot : Bolt.EntityBehaviour<ISniperPlayerState>
{
    private PlayerController playerController;

    void Start()
    {
        if (entity.isOwner)
            playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (PauseMenu.isOn)
            return;

        if (state.IsDead)
            return;

        if(entity.isOwner)
        {
            if (CameraController.instance.myWeaponHolder.gameObject.activeSelf)
            {
                if (playerController.activeWeapon.weaponType == WeaponBase.weaponTypeClass.weapon_Rifle)
                {
                    if ((playerController.activeWeapon.currentAmmo > 0 && playerController.activeWeapon.currentMaxAmmo >= 0) &&
                        !playerController.activeWeapon.WeaponIsReloading() &&
                        !playerController.activeWeapon.WeaponIsSelecting())
                    {
                        if (playerController.activeWeapon.fireTimer < playerController.activeWeapon.fireRate)
                            playerController.activeWeapon.fireTimer += Time.deltaTime;

                        if (Input.GetButton("Fire1"))
                            playerController.activeWeapon.PrimaryFire();

                        /*if (Input.GetButtonDown("Fire1"))
                        {
                            playerController.activeWeapon.InvokeRepeating("PrimaryFire", 0f, 1f / playerController.activeWeapon.fireRate);
                        }
                        else if (Input.GetButtonUp("Fire1") || (playerController.activeWeapon.currentAmmo <= 0 && playerController.activeWeapon.currentMaxAmmo <= 0))
                            playerController.activeWeapon.CancelInvoke("PrimaryFire");*/
                    }
                }
                else if (playerController.activeWeapon.weaponType == WeaponBase.weaponTypeClass.weapon_SniperRifle)
                {
                    if ((playerController.activeWeapon.currentAmmo > 0 && playerController.activeWeapon.currentMaxAmmo > 0) &&
                            !playerController.activeWeapon.WeaponIsFiring() &&
                            !playerController.activeWeapon.WeaponIsReloading() &&
                            !playerController.activeWeapon.WeaponIsSelecting())
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            playerController.activeWeapon.PrimaryFire();
                            playerController.activeWeapon.recoil += 0.1f;
                        }
                    }
                }
                else if (playerController.activeWeapon.weaponType == WeaponBase.weaponTypeClass.weapon_SemiAuto)
                {
                    if ((playerController.activeWeapon.currentAmmo > 0) &&
                            !playerController.activeWeapon.WeaponIsReloading() &&
                            !playerController.activeWeapon.WeaponIsSelecting())
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            playerController.activeWeapon.PrimaryFire();
                            playerController.activeWeapon.recoil += 0.1f;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (/*(playerController.activeWeapon.currentAmmo > 0 && playerController.activeWeapon.currentMaxAmmo >= 0) &&*/
                        !playerController.activeWeapon.WeaponIsReloading() &&
                        !playerController.activeWeapon.WeaponIsSelecting())
                    {
                        playerController.activeWeapon.AltFire();
                    }
                }

                if (Input.GetKeyDown(KeyCode.R) && 
                    (playerController.activeWeapon.currentMaxAmmo > 0 && playerController.activeWeapon.currentAmmo < playerController.activeWeapon.defaultAmmo) && 
                    !playerController.activeWeapon.WeaponIsFiring() && 
                    !playerController.activeWeapon.WeaponIsReloading())
                {
                    playerController.activeWeapon.Reload();
                }

                playerController.activeWeapon.Recoiling();
            }
        }
    }
}

/*[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : Bolt.EntityBehaviour<ISniperPlayerState>
{
    private WeaponManager wManager;
    private WeaponObject currentWeaponObject;
    private GameObject currentFPWeapon;

    //[HideInInspector] public Animator fpWeaponAnim;

    void Start()
    {
        if (entity.isOwner)
        {
            wManager = GetComponent<WeaponManager>();
        }
        //fpWeaponAnim = wManager.fpWeapon.GetComponentInChildren<Animator>();
    }

    void Update ()
    {
        if (entity.isOwner && entity.hasControl)
        {
            currentWeaponObject = wManager.activeWeaponObject;
            currentFPWeapon = wManager.activeFPWeaponObject;

            if (!AnimatorIsPlaying(currentWeaponObject.reloadAnimationName) || !AnimatorIsPlaying(currentWeaponObject.boltAnimationName))
            {
                if (currentWeaponObject.fireRate <= 0f)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        state.Firing();
                        Shoot();
                    }
                }
            }

            //fpWeaponAnim.SetTrigger(wManager.activeWeaponObject.FiringParameter);

            if (Input.GetKeyDown(KeyCode.R) && (!AnimatorIsPlaying(currentWeaponObject.fireAnimationName) || !AnimatorIsPlaying(currentWeaponObject.boltAnimationName)))
                Reload();

            if (AnimatorIsPlaying(currentWeaponObject.fireAnimationName))
                Debug.Log("Current Playing State Anim: " + currentWeaponObject.fireAnimationName);

            //fpWeaponAnim.SetTrigger(wManager.activeWeaponObject.ReloadingParameter);
        }
    }

    void Shoot()
    {
        currentFPWeapon.GetComponentInChildren<Animator>().SetTrigger(currentWeaponObject.firingParameter);
    }

    void Reload()
    {
        currentFPWeapon.GetComponentInChildren<Animator>().SetTrigger(currentWeaponObject.reloadingParameter);
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return currentFPWeapon.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}*/
