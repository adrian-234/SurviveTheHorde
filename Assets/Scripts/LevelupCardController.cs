using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

struct upgradeType
{
    public upgradeType(string n, int cV, int uV, int rV, int lV)
    {
        name = n;
        commonValue = cV;
        uncommonValue = uV;
        rareValue = rV;
        legendaryValue = lV;
    }

    public string name;
    public int commonValue, uncommonValue, rareValue, legendaryValue;
}

[DefaultExecutionOrder(200)]
public class LevelupCardController : MonoBehaviour
{
    private static Color commonColor = new Color32(255, 255, 255, 255);
    private static Color uncommonColor = new Color32(3, 173, 0, 255);
    private static Color rareColor = new Color32(39, 83, 233, 255);
    private static Color legendaryColor = new Color32(197, 0, 0, 255);

    private static int uncommonChance = 65;
    private static int rareChance = 89;
    private static int legendaryChance = 99;
    private static List<upgradeType> upgradesList = new()
    {
        new upgradeType("Move speed", 1, 2, 3, 4),
        new upgradeType("Fire rate", 1, 2, 5, 10),
        new upgradeType("Reload speed", 2, 3, 5, 8),
        new upgradeType("Damage", 2, 3, 4, 10),
        new upgradeType("Health", 2, 4, 6, 10),
        new upgradeType("Heal", 2, 4, 6, 10),
        new upgradeType("Dodge", 1, 3, 5, 8),
        new upgradeType("Xp gain", 3, 5, 8, 10),
        new upgradeType("Luck", 2, 4, 8, 12),
    };

    private static GenericPlayer player;
    private static GameManager gameManager;

    private string currentUpgrade;
    private int currentUpgradeValue;

    private bool missedStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<GenericPlayer>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        gameObject.GetComponent<Button>().onClick.AddListener(ApplySelected);

        if (missedStart)
        {
            missedStart = false;
            RefreshCard();
        } 
    }

    public void RefreshCard()
    {
        if (player == null)
        {
            missedStart = true;
            Start();
            return;
        }

        int upgradeId = Random.Range(0, upgradesList.Count);
        int rarity = Random.Range(0, 100);
        rarity += player.luck;

        currentUpgrade = upgradesList[upgradeId].name;

        var texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = currentUpgrade;

        if (rarity >= legendaryChance)
        {
            currentUpgradeValue = upgradesList[upgradeId].legendaryValue;

            texts[0].color = legendaryColor;
            texts[1].color = legendaryColor;
        } else if (rarity >= rareChance)
        {
            currentUpgradeValue = upgradesList[upgradeId].rareValue;

            texts[0].color = rareColor;
            texts[1].color = rareColor;
        } else if (rarity >= uncommonChance)
        {
            currentUpgradeValue = upgradesList[upgradeId].uncommonValue;

            texts[0].color = uncommonColor;
            texts[1].color = uncommonColor;
        } else
        {
            currentUpgradeValue = upgradesList[upgradeId].commonValue;

            
            texts[0].color = commonColor;
            texts[1].color = commonColor;
        }

        texts[1].text = $"<b>+{currentUpgradeValue}%</b> {currentUpgrade}";
    }

    void ApplySelected()
    {
        if (currentUpgrade == null)
            return;

        switch(currentUpgrade)
        {
            case "Damage":
                player.damage_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Health":
                player.hp_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Move speed":
                player.speed_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Fire rate":
                player.firerate_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Reload speed":
                player.reload_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Heal":
                player.heal_bonus += currentUpgradeValue / 100.0f;
                break;
            case "Dodge":
                player.dodge += currentUpgradeValue;
                break;
            case "Xp gain":
                player.xpGain += currentUpgradeValue / 100.0f;
                break;
            case "Luck":
                player.luck += currentUpgradeValue;
                break;
        }

        gameManager.LevelUpScreenEnd();
    }
}
