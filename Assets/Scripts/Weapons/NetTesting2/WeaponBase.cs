using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBase : MonoBehaviour
{
    [System.Serializable]
    public class Anims
    {
        public AnimationClip anim;
        public float animSpeed;
    }

    public enum weaponTypeClass
    {
        weapon_Melee,
        weapon_Pistol,
        weapon_SMG,
        weapon_Rifle,
        weapon_SniperRifle,
        weapon_SemiAuto
    };

    [Header("Details")]
    public weaponTypeClass weaponType;
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject fpWeaponPrefab;
    protected GameObject fpWeaponInstance;
    public GameObject tpWeaponPrefab;
    private GameObject tpWeaponInstance;
    protected WeaponInfo fpWeaponInfo;
    [HideInInspector] public WeaponInfo tpWeaponInfo;
    [HideInInspector] public Transform aimingTransform;

    [Header("First Person Bob Settings")]
    public float runSideBobSpeed = 0.2f;
    public float runSideBobAmount = 0.07f;
    public float crouchSideBobSpeed = 0.05f;
    public float crouchSideBobAmount = 0.03f;
    public float aimSideBobSpeed = 0.1f;
    public float aimSideBobAmount = 0.005f;
    public Vector3 runVectRotation;

    [Header("First Person Sway Settings")]
    public float swayAmount = 1.01f;
    public float swaySmoothness = 1f;
    public float maxSwayAmount = 0.02f;
    [Space(10)]
    public float smoothRotation = 1f;
    public float tiltAngle = 20f;

    [Header("Animation Settings")]
    public Anims[] firstPersonAnims;         //0 - Select, 1 - Still, 2 - Fire, 3 - Reload Normal, 4 - Reload Full
    public byte selectAnimIndex = 0;
    public byte stillAnimIndex = 1;
    public byte fireAnimIndex = 2;
    public byte reloadNormalAnimIndex = 3;
    public byte reloadFullAnimIndex = 4;
    public bool bHasFullReloadAnim = false;
    /*public AnimationClip selectAnim;
    public float selectAnimSpeed;

    public AnimationClip reloadNormalAnim;
    public float reloadNormalAnimSpeed;

    public AnimationClip reloadFullAnim;
    public float reloadFullAnimSpeed;

    public AnimationClip fireAnim;
    public float fireAnimSpeed;

    public AnimationClip stillAnim;
    public float stillAnimSpeed;*/

    [Header("Animation Speed")]
    public float selectAnimSpeedMultiplier = 1f;
    public float stillAnimSpeedMultiplier = 1f;
    public float fireAnimSpeedMultiplier = 1.2f;
    public float reloadNormalAnimSpeedMultiplier = 1f;
    public float reloadFullAnimSpeedMultiplier = 1f;

    [Header("Firing Settings")]
    public float bulletSpreadFactor = 0.05f;
    public float fireTimer;
    public float fireRate;
    public int bodyDamage;
    public int headShotDamage;
    public int handDamage;
    public int legDamage;
    public float muzzleFlashTimer = 0.02f;
    [HideInInspector] public Transform muzzlePoint_TP;
    [HideInInspector] public Transform muzzlePoint_FP;

    [Header("Ammo Settings")]
    public int defaultAmmo;
    public int currentAmmo;
    public int maxAmmo;
    public int currentMaxAmmo;

    [Header("Recoil Settings")]
    [HideInInspector] public float recoil = 0;
    public Vector2 maxRecoilXY = new Vector2(-20f, 20f);
    public float recoilSpeed = 10f;

    [Header("Iron Sight Settings")]
    protected Vector3 defaultWeaponOffset;
    [SerializeField] protected Vector3 ironSightOffset;
    public float ironSightSpeed = 1f;
    public bool ironSightFlag = false;
    public float zoomFOV = 15f;

    protected Animation anim;
    protected PlayerController playerController;
    protected BoltEntity playerEntity;
    protected ISniperPlayerState playerState;
    protected Transform weaponBackPos;
    protected Transform weaponHand;

    protected float defBulletSpreadFactor;

    protected virtual void Start()
    {
        if (fpWeaponInstance != null)
        {
            defaultWeaponOffset = fpWeaponInstance.transform.localPosition;
            ironSightFlag = false;
        }

        defBulletSpreadFactor = bulletSpreadFactor;
    }

    public void SetupWeapon(BoltEntity entity, ISniperPlayerState state, PlayerController _playerController, Transform _weaponHand, Transform _weaponBackPos)
    {
        if (entity != null)
        {
            playerEntity = entity;
            playerState = state;
        }

        playerController = _playerController;

        weaponHand = _weaponHand;
        weaponBackPos = _weaponBackPos;

        ironSightFlag = false;

        ResetAmmo();

        SetupThirdPersonWeapon(weaponHand);
    }

    public void ResetAmmo()
    {
        currentAmmo = defaultAmmo;
        currentMaxAmmo = maxAmmo;
    }

    public virtual bool WeaponIsReloading()
    {
        return AnimIsPlaying(firstPersonAnims[reloadNormalAnimIndex].anim.name) || AnimIsPlaying(firstPersonAnims[reloadFullAnimIndex].anim.name);
    }

    public virtual bool WeaponIsFiring()
    {
        return AnimIsPlaying(firstPersonAnims[fireAnimIndex].anim.name);
    }

    public virtual bool WeaponIsSelecting()
    {
        return AnimIsPlaying(firstPersonAnims[selectAnimIndex].anim.name);
    }

    public void PlayAnim(string animName, float animSpeed, bool bCrossfade)
    {
        if (fpWeaponInstance != null)
        {
            if (bCrossfade)
                anim.CrossFade(animName);
            else
                anim.Play(animName, PlayMode.StopAll);

            anim[animName].speed = animSpeed;
        }
    }

    public bool AnimIsPlaying(string animName)
    {
        if (fpWeaponInstance != null)
        {
            if (anim.IsPlaying(animName))
                return true;
        }

        return false;
    }

    void OnEnable()
    {
        if (fpWeaponInstance != null)
        {
            fpWeaponInstance.SetActive(true);
            
            if(weaponType == weaponTypeClass.weapon_SniperRifle)
                fpWeaponInfo.EnableSkinMeshRenderers();
            //PlayAnim(selectAnim.name, selectAnimSpeed, false);
            PlayAnim(firstPersonAnims[selectAnimIndex].anim.name, firstPersonAnims[selectAnimIndex].animSpeed * selectAnimSpeedMultiplier, false);
        }

        if (tpWeaponInstance != null)
        {
            tpWeaponInstance.transform.parent = weaponHand;
            tpWeaponInstance.transform.position = weaponHand.position;
            tpWeaponInstance.transform.rotation = weaponHand.rotation;
            tpWeaponInstance.transform.localScale = tpWeaponPrefab.transform.localScale;
            //tpWeaponInstance.SetActive(true);
        }

        OnPutUp();
    }

    protected virtual void OnPutUp()
    {
    }

    void OnDisable()
    {
        if (fpWeaponInstance != null)
        {
            ironSightFlag = false;
            fpWeaponInstance.SetActive(false);
        }

        if (tpWeaponInstance != null)
        {
            tpWeaponInstance.transform.parent = weaponBackPos;
            tpWeaponInstance.transform.position = weaponBackPos.position;
            tpWeaponInstance.transform.rotation = weaponBackPos.rotation;
            tpWeaponInstance.transform.localScale = tpWeaponInfo.weaponBackScale;
            //tpWeaponInstance.SetActive(false);
        }

        OnPutDown();
    }

    protected virtual void OnPutDown()
    {
    }

    public void SetupFirstPersonWeapon(Transform weaponHolder)
    {
        fpWeaponInstance = Instantiate(fpWeaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder) as GameObject;

        if (fpWeaponInstance != null)
        {
            fpWeaponInfo = fpWeaponInstance.GetComponent<WeaponInfo>();
            anim = fpWeaponInstance.GetComponentInChildren<Animation>();

            muzzlePoint_FP = fpWeaponInfo.fpMuzzleFlash;

            if (muzzlePoint_FP != null)
                muzzlePoint_FP.gameObject.SetActive(false);

            fpWeaponInstance.SetActive(false);
        }
    }

    public void SetupThirdPersonWeapon(Transform weaponHand)
    {
        tpWeaponInstance = Instantiate(tpWeaponPrefab, weaponHand.position, weaponHand.rotation, weaponHand) as GameObject;
        tpWeaponInstance.transform.localPosition = tpWeaponPrefab.transform.localPosition;
        tpWeaponInstance.transform.localScale = tpWeaponPrefab.transform.localScale;

        if (tpWeaponInstance != null)
        {
            tpWeaponInfo = tpWeaponInstance.GetComponent<WeaponInfo>();
            aimingTransform = tpWeaponInstance.transform.Find("AimingTransform");

            muzzlePoint_TP = tpWeaponInfo.tpMuzzleFlash;

            if (muzzlePoint_TP != null)
                muzzlePoint_TP.gameObject.SetActive(false);

            //tpWeaponInstance.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        //Play Still Anim
        if (!WeaponIsSelecting() &&
            !WeaponIsFiring() &&
            !WeaponIsReloading())
        {
            PlayAnim(firstPersonAnims[stillAnimIndex].anim.name, firstPersonAnims[stillAnimIndex].animSpeed * stillAnimSpeedMultiplier, false);
        }

        //---------------------------------------------------------------
        if (currentAmmo > defaultAmmo)
            currentAmmo = defaultAmmo;

        if (currentMaxAmmo > maxAmmo)
            currentMaxAmmo = maxAmmo;

        if (currentAmmo < 0)
            currentAmmo = 0;

        if (currentMaxAmmo < 0)
            currentMaxAmmo = 0;
        //---------------------------------------------------------------

        if (currentAmmo <= 0 && currentMaxAmmo > 0 &&
            !WeaponIsFiring() && 
            !WeaponIsSelecting() &&
            !WeaponIsReloading())
            Reload();
    }

    public virtual void PrimaryFire()
    {
        #region
        //PlayAnim(fireAnim.name, fireAnimSpeed, false);

        /*if (!AnimIsPlaying(firstPersonAnims[selectAnimIndex].anim.name) && 
            !AnimIsPlaying(firstPersonAnims[reloadNormalAnimIndex].anim.name) &&
            !AnimIsPlaying(firstPersonAnims[reloadFullAnimIndex].anim.name))
        {
            currentAmmo--;
            PlayAnim(firstPersonAnims[fireAnimIndex].anim.name, firstPersonAnims[fireAnimIndex].animSpeed * fireAnimSpeedMultiplier, false);
        }*/
        #endregion

        //Debug.Log("Base Fire was Called!");
    }

    public virtual void AltFire()
    {
    }

    public virtual void TPFire()
    {
    }

    public virtual void Reload()
    {
        #region
        /*PlayAnim(reloadNormalAnim.name, reloadNormalAnimSpeed, false);
        if (!AnimIsPlaying(firstPersonAnims[fireAnimIndex].anim.name))
        {
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
            
        if (bHasFullReloadAnim)
        {
            if (currentAmmo <= 0)
            {
                PlayAnim(firstPersonAnims[reloadFullAnimIndex].anim.name, firstPersonAnims[reloadFullAnimIndex].animSpeed * reloadFullAnimSpeedMultiplier, false);
                StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadFullAnimIndex].anim.length / 2f));
            }
            else
            {
                PlayAnim(firstPersonAnims[reloadNormalAnimIndex].anim.name, firstPersonAnims[reloadNormalAnimIndex].animSpeed * reloadNormalAnimSpeedMultiplier, false);
                StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadNormalAnimIndex].anim.length / 2f));
            }
        }
        else if(currentAmmo > 0)
        {
            PlayAnim(firstPersonAnims[reloadNormalAnimIndex].anim.name, firstPersonAnims[reloadNormalAnimIndex].animSpeed * reloadNormalAnimSpeedMultiplier, false);
            StartCoroutine(WaitForReloadToComplete(firstPersonAnims[reloadNormalAnimIndex].anim.length / 2f));
        }*/
        #endregion
    }

    protected virtual void CreateBullet()
    {
    }

    protected virtual void PlayBullet()
    {
    }

    public virtual void SpawnTest(BoltEntity entity)
    {
    }

    protected IEnumerator WaitForReloadToComplete(float time)
    {
        yield return new WaitForSeconds(time);

        if (currentMaxAmmo >= defaultAmmo - currentAmmo)        //200 > 30 - 15
        {
            currentMaxAmmo -= defaultAmmo - currentAmmo;    //200 - 15 = 185
            currentAmmo = defaultAmmo;
        }

        if (currentMaxAmmo < defaultAmmo - currentAmmo)
        {
            currentAmmo += currentMaxAmmo;
            currentMaxAmmo = 0;
        }
    }

    public IEnumerator MuzzleFlashTimerFP(float time)
    {
        if (muzzlePoint_FP != null)
        {
            muzzlePoint_FP.gameObject.SetActive(true);

            Vector3 euler = muzzlePoint_FP.localEulerAngles;
            euler.y = Random.Range(-30f, 30f);
            muzzlePoint_FP.localEulerAngles = euler;

            yield return new WaitForSeconds(time);

            muzzlePoint_FP.gameObject.SetActive(false);
        }
    }

    public IEnumerator MuzzleFlashTimerTP(float time)
    {
        if (muzzlePoint_TP != null)
        {
            muzzlePoint_TP.gameObject.SetActive(true);

            yield return new WaitForSeconds(time);

            muzzlePoint_TP.gameObject.SetActive(false);
        }
    }

    //Recoil
    public virtual void Recoiling()
    {
        if (playerEntity.isOwner)
        {
            if (recoil > 0)
            {
                // Dampen towards the target rotation
                Quaternion maxRecoil = Quaternion.Euler(maxRecoilXY.x, maxRecoilXY.y, 0);
                CameraController.instance.transform.localRotation = Quaternion.Slerp(CameraController.instance.transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
                recoil -= Time.deltaTime;
            }
            else
            {
                // Dampen towards the target rotation
                recoil = 0;
                CameraController.instance.transform.localRotation = Quaternion.Slerp(CameraController.instance.transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2f);
            }
        }
    }
}
