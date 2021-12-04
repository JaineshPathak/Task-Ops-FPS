using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class WeaponManager : Bolt.EntityEventListener<ISniperPlayerState>
{
    Transform weaponHolder;

    Animator anim;
    Transform weaponHand;
    Transform rightHandBone;

    [SerializeField] WeaponDB weaponsDatabase;
    [SerializeField] WeaponObject[] inventoryObjects;

    [SerializeField] GameObject[] myFPWeapons;
    [SerializeField] GameObject[] myTPWeapons;
    
    AimIK aimIK;
    Transform lookAtTransform;
    Transform aimTransform;

    [SerializeField] GameObject lookAtPrefab;

    public WeaponObject activeWeaponObject
    {
        get
        {
            return inventoryObjects[state.Weapon];
        }
    }

    public GameObject activeFPWeaponObject
    {
        get
        {
            return myFPWeapons[state.Weapon];
        }
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        aimIK = GetComponentInChildren<AimIK>();
    }

    private void Update()
    {
        if (entity.isOwner)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                state.Weapon = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                state.Weapon = 1;
        }
    }

    public override void Attached()
    {
        /*if (entity.isOwner)
        {
            if (PlayerPrefs.HasKey("WeaponID"))
                state.PlayerWeaponSlot[0] = PlayerPrefs.GetInt("WeaponID");

            if (PlayerPrefs.HasKey("SecondWeaponID"))
                state.PlayerWeaponSlot[1] = PlayerPrefs.GetInt("SecondWeaponID");

            weaponHolder = CameraController.instance.myWeaponHolder;
        }*/

        //inventoryObjects[0] = weaponsDatabase.weaponsList[state.PlayerWeaponSlot[0]];
        //inventoryObjects[1] = weaponsDatabase.weaponsList[state.PlayerWeaponSlot[1]];

        EquipFPWeapon();
        EquipTPWeapon();

        state.AddCallback("Weapon", WeaponChanged);

        WeaponChanged();
    }

    void Start()
    {
        //weaponHolder = cam.gameObject.transform.Find("WeaponHolder");

        if (weaponsDatabase == null)
            Debug.LogError("PLEASE PUT SOME WEAPONOBJECTS IN THE ARRAY!");

        
        //EquipTPWeapon();

        /*if (entity.hasControl)
        {
            cube = BoltNetwork.Instantiate(BoltPrefabs.LookAtCubeTest2, CameraController.instance.transform.position, CameraController.instance.transform.rotation);
            cube.transform.SetParent(CameraController.instance.myCamera.transform);
            cube.transform.position = CameraController.instance.myCamera.transform.position + new Vector3(0, 0, 3f);
        }*/
    }

    void EquipFPWeapon()
    {
        if(weaponHolder != null)
        {
            //fpWeapon = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PrimaryWeaponID].fpWeaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);

            //myFPWeapons[0] = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PlayerWeaponSlot[0]].fpWeaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            //myFPWeapons[1] = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PlayerWeaponSlot[1]].fpWeaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        }
    }

    void EquipTPWeapon()
    {
        if (weaponsDatabase != null)
        {
            rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
            weaponHand = rightHandBone.Find("WeaponHand");

            //GameObject tpWeapon = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PrimaryWeaponID].tpWeaponPrefab, weaponHand.position, weaponHand.rotation, weaponHand);
            //tpWeapon.transform.localScale = weaponsDatabase.weaponsList[state.PrimaryWeaponID].tpWeaponPrefab.transform.localScale;

            /*myTPWeapons[0] = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PlayerWeaponSlot[0]].tpWeaponPrefab, weaponHand.position, weaponHand.rotation, weaponHand);
            myTPWeapons[0].transform.localScale = weaponsDatabase.weaponsList[state.PlayerWeaponSlot[0]].tpWeaponPrefab.transform.localScale;

            myTPWeapons[1] = (GameObject)Instantiate(weaponsDatabase.weaponsList[state.PlayerWeaponSlot[1]].tpWeaponPrefab, weaponHand.position, weaponHand.rotation, weaponHand);
            myTPWeapons[1].transform.localScale = weaponsDatabase.weaponsList[state.PlayerWeaponSlot[1]].tpWeaponPrefab.transform.localScale;*/

            aimTransform = myTPWeapons[state.Weapon].transform.Find("AimingTransform");
        }

            /*if (entity.isOwner)
            {
                lookAtPrefab.transform.parent = CameraController.instance.myLookAtTransform;
                lookAtPrefab.transform.position = CameraController.instance.myLookAtTransform.position;
            }

            //lookAtPrefabGO = (GameObject)Instantiate(lookAtPrefab);

            if (entity.isOwner)
            {
                lookAtPrefabGO.transform.SetParent(CameraController.instance.myLookAtTransform);
                lookAtPrefabGO.transform.position = CameraController.instance.myLookAtTransform.position;
                //lookAtPrefabGO.transform.position = CameraController.instance.myCamera.gameObject.transform.position + lookAtPos;
            }*/
            //if(entity.isOwner && entity.hasControl)
            //lookAtTransform = CameraController.instance.myLookAtTransform;
    }

    void WeaponChanged()
    {
        if(entity.isOwner && myFPWeapons != null)
        {
            for (int i = 0; i < myFPWeapons.Length; i++)
                myFPWeapons[i].gameObject.SetActive(false);

            myFPWeapons[state.Weapon].gameObject.SetActive(true);

            //GetComponent<PlayerShoot>().fpWeaponAnim = myFPWeapons[state.Weapon].GetComponentInChildren<Animator>();
        }

        if (myTPWeapons != null)
        {
            for (int i = 0; i < myTPWeapons.Length; i++)
                myTPWeapons[i].gameObject.SetActive(false);

            myTPWeapons[state.Weapon].gameObject.SetActive(true);
        }
    }

    /*private void Update()
    {
        lookAtTransform.position = state.camPos;
        lookAtPrefab.transform.position = state.camPos;
    }*/

    void LateUpdate()
    {
        lookAtPrefab.transform.position = state.camPos;

        aimIK.solver.target = lookAtPrefab.transform;
        aimIK.solver.transform = aimTransform;
    }
}

