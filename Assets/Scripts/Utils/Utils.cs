using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	public static void SetLayerRecursively(GameObject _obj, int newLayer)
    {
        _obj.layer = newLayer;

        foreach (Transform child in _obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
            //child.gameObject.layer = newLayer;
        }
    }
}
