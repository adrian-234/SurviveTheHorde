using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockCharacter : MonoBehaviour
{
    [Serializable]
    public struct CharacterUnlockInfo {
        public string name;
        public int unlockCost;
        public Sprite characterThumbnail;
        public GameObject cardImage;
        public GameObject panel;
    }

    private class SaveData
    {
        public Dictionary<string, bool> characters = new();
    }



    public List<CharacterUnlockInfo> characterUnlockInfos;

    private static readonly string FILENAME = "/unlockedCharacters.txt";
    private DataManager dataManager;
    private SaveData saveData;

    void Start()
    {
        dataManager = DataManager.getInstance();
        
        LoadAndRefreshUI();
    }

    void LoadSaveData()
    {
        string path = Application.persistentDataPath + FILENAME;
        saveData = new SaveData();
        if (File.Exists(path))
        {
            string[] text = File.ReadAllText(path).Split(";");
            foreach(string line in text)
            {
                string[] tmp = line.Split(",");
                if (tmp.Length == 2)
                {
                    saveData.characters.Add(tmp[0], tmp[1] == "True");
                }
            }
        } else
        {
            foreach(CharacterUnlockInfo character in characterUnlockInfos)
            {
                saveData.characters.Add(character.name, false);
            }
        }
    }

    void SaveSaveData()
    {
        SaveData save;
        if (saveData == null || saveData.characters.Count == 0)
        {
            save = new SaveData();
            foreach(CharacterUnlockInfo character in characterUnlockInfos)
            {
                save.characters.Add(character.name, false);
            }
        } else
        {
            save = saveData; 
        }

        string output = "";
        foreach(var x in saveData.characters)
        {
            output += $"{x.Key},{x.Value};";
        }
        File.WriteAllText(Application.persistentDataPath + FILENAME, output);
    }

    void LoadAndRefreshUI()
    {
        LoadSaveData();

        GameObject.Find("CoinCounterText").GetComponent<TextMeshProUGUI>().text = "Coins: " + dataManager.GetCoins();

        foreach(CharacterUnlockInfo info in characterUnlockInfos)
        {
            if (saveData.characters[info.name])
            {
                Image cardImage = info.cardImage.GetComponent<Image>();
                cardImage.sprite = info.characterThumbnail;
                cardImage.color = new Color(255, 255, 255);

                Image panelImage = info.panel.transform.Find("Viewport/Content/Image - Bg/Image").GetComponent<Image>();
                panelImage.sprite = info.characterThumbnail;
                panelImage.color = new Color(255, 255, 255);

                info.panel.transform.Find("Viewport/Content/UnlockBtn - Shadow").gameObject.SetActive(false);
                info.panel.transform.Find("Viewport/Content/StartBtn - Shadow").gameObject.SetActive(true);
            }
        }
    }

    public void UnlockCharacterByName(string characterName)
    {
        foreach(CharacterUnlockInfo info in characterUnlockInfos)
        {
            if (info.name.ToLower() == characterName.ToLower() && dataManager.SpendCoins(info.unlockCost))
            {
                saveData.characters[info.name] = true;
            }
        }

        SaveSaveData();
        LoadAndRefreshUI();
    }
}