/*
public class WeaponManager : Bolt.EntityBehaviour<ISniperPlayerState>
{
    int weaponID;
    //[SerializeField] WeaponObject[] playerWeapons;    //Stores weapons with WeaponStats

    [SerializeField] GameObject[] myWeapons;

    //[SerializeField] GameObject[] tpPrefabs;
    //[SerializeField] GameObject[] fpPrefabs;

    //FP and TP setup
    Camera cam;
    Transform weaponHolder;

    //Third Person weapon vars
    Animator anim;
    Transform weaponHand;
    Transform rightHandBone;

    //Aim IK stuff
    AimIK aimIK;
    Transform aimTransform;
    Transform lookAtTransform;

    GameObject fpWeaponPrefab;
    GameObject tpWeaponPrefab;

    //[SerializeField] GameObject thirdPersonWeapon;

    void Start ()
    {
        anim = GetComponentInChildren<Animator>();

        //cam = GetComponentInChildren<Camera>();
        if(entity.isOwner && entity.hasControl)
            cam = CameraController.instance.myCamera;

        weaponHolder = cam.gameObject.transform.Find("WeaponHolder");

        //Setup AimIK
        aimIK = GetComponentInChildren<AimIK>();

        weaponID = GetWeaponID();

        //SetupWeapon();

        EquipFPWeapon();
        EquipTPWeapon();
	}

    int GetWeaponID()
    {
        if (PlayerPrefs.HasKey("WeaponID"))
            return PlayerPrefs.GetInt("WeaponID");

        return PlayerPrefs.GetInt("WeaponID", 0);
    }

    void EquipFPWeapon()
    {
        //fpWeaponPrefab = playerWeapons[weaponID].fpWeaponPrefab;

        fpWeaponPrefab = myWeapons[weaponID].GetComponent<WeaponStats>().fpPrefab;

        //fpWeaponPrefab = fpPrefabs[weaponID];

        GameObject fpWeaponGO = Instantiate(fpWeaponPrefab, weaponHolder.position, weaponHolder.rotation) as GameObject;
        fpWeaponGO.transform.parent = weaponHolder;

        #region
        //if (!playerEntity.isOwner)
        //SetLayerRecursively(fpWeapon, LayerMask.NameToLayer("Don't Draw"));

        /*switch (weaponID)
        {
            case 0:
                fpWeaponGO = (GameObject)Instantiate(fpWeapon, weaponHolder.position, weaponHolder.rotation);
                fpWeaponGO.transform.parent = weaponHolder;

                //I am not the owner of weapon so hide it.
                if (!playerEntity.isOwner)
                    SetLayerRecursively(fpWeapon, LayerMask.NameToLayer("Don't Draw"));

                break;
            case 1:
                fpWeapon = (GameObject)Instantiate(playerWeapons[1].GetComponent<WeaponStats>().GetFPWeapon(), weaponHolder.position, weaponHolder.rotation);
                fpWeapon.transform.parent = weaponHolder;

                if (!playerEntity.isOwner)
                    SetLayerRecursively(fpWeapon, LayerMask.NameToLayer("Don't Draw"));

                break;

        }
        #endregion
    }

    void EquipTPWeapon()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        //

        //tpWeaponPrefab = tpPrefabs[weaponID];

        state.WeaponId = weaponID;
        tpWeaponPrefab = myWeapons[state.WeaponId].GetComponent<WeaponStats>().tpPrefab;

        //tpWeaponPrefab = playerWeapons[state.WeaponId].tpWeaponPrefab;

        GameObject tpWeaponGO = Instantiate(tpWeaponPrefab, weaponHand.position, weaponHand.rotation) as GameObject;
        tpWeaponGO.transform.SetParent(weaponHand);
        tpWeaponGO.transform.localScale = tpWeaponPrefab.transform.localScale;

        aimTransform = tpWeaponGO.transform.Find("AimingTransform");

        lookAtTransform = cam.gameObject.transform.Find("LookAtTarget");

        //if (playerEntity.isOwner)
        //SetLayerRecursively(tpWeaponGO, LayerMask.NameToLayer("Don't Draw"));

        #region
        /*switch (weaponID)
        {
            case 0:
                tpWeapon = playerWeapons[0].GetComponent<WeaponStats>().GetTPWeapon();

                tpWeaponGO = Instantiate(tpWeapon, weaponHand.position, weaponHand.rotation) as GameObject;
                tpWeaponGO.transform.parent = weaponHand;
                tpWeaponGO.transform.localScale = playerWeapons[0].GetComponent<WeaponStats>().GetTPWeapon().transform.localScale;
                aimTransform = tpWeaponGO.transform.Find("AimingTransform");

                
                break;
            case 1:
                tpWeapon = Instantiate(playerWeapons[1].GetComponent<WeaponStats>().GetTPWeapon(), weaponHand.position, weaponHand.rotation) as GameObject;
                tpWeapon.transform.parent = weaponHand;
                tpWeapon.transform.localScale = playerWeapons[1].GetComponent<WeaponStats>().GetTPWeapon().transform.localScale;
                aimTransform = tpWeapon.transform.Find("AimingTransform");

                if (playerEntity.isOwner)
                    SetLayerRecursively(tpWeapon, LayerMask.NameToLayer("Don't Draw"));
                break;
        }
        #endregion
    }

    /*void SetupWeapon()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        GameObject tpWeapon = Instantiate(thirdPersonWeapon, weaponHand.position, weaponHand.rotation) as GameObject;
        tpWeapon.transform.parent = weaponHand;
        tpWeapon.transform.localScale = thirdPersonWeapon.transform.localScale;

        aimTransform = tpWeapon.transform.Find("AimingTransform");
        lookAtTransform = camController.myLookAtTransform;
    }

    void LateUpdate()
    {
        aimIK.solver.target = lookAtTransform;
        aimIK.solver.transform = aimTransform;
    }
}
*/