using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelupCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static GenericPlayer player;
    private static GameManager gameManager;
    private static LevelUpOptions options;
    private TextMeshProUGUI[] texts;
    private Image image;
    private Animator animator;
    private string currentUpgrade;
    private int currentUpgradeValue;
    private bool missedStart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<GenericPlayer>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            options = GameObject.Find("LevelUpOptions").GetComponent<LevelUpOptions>();
        }
        
        texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        image = gameObject.GetComponentsInChildren<Image>()[3];
        animator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<Button>().onClick.AddListener(ApplySelected);

        if (missedStart)
        {
            RefreshCard();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.ResetTrigger("Hover");
        animator.SetTrigger("Normal");
    }

    public void RefreshCard()
    {
        if (!didStart)
        {
            missedStart = true;
            return;
        }

        int upgradeId = Random.Range(0, options.options.Count);
        int rarity = Random.Range(0, 100);
        rarity += player.luck;

        currentUpgrade = options.options[upgradeId].name;

        //Fejlesztesi opcio nevenek es tipusanak kiirasa
        texts[0].text = currentUpgrade;
        if (rarity >= options.legendaryChance)
        {
            currentUpgradeValue = options.options[upgradeId].legendaryValue;
            texts[0].color = options.legendaryColor;
            texts[1].color = options.legendaryColor;
        } else if (rarity >= options.rareChance)
        {
            currentUpgradeValue = options.options[upgradeId].rareValue;
            texts[0].color = options.rareColor;
            texts[1].color = options.rareColor;
        } else if (rarity >= options.uncommonChance)
        {
            currentUpgradeValue = options.options[upgradeId].uncommonValue;
            texts[0].color = options.uncommonColor;
            texts[1].color = options.uncommonColor;
        } else
        {
            currentUpgradeValue = options.options[upgradeId].commonValue;
            texts[0].color = options.commonColor;
            texts[1].color = options.commonColor;
        }
        texts[1].text = $"<b>+{currentUpgradeValue}%</b> {currentUpgrade}";

        //Fejlesztesi opciohoz tartozo kep megjelenitese
        image.sprite = options.options[upgradeId].imgSrc;

        animator.SetTrigger("Flip");
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
