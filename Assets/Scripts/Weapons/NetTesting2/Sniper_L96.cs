using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sniper_L96 : WeaponBase
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
    //[SerializeField] private GameObject hitWallEffect;
    //[SerializeField] private GameObject bulletPrefab;

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
        //CreateHitParticle();
        CreateScope();

        if (fpWeaponInfo != null)
        {
            defaultShootPointEuler = fpWeaponInfo.shootPoint.localEulerAngles;
            defShootRot = fpWeaponInfo.shootPoint.rotation;
            newShootPointEuler = defaultShootPointEuler;
        }

        base.Start();
    }

    protected override void OnPutDown()
    {
        if(scopeUIInstance != null)
            scopeUIInstance.SetActive(false);

        if (playerEntity.isOwner)
        {
            GameUI.instance.ToggleCrosshair(true);

            if (CameraController.instance.myCamera.fieldOfView != playerController.defaultPlayerFOV)
                CameraController.instance.myCamera.fieldOfView = playerController.defaultPlayerFOV;
        }
    }

    private void CreateScope()
    {
        scopeUIInstance = Instantiate(scopeUI) as GameObject;
        scopeUIInstance.transform.SetParent(transform);
        scopeUIInstance.SetActive(false);
    }

    protected override void CreateBullet()
    {
        pooledProjectileBullets = new List<GameObject>();

        for(int i = 0; i < defaultAmmo; i++)
        {
            GameObject bullet = Instantiate(projectileBullet, transform.position, transform.rotation) as GameObject;
            bullet.SetActive(false);
            pooledProjectileBullets.Add(bullet);
        }

        #region
            /*bulletTrailInstance = Instantiate(bulletLineTrail, fpWeaponInfo.shootPoint.position, fpWeaponInfo.shootPoint.rotation) as GameObject;
            bulletTrailInstance.SetActive(false);

            hitWallEffectInstance = Instantiate(hitWallEffect);
            hitWallEffectInstance.SetActive(false);*/
            #endregion

        #region
            /*bulletObjectInstance = Instantiate(bulletObject, CameraController.instance.myCamera.transform.position, CameraController.instance.myCamera.transform.rotation) as GameObject;
            bulletRigidBody = bulletObjectInstance.GetComponent<Rigidbody>();
            bulletPhys = bulletObjectInstance.GetComponent<BulletPhysics>();
            bulletObjectInstance.SetActive(false);

            bulletObjectInstance.GetComponent<Rigidbody>().AddForce(CameraController.instance.myCamera.transform.forward * bulletPower, ForceMode.Impulse);*/
            #endregion
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

    /*protected override void PlayBullet()
    {
        RaycastHit bulletHit;

        if (bulletTrailInstance.activeSelf)
            bulletTrailInstance.SetActive(false);

        bulletTrailInstance.SetActive(true);

        if (playerController.velocityMag > 0)         //Player is moving
        {
            if (ironSightFlag)
                bulletSpreadFactor = defBulletSpreadFactor - 0.25f;
            else
                bulletSpreadFactor = defBulletSpreadFactor;
        }
        else if (playerController.velocityMag <= 0)   //Player is not moving
        {
            if (ironSightFlag)
                bulletSpreadFactor = 0;
            else
                bulletSpreadFactor = defBulletSpreadFactor;
        }

        if (Physics.Raycast(CameraController.instance.myShootPoint.position, (CameraController.instance.myShootPoint.forward + Random.insideUnitSphere * bulletSpreadFactor).normalized, out bulletHit, 1000f, hitLayerMask))
        {
            bulletTrailInstance.GetComponent<LineRenderer>().SetPosition(0, fpWeaponInfo.shootPoint.position);
            bulletTrailInstance.GetComponent<LineRenderer>().SetPosition(1, bulletHit.point);

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
                            Vector3 hitBloodPos = bulletHit.point;

                            DamageEvent dmgEvent = DamageEvent.Create();
                            dmgEvent.KillerID = playerEntity;
                            dmgEvent.VictimID = victimEntity;
                            dmgEvent.Hitlocation = bulletHit.point;

                            BoltConsole.Write("HITLOC: " + dmgEvent.Hitlocation);

                            switch (pHitBox.hitBoxType)
                            {
                                case PlayerHitbox.hitBox.head:
                                    dmgEvent.Damage = headShotDamage;
                                    BoltConsole.Write("Bullet Hit HEAD!", Color.red);
                                    break;
                                case PlayerHitbox.hitBox.chest:
                                    dmgEvent.Damage = bodyDamage;
                                    BoltConsole.Write("Bullet Hit CHEST!", Color.white);
                                    break;
                                case PlayerHitbox.hitBox.leg:
                                    dmgEvent.Damage = legDamage;
                                    BoltConsole.Write("Bullet Hit LEGS!", Color.green);
                                    break;
                                case PlayerHitbox.hitBox.hand:
                                    dmgEvent.Damage = handDamage;
                                    BoltConsole.Write("Bullet Hit HANDS!", Color.yellow);
                                    break;
                            }

                            dmgEvent.Send();
                        }
                    }
                }
            }
            else
            {
                if (hitWallEffectInstance.activeSelf)
                    hitWallEffectInstance.SetActive(false);

                hitWallEffectInstance.SetActive(true);

                hitWallEffectInstance.transform.position = bulletHit.point;
                hitWallEffectInstance.transform.rotation = Quaternion.FromToRotation(Vector3.forward, bulletHit.normal);
            }
        }

        #region
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
        #endregion
    }*/

    public override void PrimaryFire()
    {
        //Debug.Log("L96 Primary Fire Called!");

        if (playerEntity.isOwner)
        {
            if (!WeaponIsSelecting() &&
                !WeaponIsReloading())
            {
                //PlayBullet();
                currentAmmo--;
                PlayAnim(firstPersonAnims[fireAnimIndex].anim.name, firstPersonAnims[fireAnimIndex].animSpeed * fireAnimSpeedMultiplier, false);

                playerState.Firing();

                //WeaponManager3 wManager = playerEntity.GetComponent<WeaponManager3>();

                #region
                /*using (var hits = BoltNetwork.RaycastAll(new Ray(CameraController.instance.myCamera.transform.position, CameraController.instance.myCamera.transform.forward)))
                {
                    for (int i = 0; i < hits.count; ++i)
                    {
                        var hit = hits.GetHit(i);

                        var serializer = hit.body.GetComponent<PlayerHealth>();

                        if(serializer != null && serializer != playerEntity.GetComponent<PlayerHealth>())
                        {
                            var dmgEvent = DamageEvent.Create();
                            dmgEvent.PlayerID = serializer.entity;

                            switch(hit.hitbox.hitboxType)
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

    public override void TPFire()
    {
        GameObject bullet = GetPooledBulletProjectile();

        if(bullet)
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

    /*private void CreateHitParticle()
    {
        sandHitObjectInstance = (GameObject)Instantiate(sandHitEffect);
        sandHitObjectInstance.SetActive(false);

        bloodHitObjectInstance = (GameObject)Instantiate(bloodHitEffect);
        bloodHitObjectInstance.SetActive(false);

        if (bulletPhys != null)
        {
            bulletPhys.SetHitParticle(sandHitObjectInstance);
            bulletPhys.SetBloodHitParticle(bloodHitObjectInstance);
        }
    }*/

    /*public override void BulletFXTest()
    {
        GameObject bulletFX = Instantiate(bulletObject, fpWeaponInfo.shootPoint.position, fpWeaponInfo.shootPoint.rotation) as GameObject;
        bulletFX.SetActive(true);
        bulletFX.GetComponent<Rigidbody>().AddForce(bulletFX.transform.forward * bulletPower, ForceMode.Impulse);
        bulletFX.GetComponent<BulletPhysics>().CheckForHitBox(playerEntity, wManager.activeWeapon);
        bulletFX.GetComponent<BulletPhysics>().SetHitParticle(sandHitObjectInstance);
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
        if (!WeaponIsFiring())
        {
            playerState.IsReloading();

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
