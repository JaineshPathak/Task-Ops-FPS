using System.Collections;
using UnityEngine;

public class BulletPhysicTest : MonoBehaviour
{
    [SerializeField] private float life = 5f;
    [SerializeField] private float speed = 100f;
    [SerializeField] private float gravity = -9f;
    [SerializeField] private GameObject impactPrefab;
    [HideInInspector] public Vector3 oldPos, newPos;

    private LayerMask hitLayerMask;

    private bool hasHit;
    private Vector3 velocity;
    private Vector3 force;
    private RaycastHit lastHit;
    private GameObject impactPrefabInstance;

    private BoltEntity playerEntity;
    private WeaponBase wBase;

    public void SetupBullet(BoltEntity _playerEntity, WeaponBase _wBase, LayerMask _hitLayerMask, float _Speed)
    {
        playerEntity = _playerEntity;
        wBase = _wBase;
        hitLayerMask = _hitLayerMask;
        speed = _Speed;
    }

    void Awake()
    {
        impactPrefabInstance = Instantiate(impactPrefab, transform.position, transform.rotation) as GameObject;
        impactPrefabInstance.SetActive(false);
    }

    void Start()
    {
        oldPos = transform.position;
        newPos = transform.position;

        //Destroy(gameObject, life);
    }

    private void OnEnable()
    {
        hasHit = false;
        velocity = Vector3.zero;
        force = Vector3.zero;
        StartCoroutine(DisableAfterSec(life));
    }

    private IEnumerator DisableAfterSec(float t)
    {
        yield return new WaitForSeconds(t);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (hasHit)
            return;
    
        velocity = speed * transform.forward + force;
        force += new Vector3(0, gravity * Time.deltaTime, 0);
        newPos += velocity * Time.deltaTime;

        Vector3 direction = newPos - oldPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(oldPos, direction, out hit, distance, hitLayerMask))
            {
                hasHit = true;
                OnHit(hit, direction);
                lastHit = hit;
            }
        }

        if (!hasHit)
        {
            oldPos = transform.position;
            transform.position = newPos;
        }
        else
        {
            transform.position = lastHit.point;
        }
    }

    private void OnHit(RaycastHit _hit, Vector3 _dir)
    {
        if (_hit.collider != null && _hit.collider.isTrigger)
        {
            if (playerEntity != null && wBase != null)
            {
                BoltConsole.Write("I HIT SOMETHING!");

                BoltEntity victimEntity = _hit.collider.transform.root.GetComponent<BoltEntity>();

                if (victimEntity != null && playerEntity != victimEntity)
                {
                    if (!victimEntity.GetState<ISniperPlayerState>().IsDead)
                    {
                        PlayerHitbox pHitBox = _hit.collider.GetComponent<PlayerHitbox>();

                        if (pHitBox != null)
                        {
                            Vector3 hitBloodPos = _hit.point;

                            DamageEvent dmgEvent = DamageEvent.Create();
                            dmgEvent.KillerID = playerEntity;
                            dmgEvent.VictimID = victimEntity;
                            dmgEvent.Hitlocation = _hit.point;

                            BoltConsole.Write("HITLOC: " + dmgEvent.Hitlocation);

                            switch (pHitBox.hitBoxType)
                            {
                                case PlayerHitbox.hitBox.head:
                                    dmgEvent.Damage = wBase.headShotDamage;
                                    BoltConsole.Write("Bullet Hit HEAD!", Color.red);
                                    break;
                                case PlayerHitbox.hitBox.chest:
                                    dmgEvent.Damage = wBase.bodyDamage;
                                    BoltConsole.Write("Bullet Hit CHEST!", Color.white);
                                    break;
                                case PlayerHitbox.hitBox.leg:
                                    dmgEvent.Damage = wBase.legDamage;
                                    BoltConsole.Write("Bullet Hit LEGS!", Color.green);
                                    break;
                                case PlayerHitbox.hitBox.hand:
                                    dmgEvent.Damage = wBase.handDamage;
                                    BoltConsole.Write("Bullet Hit HANDS!", Color.yellow);
                                    break;
                            }

                            dmgEvent.Send();
                        }
                    }
                }
            }
        }
        else
        {
            if (impactPrefabInstance.activeSelf)
                impactPrefabInstance.SetActive(false);

            impactPrefabInstance.SetActive(true);
            impactPrefabInstance.transform.position = _hit.point;
            impactPrefabInstance.transform.rotation = Quaternion.FromToRotation(Vector3.forward, _hit.normal);

            //Instantiate(impactPrefab, _hit.point, Quaternion.FromToRotation(Vector3.forward, _hit.normal));
        }

        gameObject.SetActive(false);
    }
}
