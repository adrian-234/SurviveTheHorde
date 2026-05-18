using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.AI;

[Serializable]
    public struct PermanentUpgrade
    {
        public string name;
        public List<int> levelCosts;
        public List<int> upgradeValues;
    }

public class PermanentUpgradesData : MonoBehaviour
{
    [Serializable]
    private class PermanentUpgradesSave
    {
        public int damage, reloadSpeed, moveSpeed, health, heal, firerate = 0;

        public PermanentUpgradesSave(int damage, int reloadSpeed, int moveSpeed, int health, int heal, int firerate)
        {
            this.damage = damage;
            this.reloadSpeed = reloadSpeed;
            this.moveSpeed = moveSpeed;
            this.health = health;
            this.heal = heal;
            this.firerate = firerate;
        }
    }

    private static PermanentUpgradesData instance;

    public List<PermanentUpgrade> permanentUpgrades;
    public int damageUpgradeLevel, reloadSpeedUpgradeLevel, moveSpeedUpgradeLevel, healthUpgradeLevel, healUpgradeLevel, firerateUpgradeLevel = 0;

    void Start()
    {
        LoadPermanentUpgrades();
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

    public static PermanentUpgradesData getInstance() {
        return instance;
    }

    public void SavePermanentUpgrades()
    {
        PermanentUpgradesSave save = new PermanentUpgradesSave(damageUpgradeLevel, reloadSpeedUpgradeLevel, moveSpeedUpgradeLevel, healthUpgradeLevel, healUpgradeLevel, firerateUpgradeLevel);

        string json = JsonUtility.ToJson(save);
  
        File.WriteAllText(Application.persistentDataPath + "/upgrades.json", json);
    }

    public void LoadPermanentUpgrades()
    {
        string path = Application.persistentDataPath + "/upgrades.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PermanentUpgradesSave data = JsonUtility.FromJson<PermanentUpgradesSave>(json);

            instance.damageUpgradeLevel = data.damage;
            instance.reloadSpeedUpgradeLevel = data.reloadSpeed;
            instance.moveSpeedUpgradeLevel = data.moveSpeed;
            instance.healthUpgradeLevel = data.health;
            instance.healUpgradeLevel = data.heal;
            instance.firerateUpgradeLevel = data.firerate;
        }
    }

    public int GetUpgradeLevelByName(string name)
    {
        switch(name) {
                case "Damage":
                    return damageUpgradeLevel;
                case "Reload Speed":
                    return reloadSpeedUpgradeLevel;
                case "Firerate":
                    return firerateUpgradeLevel;
                case "Health":
                    return healthUpgradeLevel;
                case "Heal":
                    return healUpgradeLevel;
                case "Move Speed":
                    return moveSpeedUpgradeLevel;
                default:
                    return 0;
            }
    }

    public PermanentUpgrade GetUpgradeByName(string name)
    {
        foreach(PermanentUpgrade u in permanentUpgrades)
        {
            if (u.name == name)
            {
                return u;
            }
        }
        return new PermanentUpgrade();
    }

    public float GetUpgradeBoostByName(string name)
    {
        PermanentUpgrade upgrade = GetUpgradeByName(name);
        if (upgrade.upgradeValues == null)
        {
            return 0.0f;
        }
        return upgrade.upgradeValues[GetUpgradeLevelByName(name) - 1] / 100.0f;
    }
}
