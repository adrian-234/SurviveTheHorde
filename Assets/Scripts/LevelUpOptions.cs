using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct LevelUpOption
{
    public string name;
    public int commonValue, uncommonValue, rareValue, legendaryValue;
    public Sprite imgSrc;
}

public class LevelUpOptions : MonoBehaviour
{
    public Color commonColor;
    public Color uncommonColor;
    public Color rareColor;
    public Color legendaryColor;

    public int uncommonChance;
    public int rareChance;
    public int legendaryChance;

    public List<LevelUpOption> options;
}
