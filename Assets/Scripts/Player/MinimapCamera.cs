using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : BoltSingletonPrefab<MinimapCamera>
{
    public void Configure(BoltEntity entity)
    {
        //Debug.Log("Configure Called on player camera");
        Transform cameraPos = entity.transform.Find("MinimapPos");

        gameObject.transform.parent = cameraPos;
        gameObject.transform.position = cameraPos.position;
        gameObject.transform.rotation = cameraPos.rotation;

        //gameObject.transform.localPosition = new Vector3(0, 100, 0);
        //gameObject.transform.localRotation = Quaternion.Euler(90f, 0, 0);
    }
}
