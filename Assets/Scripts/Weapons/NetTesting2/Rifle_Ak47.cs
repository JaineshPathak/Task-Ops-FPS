using System.Collections.Generic;
using UnityEngine;

public class Rifle_Ak47 : WeaponBase
{
    [Header("Custom Animation Settings")]
    [SerializeField] private Vector3 aimScaleOffset;
    public byte fireAimAnimIndex;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 6f;
    [SerializeField] private float aimScaleSpeed = 2f;

    [Header("Projectile Settings")]
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private float bulletPower = 100f;
    [SerializeField] private GameObject bulletHitWallEffect;
    [SerializeField] private List<GameObject> pooledBulletsHitWallEffects;

    [SerializeField] private GameObject fakeBulletObject;
    [SerializeField] private List<GameObject> pooledFakeBullets;
    [SerializeField] private Vector2 recoilXY;
    [SerializeField] private float recoilAdder = -0.5f;

    private float zoomV;
    private Vector3 aimScaleV;

    private Vector3 defaultShootPointEuler;
    private Vector3 newShootPointEuler;
    private Vector3 defaultScaleOffset;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (fpWeaponInfo != null)
        {
            defaultShootPointEuler = fpWeaponInfo.shootPoint.localEulerAngles;
            newShootPointEuler = defaultShootPointEuler;

            defaultScaleOffset = fpWeaponInfo.transform.localScale;
        }

