using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private DataManager dataManager;
    public List<GameObject> characters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataManager = DataManager.getInstance();
        if (!dataManager)
        {
            Debug.Log("Nem sikerult beallitani a dataManagert. 0-s id lett ezert lespawnolva.");
            Instantiate(characters[0], Vector3.zero,  characters[0].transform.rotation);
        }

        Instantiate(characters[dataManager.selectedCharacterId], Vector3.zero,  characters[dataManager.selectedCharacterId].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies() {throw new NotImplementedException();}
}
