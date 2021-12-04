using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using RootMotion.FinalIK;
using UnityEngine.Audio;

public class PlayerSetup : Bolt.EntityBehaviour<ISniperPlayerState>
{
    [SerializeField] private const string remotePlayerLayerName = "Remote Player";
    [SerializeField] private const string dontDrawLayerName = "Don't Draw";
    [SerializeField] private const string remoteMinimapLayerName = "Minimap Local Player";
    [SerializeField] private const string localMinimapLayerName = "Minimap Remote Player";
    [SerializeField] private SpriteRenderer minimapIcon;
    [SerializeField] private AudioMixer mainMixer;
    [HideInInspector] public RagdollController ragdollController;

    public GameObject playerGraphics;
    public Transform weaponBackPos;

    private GameObject playerRagdollInstance;
    private Animator anim;
    private string playerName;
    private float loadGameVolume;
    private JSONObject configJSON;
    private string configFilePath;

    #region
    /*Transform rightHandBone;
    Transform weaponHand;
    [SerializeField] GameObject[] thirdPersonWeaponPrefabs;

    [SerializeField] GameObject playerUIPrefab;

    [HideInInspector] public GameObject playerUIInstance;

    Animator anim;

    Transform weaponHand;
    Transform rightHandBone;
    [SerializeField] GameObject thirdPersonWeaponPrefab;

    AimIK aimIK;
    Transform aimTransform;
    CameraController camController;
    Transform lookAtTransform;*/
    #endregion

    public override void Attached()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();
            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }

        loadGameVolume = configJSON["Audio_Game_Volume"];
        mainMixer.SetFloat("GameVol", loadGameVolume);

        //Hide the playerGraphics for local player. Remote player can see this.
        if (entity.isOwner)
        {
            Utils.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
        }

        playerName = configJSON["Player_Name"];
        if (playerName.Length == 0)
        {
            state.PlayerName = transform.name;
            transform.name = state.PlayerName;
        }
        else
        {
            state.PlayerName = playerName;
            transform.name = state.PlayerName;
        }

        GameManagerController.RegisterPlayer(entity);
    }

    public override void Detached()
    {
        GameManagerController.UnregisterPlayer(entity);
    }

    void Start()
    {
        #region
        /*anim = playerGraphics.GetComponent<Animator>();
        playerEntity = GetComponent<BoltEntity>();
        camController = GetComponentInChildren<CameraController>();

        aimIK = playerGraphics.GetComponent<AimIK>();*/
        #endregion

        anim = playerGraphics.GetComponent<Animator>();

        ragdollController = GetComponent<RagdollController>();
        if(ragdollController != null)
            ragdollController.SetupRagdolls(playerGraphics);

        if (!entity.isOwner)
            AssignRemoteLayerName();

        #region
        //SetupWeapon();
        //SetupThirdPersonWeapon();

        //if(playerUIPrefab != null)
        //SpawnPlayerUI();
        #endregion

        AssignMinimapColor();

        Cursor.lockState = PauseMenu.isOn ? CursorLockMode.None : CursorLockMode.Locked;

        //AssignRagdoll();
	}

    void AssignRemoteLayerName()
    {
        gameObject.layer = LayerMask.NameToLayer(remotePlayerLayerName);
    }

    void AssignMinimapColor()
    {
        if(minimapIcon != null)
        {
            if (entity.isOwner)
            {
                minimapIcon.gameObject.layer = LayerMask.NameToLayer(localMinimapLayerName);
                minimapIcon.color = Color.green;        //Local Player
            }
            else
            {
                minimapIcon.gameObject.layer = LayerMask.NameToLayer(remoteMinimapLayerName);
                minimapIcon.color = Color.red;          //Remote Player
            }
        }
    }

    /*void AssignRagdoll()
    {
        playerRagdollInstance = GameObject.Instantiate(playerRagdoll, transform.position, transform.rotation, transform);
        if (playerRagdollInstance != null)
            playerRagdollInstance.SetActive(false);
    }

    public void ActivateRagdoll()
    {
        if (playerRagdollInstance != null)
        {
            playerRagdollInstance.transform.parent = null;
            playerRagdollInstance.SetActive(true);
        }
    }

    public void DeactivateRagdoll()
    {
        if(playerRagdollInstance != null)
        {
            playerRagdollInstance.transform.SetParent(transform);
            playerRagdollInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);
            playerRagdollInstance.gameObject.SetActive(false);
        }
    }*/

    #region
    /*void SetupThirdPersonWeapon()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        //state.WeaponId = PlayerPrefs.GetInt("WeaponID");

        GameObject tpWeapon = Instantiate(thirdPersonWeaponPrefabs[0], weaponHand.position, weaponHand.rotation) as GameObject;
        tpWeapon.transform.parent = weaponHand;
        tpWeapon.transform.localScale = thirdPersonWeaponPrefabs[0].transform.localScale;
    }*/

    /*void SpawnPlayerUI()
    {
        playerUIInstance = Instantiate(playerUIPrefab);
        playerUIInstance.name = playerUIPrefab.name;

        PlayerUI playerUI = playerUIInstance.GetComponent<PlayerUI>();
        playerUI.SetupMiniMap();
        playerUI.SetHealthController(GetComponent<PlayerHealth>());
        state.AddCallback("Health", playerUI.HealthChanged);
        //playerUI.SetupHealthText(state.Health);
    }*/

    /*void SetupWeapon()
    {
        rightHandBone = anim.GetBoneTransform(HumanBodyBones.RightHand);
        weaponHand = rightHandBone.Find("WeaponHand");

        GameObject tpWeapon = Instantiate(thirdPersonWeaponPrefab, weaponHand.position, weaponHand.rotation) as GameObject;
        tpWeapon.transform.parent = weaponHand;
        tpWeapon.transform.localScale = thirdPersonWeaponPrefab.transform.localScale;

        aimTransform = tpWeapon.transform.Find("AimingTransform");
        lookAtTransform = camController.myLookAtTransform;
    }*/

    /*void SetLayerRecursively(GameObject _playerGraphics, int newLayer)
    {
        _playerGraphics.layer = newLayer;

        foreach (Transform child in _playerGraphics.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
            //child.gameObject.layer = newLayer;
        }
    }*/

    /*void LateUpdate()
    {
        aimIK.solver.target = lookAtTransform;
        aimIK.solver.transform = aimTransform;
    }*/
    #endregion
}
