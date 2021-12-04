using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using System;

public class UIServerList : Bolt.GlobalEventListener
{
    public GameObject noServerFoundText;
    public GameObject uiServerJoinButton;
    public RectTransform serverListRect;

    protected int currentPage = 0;
    protected int previousPage = 0;

    new void OnEnable()
    {
        base.OnEnable();

        currentPage = 0;
        previousPage = 0;

        foreach (Transform t in serverListRect)
            Destroy(t.gameObject);

        noServerFoundText.SetActive(false);

        RequestPage(0);
    }

    public void ChangePage(int dir)
    {
        int newPage = Mathf.Max(0, currentPage + dir);

        //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
        if (noServerFoundText.activeSelf)
            newPage = 0;

        RequestPage(newPage);
    }

    public void RequestPage(int page)
    {
        previousPage = currentPage;
        currentPage = page;
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> matches)
    {
        if (BoltNetwork.IsRunning && BoltNetwork.IsClient)
        {
            if (matches.Count == 0)
            {
                noServerFoundText.SetActive(true);
                return;
            }

            noServerFoundText.SetActive(false);
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            int i = 0;
            foreach (var pair in matches)
            {
                UdpSession udpSession = pair.Value;

                GameObject o = Instantiate(uiServerJoinButton) as GameObject;

                o.GetComponent<UIServerJoinButton>().PopulateEntry(udpSession);
                o.transform.SetParent(serverListRect, false);

                ++i;
            }
        }
    }
}
