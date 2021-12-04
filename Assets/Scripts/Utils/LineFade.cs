using System.Collections;
using UnityEngine;

public class LineFade : MonoBehaviour
{
    private LineRenderer line;

    [SerializeField] private Color color;
    [SerializeField] private float disableTimer = 4f;

	void Start ()
    {
        line = GetComponent<LineRenderer>();	
	}

    void Update()
    {
        color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * 10f);

        line.startColor = color;
        line.endColor = color;
    }

    private void OnEnable()
    {
        color.a = 255f;
        StartCoroutine(DisableAfterSec(disableTimer));
    }

    private IEnumerator DisableAfterSec(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
