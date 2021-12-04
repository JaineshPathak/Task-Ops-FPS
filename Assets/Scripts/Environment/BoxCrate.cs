using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class BoxCrate : Bolt.EntityBehaviour<IBoxCrateState>
{
    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }
}
