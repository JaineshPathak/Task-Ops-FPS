using UnityEngine;
using UnityEngine.UI;
using UdpKit;
using Bolt.photon;
using Bolt;
using BoltInternal;
using Bolt.Utils;
using udpkit.platform.photon.photon;
using Bolt.Tokens;

public class UIServerJoinButton : MonoBehaviour
{
    public Text serverNameText;
    public Text slotNumberText;
    public Text sceneMapText;

    public Button theJoinButton;

    public void PopulateEntry(UdpSession match)
    {
        ServerRoomToken token = match.GetProtocolToken() as ServerRoomToken;

        serverNameText.text = match.HostName;

        int currentPlayers = match.ConnectionsCurrent - 1;

        slotNumberText.text = currentPlayers.ToString() + "/" + match.ConnectionsMax.ToString();

        if (token != null)
            sceneMapText.text = token.activeSceneName;

        theJoinButton.onClick.RemoveAllListeners();
        theJoinButton.onClick.AddListener(() => { JoinMatch(match); });
    }

    public void JoinMatch(UdpSession match)
    {
        BoltNetwork.Connect(match);
    }
}
