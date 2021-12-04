using System.Collections;
using UnityEngine;

public class FakeBullets : MonoBehaviour
{
    public float disableTimer;

    private void OnEnable()
    {
        StartCoroutine(DisableAfterSec(disableTimer));
    }

    private IEnumerator DisableAfterSec(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
