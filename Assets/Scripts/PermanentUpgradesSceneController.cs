using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PermanentUpgradesSceneController : MonoBehaviour
{
    private DataManager dataManager;
    private PermanentUpgradesData upgradesData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataManager = DataManager.getInstance();
        upgradesData = PermanentUpgradesData.getInstance();

        if (!dataManager || !upgradesData)
        {
            Debug.LogError("Nem sikerult beallitani valamelyik data kezelot.");
        } else
        {
            LoadUI();
        }
    }

    void LoadUI()
    {
        GameObject.Find("CoinCounterText").GetComponent<TextMeshProUGUI>().text = "Coins: " + dataManager.GetCoins();

        foreach (GameObject card in GameObject.FindGameObjectsWithTag("PermanentUpgradeCard")) {
            string name = card.GetComponentInChildren<TextMeshProUGUI>().text;
            Debug.Log("card name: " + name);

            int upgradeLevel = upgradesData.GetUpgradeLevelByName(name);
            
            Transform indicators = card.transform.Find("Indicators");
            for (int i = 0; i < upgradeLevel && i < indicators.childCount; i++)
            {
                indicators.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }

            Transform btn = card.transform.Find("Button");
            if (upgradeLevel == 3)
            {
                btn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX";
            } else
            {
                PermanentUpgrade upgrade = upgradesData.GetUpgradeByName(name);
                if (upgrade.levelCosts != null)
                {
                    btn.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgrade.levelCosts[upgradeLevel] + " coin";
                } else
                {
                    Debug.LogError("Nem talalt upgrade tipus: " + name);
                }
            }

            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => BuyUpgrade(name));
        }
    }

    public void BuyUpgrade(string name)
    {
        Debug.Log("Vasarlas");
        int upgradeLevel = upgradesData.GetUpgradeLevelByName(name);

        if (upgradeLevel >= 3)
        {
            return;
        }

        int upgradeCost = upgradesData.GetUpgradeByName(name).levelCosts[upgradeLevel];
        if (upgradesData.GetUpgradeLevelByName(name) < 3 && dataManager.GetCoins() >= upgradeCost)
        {
            switch(name) {
                case "Damage":
                    upgradesData.damageUpgradeLevel += 1;
                    break;
                case "Reload Speed":
                    upgradesData.reloadSpeedUpgradeLevel += 1;
                    break;
                case "Firerate":
                    upgradesData.firerateUpgradeLevel += 1;
                    break;
                case "Health":
                    upgradesData.healthUpgradeLevel += 1;
                    break;
                case "Heal":
                    upgradesData.healUpgradeLevel += 1;
                    break;
                case "Move Speed":
                    upgradesData.moveSpeedUpgradeLevel += 1;
                    break;
            }

            upgradesData.SavePermanentUpgrades();
            dataManager.SetCoins(dataManager.GetCoins() - upgradeCost);
            LoadUI();

            Debug.Log("Upgrade megvasrolva nev: " + name + "     ar: " + upgradeCost);
        }
    }
}