        CreateBullet();
    }

    protected override void CreateBullet()
    {
        pooledBulletsHitWallEffects = new List<GameObject>();

        for(int i = 0; i < defaultAmmo; i++)
        {
            GameObject obj = Instantiate(bulletHitWallEffect, CameraController.instance.myCamera.transform.position, CameraController.instance.myCamera.transform.rotation) as GameObject;
            obj.SetActive(false);
            pooledBulletsHitWallEffects.Add(obj);

            GameObject traceObj = Instantiate(fakeBulletObject, fpWeaponInfo.shootPoint.transform.position, fpWeaponInfo.shootPoint.transform.rotation) as GameObject;
            traceObj.SetActive(false);
            pooledFakeBullets.Add(traceObj);
        }

        //bulletTraceInstance = Instantiate(bulletTraceEffects, fpWeaponInfo.shootPoint.transform.position, fpWeaponInfo.shootPoint.transform.rotation) as GameObject;
        //bulletTraceInstance.transform.SetParent(fpWeaponInfo.shootPoint.transform);
        //bulletTraceInstance.SetActive(false);
    }

    private GameObject GetPooledBulletHitWallEffect()
    {
        for(int i = 0; i < pooledBulletsHitWallEffects.Count; i++)
        {
            if (!pooledBulletsHitWallEffects[i].activeSelf)
                return pooledBulletsHitWallEffects[i];
        }

        return null;
    }

    private GameObject GetPooledFakeBulletTraceEffect()
    {
        for (int i = 0; i < pooledFakeBullets.Count; i++)
        {
            if (!pooledFakeBullets[i].activeSelf)
                return pooledFakeBullets[i];
        }

        return null;
    }

    protected override void PlayBullet()
    {
        RaycastHit bulletHit;

        if (playerEntity.isOwner)
        {
            GameObject bulletTrace = GetPooledFakeBulletTraceEffect();
            bulletTrace.SetActive(true);
            bulletTrace.transform.position = fpWeaponInfo.shootPointForward.position;
            bulletTrace.transform.rotation = fpWeaponInfo.shootPointForward.rotation;
            //bulletTrace.GetComponent<Rigidbody>().AddForce(bulletTrace.transform.forward * bulletPower, ForceMode.Impulse);

            if (Physics.Raycast(CameraController.instance.myShootPoint.position, (CameraController.instance.myShootPoint.forward + Random.insideUnitSphere * bulletSpreadFactor).normalized, out bulletHit, 100f, hitLayerMask))
            {
                //If Player is shooting a wall, floor, etc. Adjust the rotation of bullet tracers according to the bullet hit point.
                Vector3 newDir = (bulletHit.point - fpWeaponInfo.shootPointForward.position).normalized;
                bulletTrace.transform.rotation = Quaternion.LookRotation(newDir);

                if (bulletHit.collider.isTrigger)
                {
                    BoltEntity victimEntity = bulletHit.collider.transform.root.GetComponent<BoltEntity>();

                    if (playerEntity != victimEntity)
                    {
                        if (!victimEntity.GetState<ISniperPlayerState>().IsDead)
                        {
                            PlayerHitbox pHitBox = bulletHit.collider.GetComponent<PlayerHitbox>();

                            if (pHitBox != null)
                            {
                                DamageEvent dmgEvent = DamageEvent.Create();
                                dmgEvent.KillerID = playerEntity;
                                dmgEvent.VictimID = victimEntity;
                                dmgEvent.Hitlocation = bulletHit.point;

                                BoltConsole.Write("HITLOC: " + dmgEvent.Hitlocation);

                                switch (pHitBox.hitBoxType)
                                {
                                    case PlayerHitbox.hitBox.head:
                                        dmgEvent.Damage = headShotDamage;
                                        //BoltConsole.Write("Bullet Hit HEAD!", Color.red);
                                        break;
                                    case PlayerHitbox.hitBox.chest:
                                        dmgEvent.Damage = bodyDamage;
                                        //BoltConsole.Write("Bullet Hit CHEST!", Color.white);
                                        break;
                                    case PlayerHitbox.hitBox.leg:
                                        dmgEvent.Damage = legDamage;
                                        //BoltConsole.Write("Bullet Hit LEGS!", Color.green);
                                        break;
                                    case PlayerHitbox.hitBox.hand:
                                        dmgEvent.Damage = handDamage;
                                        //BoltConsole.Write("Bullet Hit HANDS!", Color.yellow);
                                        break;
                                }

                                dmgEvent.Send();
                            }
                        }
                    }
                }
                else
                {
                    if (bulletHit.collider.tag != "Player")
                    {
                        GameObject bulletEffect = GetPooledBulletHitWallEffect();

                        bulletEffect.SetActive(true);
                        bulletEffect.transform.position = bulletHit.point;
                        bulletEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, bulletHit.normal);
                    }
                }
            }
            else
            {
                //If Player is shooting in the sky - randomize the rotation of bullet traces
                Vector3 newRot = fpWeaponInfo.shootPoint.eulerAngles;

                if (ironSightFlag)
                {
                    newRot.x += Random.Range(-2.5f / 2f, +2.5f / 2f);
                    newRot.y += Random.Range(-2.5f /2f, +2.5f / 2f);
                }
                else
                {
                    newRot.x += Random.Range(-2.5f, +2.5f);
                    newRot.y += Random.Range(-2.5f, +2.5f);
                }

                bulletTrace.transform.rotation = Quaternion.Euler(newRot);
            }
        }

        #region
        /*GameObject bullet = GetPooledBullet();

        if (bullet != null)
        {
            if (ironSightFlag)
            {
                bullet.transform.position = fpWeaponInfo.shootPoint.position;
                bullet.transform.rotation = fpWeaponInfo.shootPoint.rotation;
            }
            else
            {
                newShootPointEuler = fpWeaponInfo.shootPoint.localEulerAngles;
                newShootPointEuler.x += Random.Range(-5f, +5f);
                newShootPointEuler.y += Random.Range(-3f, +3f);

                fpWeaponInfo.shootPoint.localEulerAngles = newShootPointEuler;

                bullet.transform.position = fpWeaponInfo.shootPoint.position;
                bullet.transform.rotation = fpWeaponInfo.shootPoint.rotation;

                fpWeaponInfo.shootPoint.localEulerAngles = defaultShootPointEuler;
            }

            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletPower, ForceMode.Impulse);
            bullet.GetComponent<BulletPhysics>().CheckForHitBox(playerEntity, wManager.activeWeapon);
        }*/
        #endregion
    }

    public override bool WeaponIsFiring()
    {
        return AnimIsPlaying(firstPersonAnims[fireAnimIndex].anim.name) || AnimIsPlaying(firstPersonAnims[fireAimAnimIndex].anim.name);
    }

    public override void PrimaryFire()
    {
        //Debug.Log("L96 Primary Fire Called!");

        if (fireTimer < fireRate)
            return;

        if (!AnimIsPlaying(firstPersonAnims[selectAnimIndex].anim.name) &&
            !AnimIsPlaying(firstPersonAnims[reloadNormalAnimIndex].anim.name))
        {
            fireTimer = 0.0f;
            PlayBullet();
            recoil += 0.1f;
            currentAmmo--;

            if (ironSightFlag)
            {
                if (WeaponIsFiring())
                    anim.Stop(firstPersonAnims[fireAimAnimIndex].anim.name);

                PlayAnim(firstPersonAnims[fireAimAnimIndex].anim.name, firstPersonAnims[fireAimAnimIndex].animSpeed, false);
            }
            else
            {
                if (WeaponIsFiring())
                    anim.Stop(firstPersonAnims[fireAnimIndex].anim.name);

                PlayAnim(firstPersonAnims[fireAnimIndex].anim.name, firstPersonAnims[fireAnimIndex].animSpeed, false);
            }

            playerEntity.GetState<ISniperPlayerState>().Firing();

            //WeaponManager3 wManager = playerEntity.GetComponent<WeaponManager3>();

            StartCoroutine(MuzzleFlashTimerFP(muzzleFlashTimer));
        }
    }

    public override void AltFire()
    {
        if (playerEntity.isOwner)
        {
            ironSightFlag = !ironSightFlag;

            if (ironSightFlag)
                GameUI.instance.ToggleCrosshair(false);
            else
                GameUI.instance.ToggleCrosshair(true);

            if (ironSightFlag)
                bulletSpreadFactor /= 2f;
            else
                bulletSpreadFactor = defBulletSpreadFactor;
        }
    }

    public override void Reload()
    {
        if (playerEntity.isOwner)
            GameUI.instance.ToggleCrosshair(true);

        CancelInvoke("PrimaryFire");
        ironSightFlag = false;
        bulletSpreadFactor = defBulletSpreadFactor;

        playerEntity.GetState<ISniperPlayerState>().IsReloading();

        PlayAnim(firstPersonAnims[reloadNormalAnimIndex].anim.name, firstPersonAnims[reloadNormalAnimIndex].animSpeed * reloadNormalAnimSpeedMultiplier, false);
        StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadNormalAnimIndex].anim.length / 2f));
    }

    private void FixedUpdate()
    {
        if (fpWeaponInstance != null)
        {
            if (weaponType == weaponTypeClass.weapon_Rifle)
            {
                if (ironSightFlag)
                {
                    fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, ironSightOffset, ironSightSpeed * Time.fixedDeltaTime);

                    if (playerEntity.isOwner)
                        CameraController.instance.myCamera.fieldOfView = Mathf.SmoothDamp(CameraController.instance.myCamera.fieldOfView, zoomFOV, ref zoomV, zoomSpeed * Time.fixedDeltaTime);

                    fpWeaponInstance.transform.localScale = Vector3.SmoothDamp(fpWeaponInstance.transform.localScale, aimScaleOffset, ref aimScaleV, aimScaleSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    fpWeaponInstance.transform.localPosition = Vector3.MoveTowards(fpWeaponInstance.transform.localPosition, defaultWeaponOffset, ironSightSpeed * Time.fixedDeltaTime);

                    if (playerEntity.isOwner)
                        CameraController.instance.myCamera.fieldOfView = Mathf.SmoothDamp(CameraController.instance.myCamera.fieldOfView, playerController.defaultPlayerFOV, ref zoomV, zoomSpeed * Time.fixedDeltaTime);

                    fpWeaponInstance.transform.localScale = Vector3.SmoothDamp(fpWeaponInstance.transform.localScale, defaultScaleOffset, ref aimScaleV, aimScaleSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}
