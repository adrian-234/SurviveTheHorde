using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<GameObject> weapons; // Kulonbozo fegyverek prefabjei
    public List<EnemyType> enemies;     //Kulonbozo ellenfelekhez tartozo prefabek (nev - gameobject formaban)
    public List<SpawnWave> spawnMap;    //Ez a lista tarolja, hogy mikor melyik ellenfelet lehet spawnolni es mennyit. spawnStart mezo erteke szerint novekvo sorrenben kell lennie
    public float enemyScaleFactor;      //Ennyivel erosodnek az ellensegek minden perc utan
    public int mapWidth, mapHeight;

    private int elapsedTime = 0;    //Kor kezdete eltelt ido masodpercben

    private List<int> currentWaves = new(); //Ez a lista azert van, hogy konyebben es gyorsabban lehessen veletlen szeruen valasztani a lespawnolhato enemyk kozul
    private int lastWaveIndex = 0;  //Annak az elemnek az indexe a spawnMap listabol ami meg nem kerult bele a currentWaves-be
    
    private static readonly WaitForSeconds SpawnCd = new(2.5f);

    void Start()
    {
        Time.timeScale = 1; //Biztosagi okokbol, mert ha kilep a jatekos a pause menuvel kilep es vissza jon egy uj korben akkor a timeScale 0 marad amikor vissza jon

        dataManager = DataManager.getInstance();
        if (!dataManager) //csak a konnyebb es gyorsabb teszteles miatt kell ez az if
        {
            Debug.LogError("Nem sikerult beallitani a dataManagert. 0-s idk lettek ezert lespawnolva.");
            player = Instantiate(characters[0], Vector3.zero,  characters[0].transform.rotation);
            GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();
            GenericWeapon weapon = Instantiate(weapons[0], player.transform).GetComponent<GenericWeapon>();
            weapon.player = gPlayer;
            gPlayer.weapon = weapon;
        } else
        {
            player = Instantiate(characters[dataManager.selectedCharacterId], Vector3.zero,  characters[dataManager.selectedCharacterId].transform.rotation);
            GenericPlayer gPlayer = player.GetComponent<GenericPlayer>();
            GenericWeapon weapon = Instantiate(weapons[dataManager.selectedWeaponId], player.transform).GetComponent<GenericWeapon>();
            weapon.player = gPlayer;
            gPlayer.weapon = weapon;

            //permanent boostok betöltése
            PermanentUpgradesData permanentUpgradesData = PermanentUpgradesData.getInstance();
            Debug.Log($"betoltott fejlesztesek:\n\tdamage{permanentUpgradesData.GetUpgradeBoostByName("Damage")}\n\treload {permanentUpgradesData.GetUpgradeBoostByName("Reload Speed")}\n\tfirerate {permanentUpgradesData.GetUpgradeBoostByName("Firerate")}\n\thp {permanentUpgradesData.GetUpgradeBoostByName("Health")}\n\theal {permanentUpgradesData.GetUpgradeBoostByName("Heal")}\n\tmovespeed {permanentUpgradesData.GetUpgradeBoostByName("Move Speed")}");
            gPlayer.AddDamageBonus(permanentUpgradesData.GetUpgradeBoostByName("Damage"));
            gPlayer.AddReloadBonus(permanentUpgradesData.GetUpgradeBoostByName("Reload Speed"));
            gPlayer.AddFirerateBonus(permanentUpgradesData.GetUpgradeBoostByName("Firerate"));
            gPlayer.AddHpBonus(permanentUpgradesData.GetUpgradeBoostByName("Health"));
            gPlayer.AddHealBonus(permanentUpgradesData.GetUpgradeBoostByName("Heal"));
            gPlayer.AddSpeedBonus(permanentUpgradesData.GetUpgradeBoostByName("Move Speed"));
        }

        StartCoroutine(SpawnEnemies());
    }  

    IEnumerator SpawnEnemies()
    {
        while(true)
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
                        var enemy = Instantiate(enemyPrefab, getRandomPos(), enemyPrefab.transform.rotation).GetComponent<GenericEnemy>();

                        //ellensegek felerositeso az eletelt ido alapjan 
                        float multipliyer = 1 + enemyScaleFactor * (elapsedTime / 60);
                        enemy.damage *= multipliyer;
                        enemy.hp *= multipliyer;
                    }
                } else {
                    Debug.LogError($"Nincs talalat az enemy nevere. Keresett nev: {wave.name}");
                }
            }
            
            yield return SpawnCd;
        }
    }

    private Vector3 getRandomPos()
    {
        int radius = 80;    //ennyire messzire spawnolnak az enemyk a playertol
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randX = (float)Math.Cos(angle) * radius + player.transform.position.x;
        float randY = (float)Math.Sin(angle) * radius + player.transform.position.y;

        if (randX > mapWidth / -2 && randX < mapWidth / 2 && randY > mapHeight / -2 && randY < mapHeight / 2)
        {
            return new Vector3(randX, randY);
        } else
        {
            return getRandomPos();
        }
    }
}
