using System.Collections;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float destroyTimer = 5f;
    [SerializeField] private float sphereRadius = 0.5f;

    [SerializeField] private GameObject sandHitParticle;
    private GameObject sandHitParticleInstance;

    [SerializeField] private GameObject bloodHitParticle;
    private GameObject bloodHitParticleInstance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        sandHitParticleInstance = Instantiate(sandHitParticle, transform.position, transform.rotation) as GameObject;
        sandHitParticleInstance.SetActive(false);

        //bloodHitParticleInstance = Instantiate(bloodHitParticle, transform.position, transform.rotation) as GameObject;
        //bloodHitParticleInstance.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateAfterFewSec(destroyTimer));
        //sandHitParticle.gameObject.SetActive(false);
        //sandHitParticle.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;

        StopAllCoroutines();
    }

    IEnumerator DeactivateAfterFewSec(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }

    public void CheckForHitBox(BoltEntity playerEntity, WeaponBase wBase)
    {
        StartCoroutine(CheckHitBox(playerEntity, wBase));
    }

    IEnumerator CheckHitBox(BoltEntity playerEntity, WeaponBase wBase)
    {
        while (true)
        {
            if (playerEntity.isOwner)
            {
                using (var hits = BoltNetwork.OverlapSphereAll(transform.position, sphereRadius))
                {
                    for (int i = 0; i < hits.count; i++)
                    {
                        BoltPhysicsHit hit = hits.GetHit(i);

                        BoltEntity victimEntity = hit.body.GetComponent<BoltEntity>();
                        PlayerHealth victimHealth;

                        if (victimEntity != playerEntity)
                        {
                            victimHealth = hit.body.GetComponent<PlayerHealth>();

                            /*if (bloodHitParticle != null)
                            {
                                Vector3 hitBloodPos = hit.hitbox.transform.position;

                                if (bloodHitParticleInstance.activeSelf)
                                    bloodHitParticleInstance.SetActive(false);

                                bloodHitParticle.SetActive(true);
                                bloodHitParticle.transform.position = hitBloodPos;
                            }

                            BoltConsole.Write("Hit: HIT: " + hit.hitbox.hitboxType, Color.green);
                            BoltConsole.Write("Hit: HIT body: " + hit.body.name, Color.white);


                            var serializer = hit.body.GetComponent<PlayerHealth>();

                            BoltConsole.Write("Hit: HIT: " + hit.hitbox.hitboxType, Color.green);
                            BoltConsole.Write("Hit: HIT body: " + hit.body.name, Color.white);*/

                            if ( (!victimEntity.GetState<ISniperPlayerState>().IsDead) && victimHealth != null)
                            {
                                Vector3 hitBloodPos = hit.hitbox.transform.position;

                                DamageEvent dmgEvent = DamageEvent.Create();
                                dmgEvent.VictimID = victimEntity;
                                //dmgEvent.HitLocation = hitBloodPos;

                                switch (hit.hitbox.hitboxType)
                                {
                                    case BoltHitboxType.Body:
                                        dmgEvent.Damage = wBase.bodyDamage;
                                        break;
                                    case BoltHitboxType.Head:
                                        dmgEvent.Damage = wBase.headShotDamage;
                                        break;
                                    case BoltHitboxType.Hand:
                                        dmgEvent.Damage = wBase.handDamage;
                                        break;
                                    case BoltHitboxType.UpperArm:
                                        dmgEvent.Damage = wBase.handDamage;
                                        break;
                                    case BoltHitboxType.Thigh:
                                        dmgEvent.Damage = wBase.handDamage;
                                        break;
                                    case BoltHitboxType.Leg:
                                        dmgEvent.Damage = wBase.legDamage;
                                        break;
                                }

                                dmgEvent.Send();

                                gameObject.SetActive(false);

                                //serializer.TakeDamage(wManager.activeWeapon.bodyDamage);
                            }
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Collider other = collision.collider;
            ContactPoint contactPoint = collision.contacts[0];

            Vector3 hitPos = contactPoint.point;
            Quaternion hitRot = Quaternion.FromToRotation(Vector3.forward, contactPoint.normal);

            if (sandHitParticleInstance != null)
            {
                if (sandHitParticleInstance.activeSelf)
                    sandHitParticleInstance.SetActive(false);

                sandHitParticleInstance.SetActive(true);
                sandHitParticleInstance.transform.position = hitPos;
                sandHitParticleInstance.transform.rotation = hitRot;

                //sandHitParticle.Play(true);
            }

            gameObject.SetActive(false);
        }
    }
}
