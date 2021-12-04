using System;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : BoltSingletonPrefab<CameraController>
{
    [SerializeField] Transform Cam;
    [SerializeField] Transform lookAtTransform;
    [SerializeField] Transform weaponHolderTransform;
    [SerializeField] Transform shootPoint;
    [SerializeField] private AnimationClip camAnimJumpClip;
    [SerializeField] private AnimationClip camAnimHardLandClip;
    [SerializeField] private PostProcessingProfile pp_Profile;

    private bool pp_MotionBlur;
    private bool pp_AO;
    private bool pp_Bloom;
    private bool b_lensFlare;
    private FlareLayer myLensFlare;

    private JSONObject configJSON;
    private string configFilePath;

    public Camera myCamera
    {
        get { return Cam.GetComponent<Camera>(); }
    }

    public Transform myLookAtTransform
    {
        get { return lookAtTransform; }
    }

    public Transform myWeaponHolder
    {
        get
        {
            return weaponHolderTransform;
        }
    }

    public Transform myShootPoint
    {
        get
        {
            return shootPoint;
        }
    }

    public string camJumpAnimClip
    {
        get { return camAnimJumpClip.name; }
    }

    public string camHardLandAnimClip
    {
        get { return camAnimHardLandClip.name; }
    }

    private const string newName = "PlayerCamera";

    // Setup the camera with the default view
    public void Configure(BoltEntity entity)
    {
        BoltConsole.Write("Configure called on player camera", Color.green);

        Transform cameraPos = entity.transform.Find("CameraPos");

        gameObject.transform.name = newName;
        gameObject.transform.parent = cameraPos;
        gameObject.transform.position = cameraPos.position;
        gameObject.transform.rotation = cameraPos.rotation;

        //gameObject.transform.localPosition = new Vector3(0, 100, 0);
        //gameObject.transform.localRotation = Quaternion.Euler(90f, 0, 0);
    }

    private void Start()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        configJSON = new JSONObject();
        string jSONString = File.ReadAllText(configFilePath);
        configJSON = JSON.Parse(jSONString) as JSONObject;

        LoadAOSettings();
        LoadBloomSettings();
        LoadMotionBlurSettings();
        LoadLensFlareSetting();
    }

    private void LoadAOSettings()
    {
        AmbientOcclusionModel aoSettings = pp_Profile.ambientOcclusion;

        pp_AO = configJSON["Effect_Ambient_Occlusion"];

        aoSettings.enabled = pp_AO;
        pp_Profile.ambientOcclusion.enabled = aoSettings.enabled;
    }

    private void LoadBloomSettings()
    {
        BloomModel bloomSettings = pp_Profile.bloom;

        pp_Bloom = configJSON["Effect_Bloom"];

        bloomSettings.enabled = pp_Bloom;
        pp_Profile.bloom.enabled = bloomSettings.enabled;
    }

    private void LoadMotionBlurSettings()
    {
        MotionBlurModel mbSettings = pp_Profile.motionBlur;

        pp_MotionBlur = configJSON["Effect_Motion_Blur"];

        mbSettings.enabled = pp_MotionBlur;
        pp_Profile.motionBlur.enabled = mbSettings.enabled;
    }

    private void LoadLensFlareSetting()
    {
        myLensFlare = Cam.GetComponent<FlareLayer>();

        b_lensFlare = configJSON["Effect_Lens_Flare"];

        myLensFlare.enabled = b_lensFlare;
    }
}
