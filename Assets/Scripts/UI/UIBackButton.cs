using UnityEngine;

public class UIBackButton : MonoBehaviour
{
    public GameObject enableObject;
    public GameObject disableObject;

    public bool shutdownBolt = false;

    void Start ()
    {
        if (enableObject == null || disableObject == null)
            Debug.LogError("Please add game object to enable or disable!");
    }
	
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            disableObject.SetActive(false);
            enableObject.SetActive(true);

            if (shutdownBolt && (BoltNetwork.IsRunning))
                BoltNetwork.Shutdown();
        }
	}
}
