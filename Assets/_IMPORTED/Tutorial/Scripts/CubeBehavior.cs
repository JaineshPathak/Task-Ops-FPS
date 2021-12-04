/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehavior : Bolt.EntityEventListener<ICubeState>
{
    float resetColorTime;
    Renderer renderer;

    public GameObject[] weaponObjects;

    public override void Attached()
    {
        renderer = GetComponent<Renderer>();

        state.SetTransforms(state.CubeTransform, transform);

        if(entity.isOwner)
        {
            state.CubeColor = new Color(Random.value, Random.value, Random.value);

            for(int i = 0; i < state.WeaponArray.Length; ++i)
            {
                state.WeaponArray[i].WeaponId = Random.Range(0, weaponObjects.Length - 1);
                state.WeaponArray[i].WeaponAmmo = Random.Range(50, 100);
            }

            state.WeaponActiveIndex = -1;
        }

        state.AddCallback("CubeColor", ColorChanged);

        state.AddCallback("WeaponActiveIndex", WeaponActiveIndexChanged);
    }

    private void OnGUI()
    {
        if(entity.isOwner)
        {
            GUI.color = state.CubeColor;
            GUILayout.Label("@@@");
            GUI.color = Color.white;
        }
    }

    private void Update()
    {
        if(resetColorTime < Time.time)
        {
            renderer.material.color = state.CubeColor;
        }
    }

    public override void SimulateOwner()
    {
        var speed = 4f;
        var movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            movement.z += 1;
        if (Input.GetKey(KeyCode.S))
            movement.z -= 1;
        if (Input.GetKey(KeyCode.A))
            movement.x -= 1;
        if (Input.GetKey(KeyCode.D))
            movement.x += 1;

        if (Input.GetKeyDown(KeyCode.Alpha1)) state.WeaponActiveIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) state.WeaponActiveIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) state.WeaponActiveIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha0)) state.WeaponActiveIndex = -1;

        if (movement != Vector3.zero)
        {
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.frameDeltaTime);
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            var flash = FlashColorEvent.Create(entity);
            flash.FlashColor = Color.red;
            flash.Send();
        }
    }

    void ColorChanged()
    {
        renderer.material.color = state.CubeColor;
    }

    public override void OnEvent(FlashColorEvent evnt)
    {
        resetColorTime = Time.time + 0.25f;
        renderer.material.color = evnt.FlashColor;
    }

    void WeaponActiveIndexChanged()
    {
        for(int i = 0; i < weaponObjects.Length; ++i)
        {
            weaponObjects[i].SetActive(false);
        }

        if(state.WeaponActiveIndex >= 0)
        {
            int objectId = state.WeaponArray[state.WeaponActiveIndex].WeaponId;
            weaponObjects[objectId].SetActive(true);
        }
    }
}
*/