using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper_M40A5 : WeaponBase
{
    [Header("Zoom Settings")]
    [SerializeField] private float scopeDelayTime = 0.18f;
    [SerializeField] private float zoomSpeed = 6f;
    [SerializeField] private GameObject scopeUI;

    [Header("Projectile Settings")]
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private GameObject projectileBullet;
    [SerializeField] private List<GameObject> pooledProjectileBullets;
    [SerializeField] private float bulletPower = 100f;
    
    private GameObject scopeUIInstance;
    private float zoomV;
    private Vector3 defaultShootPointEuler;
    private Vector3 newShootPointEuler;
    private Quaternion defShootRot;

    void Awake()
    {
        CreateBullet();
    }

    protected override void Start()
    {
        CreateScope();

        if (fpWeaponInfo != null)
        {
            defaultShootPointEuler = fpWeaponInfo.shootPoint.localEulerAngles;
            defShootRot = fpWeaponInfo.shootPoint.rotation;
            newShootPointEuler = defaultShootPointEuler;
        }

        base.Start();
    }

    protected override void CreateBullet()
    {
        pooledProjectileBullets = new List<GameObject>();

        for (int i = 0; i < defaultAmmo; i++)
        {
            GameObject bullet = Instantiate(projectileBullet, transform.position, transform.rotation) as GameObject;
            bullet.SetActive(false);
            pooledProjectileBullets.Add(bullet);
        }
    }

    private GameObject GetPooledBulletProjectile()
    {
        for (int i = 0; i < pooledProjectileBullets.Count; i++)
        {
            if (!pooledProjectileBullets[i].activeSelf)
                return pooledProjectileBullets[i];
        }

        return null;
    }

    private void CreateScope()
    {
        scopeUIInstance = Instantiate(scopeUI) as GameObject;
        scopeUIInstance.transform.SetParent(transform);
        scopeUIInstance.SetActive(false);
    }

    protected override void Update()
    {
        if (playerEntity.isOwner)
        {
            playerState.ShootPos = fpWeaponInfo.shootPoint.position;
            playerState.ShootRot = fpWeaponInfo.shootPoint.rotation;
        }

        if (playerController.velocityMag > 0.1f)         //Player is Moving
        {
            if (!ironSightFlag)
                bulletSpreadFactor = defBulletSpreadFactor + 0.05f;
            else if (ironSightFlag)
                bulletSpreadFactor = defBulletSpreadFactor - 0.05f;
        }
        else if (playerController.velocityMag <= 0)      //Player is Standing
        {
            if (!ironSightFlag)
                bulletSpreadFactor = defBulletSpreadFactor;
            else if (ironSightFlag)
                bulletSpreadFactor = 0;
        }

        base.Update();
    }

    protected override void OnPutDown()
    {
        scopeUIInstance.SetActive(false);

        if (playerEntity.isOwner)
        {
            GameUI.instance.ToggleCrosshair(true);

            if (CameraController.instance.myCamera.fieldOfView != playerController.defaultPlayerFOV)
                CameraController.instance.myCamera.fieldOfView = playerController.defaultPlayerFOV;
        }
    }

    public override void PrimaryFire()
    {
        //Debug.Log("L96 Primary Fire Called!");

        if (playerEntity.isOwner)
        {
            if (!WeaponIsSelecting() &&
                !WeaponIsReloading())
            {
                playerState.Firing();
                currentAmmo--;
                PlayAnim(firstPersonAnims[fireAnimIndex].anim.name, firstPersonAnims[fireAnimIndex].animSpeed * fireAnimSpeedMultiplier, false);
                #region
                /*using (var hits = BoltNetwork.RaycastAll(new Ray(CameraController.instance.myCamera.transform.position, CameraController.instance.myCamera.transform.forward)))
                {
                    for (int i = 0; i < hits.count; ++i)
                    {
                        var hit = hits.GetHit(i);
                        var serializer = hit.body.GetComponent<PlayerHealth>();

                        if (serializer != null && serializer != playerEntity.GetComponent<PlayerHealth>())
                        {
                            var dmgEvent = DamageEvent.Create();
                            dmgEvent.PlayerID = serializer.entity;
                            switch (hit.hitbox.hitboxType)
                            {
                                case BoltHitboxType.Body:
                                    dmgEvent.Damage = wManager.activeWeapon.bodyDamage;
                                    break;
                                case BoltHitboxType.Head:
                                    dmgEvent.Damage = wManager.activeWeapon.headShotDamage;
                                    break;
                                case BoltHitboxType.Hand:
                                case BoltHitboxType.UpperArm:
                                    dmgEvent.Damage = wManager.activeWeapon.handDamage;
                                    break;
                                case BoltHitboxType.Thigh:
                                case BoltHitboxType.Leg:
                                    dmgEvent.Damage = wManager.activeWeapon.legDamage;
                                    break;
                            }
                            dmgEvent.Send();

                            //serializer.TakeDamage(wManager.activeWeapon.bodyDamage);
                        }
                    }
                }*/
                #endregion

                StartCoroutine(MuzzleFlashTimerFP(muzzleFlashTimer));
            }
        }
    }

    public override void TPFire()
    {
        GameObject bullet = GetPooledBulletProjectile();

        if (bullet)
        {
            bullet.SetActive(true);

            if (playerEntity.isOwner)
            {
                Vector3 fireDir = (CameraController.instance.myShootPoint.forward + Random.insideUnitSphere * bulletSpreadFactor).normalized;
                fpWeaponInfo.shootPoint.forward = fireDir;
            }

            bullet.transform.position = playerState.ShootPos;
            bullet.GetComponent<BulletPhysicTest>().oldPos = Vector3.zero;
            bullet.GetComponent<BulletPhysicTest>().newPos = Vector3.zero;
            bullet.GetComponent<BulletPhysicTest>().oldPos = playerState.ShootPos;
            bullet.GetComponent<BulletPhysicTest>().newPos = playerState.ShootPos;
            bullet.transform.rotation = playerState.ShootRot;
            bullet.GetComponent<BulletPhysicTest>().SetupBullet(playerEntity, playerController.activeWeapon, hitLayerMask, bulletPower);
        }
    }

    /*protected override void PlayBullet()
    {
        bulletObjectInstance.SetActive(false);

        if (ironSightFlag)
        {
            bulletObjectInstance.transform.position = CameraController.instance.myShootPoint.position;
            bulletObjectInstance.transform.rotation = CameraController.instance.myCamera.transform.rotation;
        }
        else
        {
            newShootPointEuler = fpWeaponInfo.shootPoint.localEulerAngles;
            newShootPointEuler.x += Random.Range(-5f, +5f);
            newShootPointEuler.y += Random.Range(-3f, +3f);
            //euler.y = Random.Range(0, 5f);

            fpWeaponInfo.shootPoint.localEulerAngles = newShootPointEuler;

            bulletObjectInstance.transform.position = fpWeaponInfo.shootPoint.position;
            bulletObjectInstance.transform.rotation = fpWeaponInfo.shootPoint.rotation;

            fpWeaponInfo.shootPoint.localEulerAngles = defaultShootPointEuler;
        }

        bulletObjectInstance.SetActive(true);
        bulletRigidBody.AddForce(bulletObjectInstance.transform.forward * bulletPower, ForceMode.Impulse);
        bulletRigidBody.velocity = new Vector3(bulletRigidBody.velocity.x, bulletRigidBody.velocity.y + 2f, bulletRigidBody.velocity.z);
        bulletPhys.CheckForHitBox(playerEntity, wManager.activeWeapon);
    }*/

    public override void AltFire()
    {
        if (playerEntity.isOwner)
        {
            if (weaponType == weaponTypeClass.weapon_SniperRifle)
            {
                ironSightFlag = !ironSightFlag;

                if (ironSightFlag)
                {
                    fpWeaponInfo.shootPoint.localEulerAngles = defaultShootPointEuler;
                    StartCoroutine(OnScoped(scopeDelayTime));
                }
                else
                {
                    fpWeaponInfo.EnableSkinMeshRenderers();
                    scopeUIInstance.SetActive(false);

                    if (playerEntity.isOwner)
                        GameUI.instance.ToggleCrosshair(true);
                }
            }
        }
    }

    public override void Reload()
    {
        playerState.IsReloading();

        if (!WeaponIsFiring())
        {
            ironSightFlag = false;
            fpWeaponInfo.EnableSkinMeshRenderers();
            scopeUIInstance.SetActive(false);

            if (playerEntity.isOwner)
                GameUI.instance.ToggleCrosshair(true);

            if (currentAmmo <= 0)
            {
                PlayAnim(firstPersonAnims[reloadFullAnimIndex].anim.name, firstPersonAnims[reloadFullAnimIndex].animSpeed * reloadFullAnimSpeedMultiplier, false);
                StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadFullAnimIndex].anim.length / 2f));
            }
            else if (currentAmmo > 0)
            {
                PlayAnim(firstPersonAnims[reloadNormalAnimIndex].anim.name, firstPersonAnims[reloadNormalAnimIndex].animSpeed * reloadNormalAnimSpeedMultiplier, false);
                StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadNormalAnimIndex].anim.length / 2f));
            }
        }
    }

    private void FixedUpdate()
    {
        if (fpWeaponInstance != null)
        {
            if (weaponType == weaponTypeClass.weapon_SniperRifle)
            {
                if (ironSightFlag)
                {
                    fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, ironSightOffset, ironSightSpeed * Time.fixedDeltaTime);

                    if (playerEntity.isOwner)
                        CameraController.instance.myCamera.fieldOfView = Mathf.SmoothDamp(CameraController.instance.myCamera.fieldOfView, zoomFOV, ref zoomV, zoomSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, defaultWeaponOffset, ironSightSpeed * Time.fixedDeltaTime);

                    if (playerEntity.isOwner)
                        CameraController.instance.myCamera.fieldOfView = Mathf.SmoothDamp(CameraController.instance.myCamera.fieldOfView, playerController.defaultPlayerFOV, ref zoomV, zoomSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }

    #region
    /*protected override void Update()
    {
        if (weaponType == weaponTypeClass.weapon_SniperRifle)
        {
            if (ironSightFlag)
                fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, ironSightOffset, ironSightSpeed * Time.deltaTime);
            else
                fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, defaultWeaponOffset, ironSightSpeed * Time.deltaTime);
        }

        base.Update();
    }

    IEnumerator OnScoped(float time)
    {
        yield return new WaitForSeconds(time);

        fpWeaponInfo.DisableSkinMeshRenderers();

        if (playerEntity.isOwner)
            CameraController.instance.myCamera.fieldOfView = zoomFOV;

        scopeUIInstance.SetActive(true);
    }*/
    #endregion

    IEnumerator OnScoped(float time)
    {
        yield return new WaitForSeconds(time);

        fpWeaponInfo.DisableSkinMeshRenderers();
        scopeUIInstance.SetActive(true);

        if(playerEntity.isOwner)
            GameUI.instance.ToggleCrosshair(false);

        //if (entity.isOwner)
        //CameraController.instance.myCamera.fieldOfView = zoomFOV;
    }
}
