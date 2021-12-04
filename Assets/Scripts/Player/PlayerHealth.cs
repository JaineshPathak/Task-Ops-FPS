using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Bolt.EntityBehaviour<ISniperPlayerState>
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObject bloodHitParticle;
    private List<GameObject> pooledBloodHitParticles;

    private PlayerSetup playerSetup;
    private PlayerMotor playerMotor;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private WeaponManager3 wManager;
    private Vector3 defaultCamPos;

    private void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        playerMotor = GetComponent<PlayerMotor>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        wManager = GetComponent<WeaponManager3>();

        if (entity.isOwner)
            defaultCamPos = CameraController.instance.transform.localPosition;

        pooledBloodHitParticles = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(bloodHitParticle, transform.position, transform.rotation) as GameObject;
            go.SetActive(false);
            pooledBloodHitParticles.Add(go);
        }
    }

    public override void Attached()
    {
        if(entity.isOwner)
            state.Health = maxHealth;

        state.AddCallback("IsDead", OnDeath);

        state.AddCallback("Health", ShowBloodEffects);
    }

    public int GetCurrentHealth()
    {
        return state.Health;
    }

    //Testing purpose
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && entity.isOwner)
            TakeDamage(10, entity, transform.position);

        if (entity.isOwner)
            if (Input.GetKeyDown(KeyCode.M))
                BoltNetwork.Instantiate(BoltPrefabs.WoodenCrate, transform.position + new Vector3(0, 0, 1f), transform.rotation);
    }

    void SetupPlayer()
    {
        state.Health = maxHealth;
        state.IsDead = false;
        capsuleCollider.enabled = true;
        rb.isKinematic = false;
        //playerSetup.ragdollController.DisableRagdolls();
        state.Animator.enabled = true;

        if(entity.isOwner)
            state.Weapon = 0;

        //playerSetup.DeactivateRagdoll();
    }

    public void TakeDamage(int damage, BoltEntity Instigator, Vector3 hitLocation)
    {
        if (state.IsDead)
            return;

        state.Health -= damage;

        state.HitLocation = hitLocation;

        ShowBloodEffects();

        if (state.Health <= 0)
            Death(Instigator);
    }

    private GameObject GetPooledBloodEffect()
    {
        for (int i = 0; i < pooledBloodHitParticles.Count; i++)
        {
            if (!pooledBloodHitParticles[i].activeInHierarchy)
                return pooledBloodHitParticles[i];
        }

        return null;
    }

    private void ShowBloodEffects()
    {
        if (state.Health == maxHealth)
            return;

        GameObject bloodEffect = GetPooledBloodEffect();

        if (bloodEffect != null)
        {
            bloodEffect.SetActive(true);
            bloodEffect.transform.position = state.HitLocation;
        }
    }

    private void Death(BoltEntity Instigator)
    {
        BoltEntity killer = GameManagerController.GetPlayer(Instigator);

        KilledEvent kEvent = KilledEvent.Create();
        kEvent.Killer = killer;
        kEvent.Victim = entity;
        kEvent.Send();

        //BoltConsole.Write("KILLER: " + killer.transform.name + killer.networkId.ToString());

        state.IsDead = true;
        state.Health = 0;
        state.ShootPos = Vector3.zero;
        state.ShootRot = Quaternion.identity;

        //playerSetup.ActivateRagdoll();
        //rb.isKinematic = true;
        //capsuleCollider.enabled = false;
        //playerSetup.ragdollController.EnableRagdolls();

        //state.Animator.enabled = false;

        StartCoroutine(Respawn());
    }

    void OnDeath()
    {
        //When player is dead!
        if (state.IsDead)
        {
            playerSetup.ragdollController.EnableRagdolls();
            rb.isKinematic = true;
            capsuleCollider.enabled = false;
            state.Animator.enabled = false;
            wManager.DisableBob();

            if (entity.isOwner)
            {
                CameraExtensions.LayerCullingToggle(CameraController.instance.myCamera, "Don't Draw", true);
                CameraController.instance.transform.localPosition = defaultCamPos + new Vector3(0, 0, -2f);
                CameraController.instance.myWeaponHolder.gameObject.SetActive(false);
            }
        }
        else
        {
            //When is player is respawned!
            playerSetup.ragdollController.DisableRagdolls();
            capsuleCollider.enabled = true;
            rb.isKinematic = false;
            state.Animator.enabled = true;
            wManager.EnableBob();
            wManager.ResetAllAmmo();

            if (entity.isOwner)
            {
                CameraExtensions.LayerCullingToggle(CameraController.instance.myCamera, "Don't Draw", false);
                CameraController.instance.transform.localPosition = defaultCamPos;
                CameraController.instance.myWeaponHolder.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator Respawn()
    {
        GameObject spawnPoint;

        if(GameManagerController.instance != null)
            yield return new WaitForSeconds(GameManagerController.instance.respawnTime);

        if (GameManagerController.instance != null)
        {
            spawnPoint = GameManagerController.instance.FindRandomSpawnPoint();
            transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
            playerMotor.curCamRotX = 0f;
        }

        SetupPlayer();
    }
}
