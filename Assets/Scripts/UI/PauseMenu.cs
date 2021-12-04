using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isOn = false;
	
    // Use this for initialization
	public void LeaveRoom()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }
        SceneManager.LoadScene(0);  //Main menu
    }
}
