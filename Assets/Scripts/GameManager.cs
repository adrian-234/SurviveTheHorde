using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct EnemyType //Ilyen EnemyType okkal vannak letarolva a kulonbozo ellenfelek azert, hogy ne kellejen mindig megadni a prefabet, hozzajuk csak a nevekut
{
    public string name;
    public GameObject prefab;
}

[Serializable]
public struct SpawnWave //Azt tarolja, hogy egy ellenfel tipust mikortol es meddig lehet spawnolni es mennyit lehet belole letenni egyszerre
{
    public string name; //az enemy neve amit spawnolni kell ennek meg kell egyeznie egy elem nevevel az enemies listabol
    public int spawnStart, spawnEnd, minCount, maxCount;
}

public class GameManager : MonoBehaviour
{
    private DataManager dataManager;
    private GameObject player;
    public List<GameObject> characters; //Kulonbozo jatekos karakerek prefabjei
    public List<EnemyType> enemies;     //Kulonbozo ellenfelekhez tartozo prefabek (nev - gameobject formaban)
    public List<SpawnWave> spawnMap;    //Ez a lista tarolja, hogy mikor melyik ellenfelet lehet spawnolni es mennyit. spawnStart mezo erteke szerint novekvo sorrenben kell lennie
    public float enemyScaleFactor;      //Ennyivel erosodnek az ellensegek minden perc utan
    public bool isGameActive = true;
    public bool paused = false;
    public GameObject levelUpContainer, pauseMenu, gameoverScreen;
    public List<GameObject> levelUpCards;
    public int killCoinValue, LevelupCoinValue, abilityCoinValue, bossCoinValue;

    private GameObject xpBar, hpBar, hpBarBg, timer, powerUpIcon;
    private TextMeshProUGUI currentLevelText, damageText, mSpeedText, fRateText, rSpeedText, hpText, healText, dodgeText, luckText, xpText;
    private int elapsedTime = 0;    //Kor kezdete eltelt ido masodpercben
    public int killCounter, abilityCounter = 0;

    private List<int> currentWaves = new(); //Ez a lista azert van, hogy konyebben es gyorsabban lehessen veletlen szeruen valasztani a lespawnolhato enemyk kozul
    private int lastWaveIndex = 0;  //Annak az elemnek az indexe a spawnMap listabol ami meg nem kerult bele a currentWaves-be

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1; //Biztosagi okokbol, mert ha kilep a jatekos a pause menuvel kilep es vissza jon egy uj korben akkor a timeScale 0 marad amikor vissza jon

        dataManager = DataManager.getInstance();
        if (!dataManager) //csak a konnyebb es gyorsabb teszteles miatt kell ez az if
        {
            Debug.LogError("Nem sikerult beallitani a dataManagert. 0-s id lett ezert lespawnolva.");
            player = Instantiate(characters[0], Vector3.zero,  characters[0].transform.rotation);
        } else
        {
            player = Instantiate(characters[dataManager.selectedCharacterId], Vector3.zero,  characters[dataManager.selectedCharacterId].transform.rotation);
            //permanent boostok betöltése
            PermanentUpgradesData permanentUpgradesData = PermanentUpgradesData.getInstance();
            GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();
            gPlayer.damage_bonus = permanentUpgradesData.GetUpgradeBoostByName("Damage");
            gPlayer.reload_bonus = permanentUpgradesData.GetUpgradeBoostByName("Reload Speed");
        }

        //Kijelzo tetejen levo ui elemek
        currentLevelText = GameObject.Find("PlayerLevel").GetComponent<TextMeshProUGUI>();
        xpBar = GameObject.Find("XpProgressBar");
        hpBar = GameObject.Find("CurrentHp");
        hpBarBg = GameObject.Find("HpProgressBar");
        timer = GameObject.Find("Timer");
        powerUpIcon = GameObject.Find("PowerUpIcon");

