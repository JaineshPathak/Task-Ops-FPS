using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class WeaponManager2 : Bolt.EntityEventListener<ISniperPlayerState>
{
    /*[SerializeField] private WeaponsDatabase weaponsDatabasePrefab;
    [SerializeField] private GameObject[] playerFPWeapons;
    [SerializeField] private GameObject[] playerTPWeapons;*/

    [SerializeField] private WeaponsDatabase weaponsDatabasePrefab;
    [SerializeField] private WeaponStats[] playerWeapons;

    [SerializeField] private GameObject[] playerFPWeapons;
    [SerializeField] private GameObject[] playerTPWeapons;

    //Third Person AimIK Stuff
    private Animator anim;
    private AimIK aimIK;
    private Transform lookAtTransform;
    private Transform aimTransform;
    private Transform weaponHand;
    private Transform rightHandBone;
    [SerializeField] private GameObject lookAtPrefab;

    //First Person stuff
    private Transform weaponHolder;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        aimIK = GetComponentInChildren<AimIK>();
    }

    public override void Attached()
    {
        if (entity.isOwner)
        {
            /*if (PlayerPrefs.HasKey("WeaponID"))
                state.PlayerWeaponSlot[0] = PlayerPrefs.GetInt("WeaponID");

            if (PlayerPrefs.HasKey("SecondWeaponID"))
                state.PlayerWeaponSlot[1] = PlayerPrefs.GetInt("SecondWeaponID");*/

            weaponHolder = CameraController.instance.myWeaponHolder;
        }

        //playerWeapons[0] = weaponsDatabasePrefab.Weapons[state.PrimaryWeaponID];
        //playerWeapons[1] = weaponsDatabasePrefab.Weapons[state.SecondaryWeaponID];

        if (entity.isOwner)
            EquipFPWeapons();

        EquipTPWeapons();

        state.AddCallback("Weapon", WeaponChanged);

        WeaponChanged();
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


    void EquipFPWeapons()
    {
        playerFPWeapons[0] = (GameObject)Instantiate(playerWeapons[0].fpPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        playerFPWeapons[1] = (GameObject)Instantiate(playerWeapons[1].fpPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);

        //playerFPWeapons[0] = (GameObject)Instantiate(weaponsDatabasePrefab.Weapons[state.PrimaryWeaponID].GetComponent<WeaponStats>().fpPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        //playerFPWeapons[1] = (GameObject)Instantiate(weaponsDatabasePrefab.Weapons[state.SecondaryWeaponID].GetComponent<WeaponStats>().fpPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
    }

    void EquipTPWeapons()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        playerTPWeapons[0] = (GameObject)Instantiate(playerWeapons[0].tpPrefab, weaponHand.position, weaponHand.rotation, weaponHand);
        playerTPWeapons[1] = (GameObject)Instantiate(playerWeapons[1].tpPrefab, weaponHand.position, weaponHand.rotation, weaponHand);

        //playerTPWeapons[0] = (GameObject)Instantiate(weaponsDatabasePrefab.Weapons[state.PrimaryWeaponID].GetComponent<WeaponStats>().tpPrefab, weaponHand.position, weaponHand.rotation, weaponHand);
        //playerTPWeapons[1] = (GameObject)Instantiate(weaponsDatabasePrefab.Weapons[state.SecondaryWeaponID].GetComponent<WeaponStats>().tpPrefab, weaponHand.position, weaponHand.rotation, weaponHand);

        aimTransform = playerTPWeapons[state.Weapon].transform.Find("AimingTransform");
    }

    void WeaponChanged()
    {
        if (entity.isOwner)
        {
            for (int i = 0; i < playerFPWeapons.Length; i++)
                playerFPWeapons[i].SetActive(false);

            playerFPWeapons[state.Weapon].SetActive(true);

            //GetComponent<PlayerShoot>().fpWeaponAnim = myFPWeapons[state.Weapon].GetComponentInChildren<Animator>();
        }

        if (playerTPWeapons != null)
        {
            for (int i = 0; i < playerTPWeapons.Length; i++)
                playerTPWeapons[i].SetActive(false);

            playerTPWeapons[state.Weapon].SetActive(true);
        }
    }

    void LateUpdate()
    {
        lookAtPrefab.transform.position = state.camPos;

        aimIK.solver.target = lookAtPrefab.transform;
        aimIK.solver.transform = aimTransform;
    }
}
