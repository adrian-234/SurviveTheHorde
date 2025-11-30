using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private DataManager dataManager;
    public List<GameObject> characters;
    public GameObject player;
    public List<GameObject> enemies;
    public bool isGameActive = true;

    private TextMeshProUGUI currentLevelText;
    private GameObject xpBar, hpBar, hpBarBg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataManager = DataManager.getInstance();
        if (!dataManager) //csak a konnyebb es gyorsabb teszteles miatt kell ez az if
        {
            Debug.LogError("Nem sikerult beallitani a dataManagert. 0-s id lett ezert lespawnolva.");
            player = Instantiate(characters[0], Vector3.zero,  characters[0].transform.rotation);
        } else
        {
            player = Instantiate(characters[dataManager.selectedCharacterId], Vector3.zero,  characters[dataManager.selectedCharacterId].transform.rotation);
        }

        StartCoroutine(SpawnEnemies());


        currentLevelText = GameObject.Find("PlayerLevel").GetComponent<TextMeshProUGUI>();
        xpBar = GameObject.Find("XpProgressBar");
        hpBar = GameObject.Find("CurrentHp");
        hpBarBg = GameObject.Find("HpProgressBar");
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    IEnumerator SpawnEnemies()
    {
        while(isGameActive)
        {
            int radius = 80;
            int randId = UnityEngine.Random.Range(0, enemies.Count);
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randX = (float)Math.Cos(angle) * radius;
            float randY = (float)Math.Sin(angle) * radius;
            
            Vector3 randPos = new Vector3(randX, randY);
            Instantiate(enemies[randId], player.transform.position + randPos, enemies[randId].transform.rotation);
            
            
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

        //A jatekos jelenlegi tapasztalati pontjainek megjelenitese
        RectTransform rectTransform_xpBar = xpBar.GetComponent<RectTransform>();
        float xpRatio = gPlayer.xp / (gPlayer.currentLevel * 2);
        rectTransform_xpBar.localScale = new Vector2(xpRatio , 1);
    }
}
