using System.Collections.Generic;
using UnityEngine;

public class PlayerUIModelSpawner : MonoBehaviour
{
    public List<GameObject> uiPlayerModelPrefabs = new List<GameObject>();
    private List<GameObject> uiPlayerModelPrefabsInstance;
    public Transform parentObject;

    private int modelID;

	// Use this for initialization
	void Start ()
    {
        if (uiPlayerModelPrefabs.Count != 0)
        {
            uiPlayerModelPrefabsInstance = new List<GameObject>(new GameObject[uiPlayerModelPrefabs.Count]);
            SpawnModels();
        }
	}

    void SpawnModels()
    {
        for(int i = 0; i < uiPlayerModelPrefabs.Count; i++)
            uiPlayerModelPrefabsInstance[i] = Instantiate(uiPlayerModelPrefabs[i], transform.position, transform.rotation, parentObject);

        LoadCurrentModel();
    }

    void LoadCurrentModel()
    {
        for (int i = 0; i < uiPlayerModelPrefabs.Count; i++)
            uiPlayerModelPrefabsInstance[i].SetActive(false);

        if (PlayerPrefs.HasKey("PlayerModelID"))
            modelID = PlayerPrefs.GetInt("PlayerModelID");

        uiPlayerModelPrefabsInstance[modelID].SetActive(true);
    }

    public void SetNewPlayerModel(int val)
    {
        for (int i = 0; i < uiPlayerModelPrefabs.Count; i++)
            uiPlayerModelPrefabsInstance[i].SetActive(false);

        PlayerPrefs.SetInt("PlayerModelID", val);

        uiPlayerModelPrefabsInstance[val].SetActive(true);
    }
}
