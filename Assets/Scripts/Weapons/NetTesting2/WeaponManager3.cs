using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using System;
using WeaponBobbing;

public class WeaponManager3 : Bolt.EntityBehaviour<ISniperPlayerState>
{
    [SerializeField] private WeaponsDatabase weaponsDatabase;
    [SerializeField] private WeaponBase[] playerWeapons;
    [SerializeField] private GameObject lookAtPrefab;

    //First Person stuff
    private Transform weaponHolder;
    private WeaponBobCSGO wBob;

    //Third Person stuff
    private Animator anim;
    private AimIK aimIK;
    private Transform aimTransform;
    private Transform weaponHand;
    private Transform weaponBackPos;
    private Transform rightHandBone;
    private bool setupDone;
    private int weaponPrimaryId, weaponSecondaryId;

    public WeaponBase activeWeapon
    {
        get
        {
            return playerWeapons[state.Weapon];
        }
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        aimIK = GetComponentInChildren<AimIK>();
    }

    public override void Attached()
    {
        if (entity.isOwner)
        {
            if (PlayerPrefs.HasKey("WeaponID"))
                state.WeaponPrimaryId = PlayerPrefs.GetInt("WeaponID");

            if (PlayerPrefs.HasKey("SecondWeaponID"))
                state.WeaponSecondaryId = PlayerPrefs.GetInt("SecondWeaponID");

            weaponHolder = CameraController.instance.myWeaponHolder;
            wBob = weaponHolder.GetComponent<WeaponBobCSGO>();
        }

        weaponBackPos = GetComponent<PlayerSetup>().weaponBackPos;

        Array.Resize(ref playerWeapons, 2);

        //for(int i = 0; i < state.PlayerWeaponSlot.Length; i++)
        playerWeapons[0] = Instantiate(weaponsDatabase.Weapons[state.WeaponPrimaryId], transform.position, transform.rotation, transform);
        playerWeapons[1] = Instantiate(weaponsDatabase.Weapons[state.WeaponSecondaryId], transform.position, transform.rotation, transform);

        if (entity.isOwner)
            EquipFPWeapons();

        state.AddCallback("Weapon", WeaponChanged);

        WeaponChanged();

        //EquipTPWeapons();
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
        for (int i = 0; i < playerWeapons.Length; i++)
        {
            playerWeapons[i].SetupFirstPersonWeapon(weaponHolder);
            //playerWeapons[i].GetComponent<WeaponBase>().SetupWeapon(entity, GetComponent<PlayerController>());
        }

        /*playerWeapons[0].GetComponent<WeaponBase>().SetupFirstPersonWeapon(weaponHolder);
        playerWeapons[1].GetComponent<WeaponBase>().SetupFirstPersonWeapon(weaponHolder);

        playerWeapons[0].GetComponent<WeaponBase>().SetupWeapon(entity, GetComponent<PlayerController>());
        playerWeapons[1].GetComponent<WeaponBase>().SetupWeapon(entity, GetComponent<PlayerController>());*/
    }

    void EquipTPWeapons()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        //for (int i = 0; i < playerWeapons.Length; i++)
            //playerWeapons[i].GetComponent<WeaponBase>().SetupThirdPersonWeapon(weaponHand);
    }

    void WeaponChanged()
    {
        if(!setupDone)
        {
            rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
            weaponHand = rightHandBone.Find("WeaponHand");

            for (int i = 0; i < playerWeapons.Length; i++)
            {
                playerWeapons[i].SetupWeapon(entity, state, GetComponent<PlayerController>(), weaponHand, weaponBackPos);
                playerWeapons[i].gameObject.SetActive(false);
            }

            setupDone = true;
        }

        for (int i = 0; i < playerWeapons.Length; i++)
            playerWeapons[i].gameObject.SetActive(false);

        playerWeapons[state.Weapon].gameObject.SetActive(true);

        aimTransform = activeWeapon.aimingTransform;
    }

    void LateUpdate()
    {
        lookAtPrefab.transform.position = state.camPos;

        aimIK.solver.target = lookAtPrefab.transform;
        aimIK.solver.transform = aimTransform;
    }

    public void EnableBob()
    {
        if (entity.isOwner)
            wBob.enabled = true;
    }

    public void DisableBob()
    {
        if (entity.isOwner)
            wBob.enabled = false;
    }

    public void ResetAllAmmo()
    {
        for (int i = 0; i < playerWeapons.Length; i++)
        {
            playerWeapons[i].ResetAmmo();
        }
    }
}