        UpdateUI();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(UpdateTimer());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenClosePauseMenu();
        }
    }

    public void OpenClosePauseMenu()
    {
        if (!paused)
        {
            PauseGame();

            //muszaly a frissites elott megjelenitei mert kulonben a Find fuggvenyek nem talaljak meg a szovegeket es nem lehet oket frissiteni
            pauseMenu.SetActive(true);

            if (damageText == null)
            {
                GetStatsPanelTexts();
            }

            //update stat panel
            GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();

            damageText.text = $"{(1 + gPlayer.damage_bonus) * 100}%";
            mSpeedText.text = $"{(1 + gPlayer.speed_bonus) * 100}%";
            fRateText.text = $"{(1 + gPlayer.firerate_bonus) * 100}%";
            rSpeedText.text = $"{(1 + gPlayer.reload_bonus) * 100}%";
            hpText.text = $"{(1 + gPlayer.hp_bonus) * 100}%";
            healText.text = $"{(1 + gPlayer.heal_bonus) * 100}%";
            dodgeText.text = $"{gPlayer.dodge}%";
            luckText.text = $"{gPlayer.luck}%";
            xpText.text = $"{gPlayer.xpGain * 100}%";
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

    IEnumerator SpawnEnemies()
    {
        while(isGameActive)
        {
            //a currentWaves lista frissetese
            currentWaves.RemoveAll(i => spawnMap[i].spawnEnd <= elapsedTime);
            while (lastWaveIndex < spawnMap.Count && spawnMap[lastWaveIndex].spawnStart <= elapsedTime) {
                currentWaves.Add(lastWaveIndex);
                lastWaveIndex++;
            }

            if (currentWaves.Count > 0) {
                //random enemy kisorsolasa
                int randId = currentWaves[UnityEngine.Random.Range(0, currentWaves.Count)];
                SpawnWave wave = spawnMap[randId];
                GameObject enemyPrefab = enemies.Find(e => e.name == wave.name).prefab;
                int randEnemyCount = UnityEngine.Random.Range(wave.minCount, wave.maxCount + 1);

                if (enemyPrefab != null) {
                    //tenyleges spawnolas
                    for (int i = 0; i < randEnemyCount; i++) {
                        int radius = 80;
                        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                        float randX = (float)Math.Cos(angle) * radius;
                        float randY = (float)Math.Sin(angle) * radius;
                        
                        Vector3 randPos = new Vector3(randX, randY);
                        var enemy = Instantiate(enemyPrefab, player.transform.position + randPos, enemyPrefab.transform.rotation).GetComponent<GenericEnemy>();

                        //ellensegek felerositeso az eletelt ido alapjan 
                        float multipliyer = 1 + enemyScaleFactor * (elapsedTime / 60);
                        enemy.damage *= multipliyer;
                        enemy.hp *= multipliyer;
                    }
                } else {
                    Debug.LogError($"Nincs talalat az enemy nevere. Keresett nev: {wave.name}");
                }
            }
            
            yield return new WaitForSeconds(2.5f);
        }
    }

    void UpdateUI()
    {
        GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();

        //A jatekos jelenlegi szintjenek megjelenitese
        currentLevelText.text = gPlayer.currentLevel.ToString();

        //A jateko jelenlegi eletenek megjelenitese
        float hpMaxWidth = 120f * (1 + gPlayer.hp_bonus);
        float hpRatio = gPlayer.currentHp / (gPlayer.hp_base * (1 + gPlayer.hp_bonus));
        hpRatio = Math.Clamp(hpRatio, 0, 1);

        RectTransform rectTransform_hpBarBg = hpBarBg.GetComponent<RectTransform>();
        rectTransform_hpBarBg.sizeDelta = new Vector2(hpMaxWidth, rectTransform_hpBarBg.sizeDelta.y);
        RectTransform rectTransform_hpBar = hpBar.GetComponent<RectTransform>();
        rectTransform_hpBar.sizeDelta = new Vector2(hpMaxWidth * hpRatio, rectTransform_hpBar.sizeDelta.y);

        //A jatekos jelenlegi tapasztalati pontjainak megjelenitese
        RectTransform rectTransform_xpBar = xpBar.GetComponent<RectTransform>();
        float xpRatio = gPlayer.xp / (gPlayer.currentLevel * 2);
        rectTransform_xpBar.localScale = new Vector2(xpRatio , 1);

        //Ability toltottseg jelzes
        powerUpIcon.SetActive(!gPlayer.abilityOnCd);
    }

    IEnumerator UpdateTimer()
    {
        while (isGameActive)
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

        GenericPlayer player = this.player.GetComponent<GenericPlayer>();

        int stop = levelUpCards.Count;
        if (player.luck < 20)
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

    public void GameoverScreen(bool isBossDead)
    {
        PauseGame();

        gameoverScreen.SetActive(true);

        GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();

        GameObject.Find("KillCounterText").GetComponent<TextMeshProUGUI>().text = killCounter.ToString();
        GameObject.Find("PlayerLevelText").GetComponent<TextMeshProUGUI>().text = gPlayer.currentLevel.ToString();
        GameObject.Find("AbilityUsedText").GetComponent<TextMeshProUGUI>().text = abilityCounter.ToString();
        GameObject.Find("BossKilledText").GetComponent<TextMeshProUGUI>().text = isBossDead ? "1" : "0";

        int coinsEarned = killCounter * killCoinValue + gPlayer.currentLevel * LevelupCoinValue + abilityCounter * abilityCoinValue + (isBossDead ? bossCoinValue : 0);
        GameObject.Find("CoinsEarnedText").GetComponent<TextMeshProUGUI>().text = coinsEarned.ToString();

        dataManager.SetCoins(dataManager.GetCoins() + coinsEarned);
    }
}
