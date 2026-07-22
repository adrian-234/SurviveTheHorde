using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject levelUpContainer, pauseMenu, gameoverScreen;
    public List<GameObject> levelUpCards;
    public bool paused = false;
    public int killCounter, abilityCounter = 0;
    public int killCoinValue, LevelupCoinValue, abilityCoinValue, bossCoinValue;

    public UIManager instance;
    private GenericPlayer player;
    private DataManager dataManager;
    private GameObject xpBar, hpBar, hpBarBg, timer, powerUpIcon;
    private TextMeshProUGUI currentLevelText, damageText, mSpeedText, fRateText, rSpeedText, hpText, healText, dodgeText, luckText, xpText;
    private int elapsedTime = 0;    //Kor kezdete eltelt ido masodpercben

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        dataManager = DataManager.getInstance();

        //Kijelzo tetejen levo ui elemek
        currentLevelText = GameObject.Find("PlayerLevel").GetComponent<TextMeshProUGUI>();
        xpBar = GameObject.Find("XpProgressBar");
        hpBar = GameObject.Find("CurrentHp");
        hpBarBg = GameObject.Find("HpProgressBar");
        timer = GameObject.Find("Timer");
        powerUpIcon = GameObject.Find("PowerUpIcon");

        StartCoroutine(UpdateTimer());
    }

    void Update()
    {
        if (player)
        {
            UpdateUI();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenClosePauseMenu();
        }
    }

    public UIManager GetInstance()
    {
        return instance;
    }

    public void SetPlayer(GenericPlayer player)
    {
        this.player = player;
    }

    public void OpenClosePauseMenu()
    {
        if (levelUpContainer.activeSelf)
        {
            return;
        }

        if (!paused)
        {
            PauseGame();

            //muszaly a frissites elott megjelenitei mert kulonben a Find fuggvenyek nem talaljak meg a szovegeket es nem lehet oket frissiteni
            pauseMenu.SetActive(true);

            if (damageText == null)
            {
                GetStatsPanelTexts();
            }


            damageText.text = $"{player.GetDamageBonus()}%";
            mSpeedText.text = $"{player.GetSpeedBonus()}%";
            fRateText.text = $"{100 - player.GetFirerateBonus() + 100}%";
            rSpeedText.text = $"{100 - player.GetReloadBonus() + 100}%";
            hpText.text = $"{player.GetHpBonus()}%";
            healText.text = $"{player.GetHealBonus()}%";
            dodgeText.text = $"{player.GetDodge()}%";
            luckText.text = $"{player.GetLuck()}%";
            xpText.text = $"{player.GetXpGain() * 100}%";
        } else
        {
            UnpauseGame();
            pauseMenu.SetActive(false);
        }
    }

    //Jatekos statisztikajanak kiirasaert felelos ui elemek elmentesi kesobbi hasznalatert
    private void GetStatsPanelTexts()
    {
        damageText = GameObject.Find("Stat field").GetComponent<TextMeshProUGUI>();
        mSpeedText = GameObject.Find("Stat field (2)").GetComponent<TextMeshProUGUI>();
        fRateText = GameObject.Find("Stat field (3)").GetComponent<TextMeshProUGUI>();
        rSpeedText = GameObject.Find("Stat field (4)").GetComponent<TextMeshProUGUI>();
        hpText = GameObject.Find("Stat field (5)").GetComponent<TextMeshProUGUI>();
        healText = GameObject.Find("Stat field (6)").GetComponent<TextMeshProUGUI>();
        dodgeText = GameObject.Find("Stat field (7)").GetComponent<TextMeshProUGUI>();
        luckText = GameObject.Find("Stat field (8)").GetComponent<TextMeshProUGUI>();
        xpText = GameObject.Find("Stat field (9)").GetComponent<TextMeshProUGUI>();
    }

    void UpdateUI()
    {
        //A jateko jelenlegi eletenek megjelenitese
        float hpMaxWidth = 1.2f * player.GetHpBonus();
        float hpRatio = player.currentHp / player.GetHp();
        hpRatio = Math.Clamp(hpRatio, 0, 1);

        RectTransform rectTransform_hpBarBg = hpBarBg.GetComponent<RectTransform>();
        rectTransform_hpBarBg.sizeDelta = new Vector2(hpMaxWidth, rectTransform_hpBarBg.sizeDelta.y);
        RectTransform rectTransform_hpBar = hpBar.GetComponent<RectTransform>();
        rectTransform_hpBar.sizeDelta = new Vector2(hpMaxWidth * hpRatio, rectTransform_hpBar.sizeDelta.y);

        //A jatekos jelenlegi tapasztalati pontjainak megjelenitese
        RectTransform rectTransform_xpBar = xpBar.GetComponent<RectTransform>();
        float xpRatio = player.currentXp / player.XpNeededForLvlUp();
        rectTransform_xpBar.localScale = new Vector2(xpRatio , 1);

        //Ability toltottseg jelzes
        powerUpIcon.SetActive(!player.abilityOnCd); // TODO ez is lehetne event alapu es csak akkor lenne frissitva ha tenyleges valtozas lenne
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            int minute = elapsedTime / 60;
            int sec = elapsedTime % 60;
            
            string timerStr = "";
            if (minute < 10)
                timerStr += "0";
            timerStr += minute;

            timerStr += ":";

            if (sec < 10)
                timerStr += "0";
            timerStr += sec;

            timer.GetComponent<TextMeshProUGUI>().text = timerStr;
            elapsedTime++;
        }
    }

    public void LevelUpScreen()
    {
        PauseGame();

        //A jatekos jelenlegi szintjenek megjelenitese
        currentLevelText.text = player.currentLevel.ToString();

        int stop = levelUpCards.Count;
        if (player.GetLuck() < 20)
            stop--;

        for (int i = 0; i < stop; i++)
        {
            levelUpCards[i].GetComponent<LevelupCardController>().RefreshCard();
        }

        levelUpContainer.SetActive(true);
    }

    public void LevelUpScreenEnd()
    {
        levelUpContainer.SetActive(false);
        UnpauseGame();
    }

    public void GameoverScreen(bool isBossDead)
    {
        PauseGame();

        gameoverScreen.SetActive(true);

        GameObject.Find("KillCounterText").GetComponent<TextMeshProUGUI>().text = killCounter.ToString();
        GameObject.Find("PlayerLevelText").GetComponent<TextMeshProUGUI>().text = player.currentLevel.ToString();
        GameObject.Find("AbilityUsedText").GetComponent<TextMeshProUGUI>().text = abilityCounter.ToString();
        GameObject.Find("BossKilledText").GetComponent<TextMeshProUGUI>().text = isBossDead ? "1" : "0";

        int coinsEarned = killCounter * killCoinValue + player.currentLevel * LevelupCoinValue + abilityCounter * abilityCoinValue + (isBossDead ? bossCoinValue : 0);
        GameObject.Find("CoinsEarnedText").GetComponent<TextMeshProUGUI>().text = coinsEarned.ToString();

        dataManager.SetCoins(dataManager.GetCoins() + coinsEarned);
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        paused = true;
    }

    void UnpauseGame()
    {
        Time.timeScale = 1;
        paused = false;
    }
}
