using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    [Serializable]
    private class CoinSave
    {
        public int coinAmount;

        public CoinSave(int coins)
        {
            coinAmount = coins;
        }
    }

    private static DataManager instance;
    public int selectedCharacterId;
    private int coins = 0;

    void Start()
    {
        LoadSavedCoins();
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static DataManager getInstance() {
        return instance;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetCoins(int newAmount)
    {
        if (newAmount < 0)
        {
            return;
        }

        coins = newAmount;
        SaveCoins();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= coins)
        {
            SetCoins(coins - amount);
            return true;
        }
        return false;
    }

    private void SaveCoins()
    {
        CoinSave save = new CoinSave(coins);

        string json = JsonUtility.ToJson(save);
  
        File.WriteAllText(Application.persistentDataPath + "/coins.json", json);
    }

    public void LoadSavedCoins()
    {
        string path = Application.persistentDataPath + "/coins.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CoinSave data = JsonUtility.FromJson<CoinSave>(json);

            instance.coins = data.coinAmount;
        } else
        {
            instance.coins = 0;
        }
    }
}