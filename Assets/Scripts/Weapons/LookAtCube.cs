using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCube : Bolt.EntityBehaviour<ICubeState>
{
    public override void Attached()
    {
        state.SetTransforms(state.CubeTransform, transform);
    }

    public void SetupParent(Transform p)
    {
        transform.parent = p;
    }
}
