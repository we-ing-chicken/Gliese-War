using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RandomManager
{
    public static bool GetThisChanceResult(float Chance, int RandAccur)
    {
        if (Chance < 0.0000001f)
        {
            Chance = 0.0000001f;
        }

        bool Success = false;
        int RandAccuracy = RandAccur;
        float RandHitRange = Chance * RandAccuracy;
        int Rand = Random.Range(1, RandAccuracy + 1);
        if (Rand <= RandHitRange)
        {
            Success = true;
        }
        return Success;
    }

    public static bool GetThisChanceResult_Percentage(float Percentage_Chance, int RandAccur)
    {
        if (Percentage_Chance < 0.0000001f)
        {
            Percentage_Chance = 0.0000001f;
        }

        Percentage_Chance = Percentage_Chance / 100;

        bool Success = false;
        int RandAccuracy = RandAccur;
        float RandHitRange = Percentage_Chance * RandAccuracy;
        int Rand = Random.Range(1, RandAccuracy + 1);
        if (Rand <= RandHitRange)
        {
            Success = true;
        }
        return Success;
    }

    public static Item.ItemRank RandomBox(float RandAccur)
    {
        Item.ItemRank result = new Item.ItemRank();
        float normalTable = RandAccur * 35 / 100;
        float rareTable = RandAccur * (35 + 30) / 100;
        float epicTable = RandAccur * (35 + 30 + 19) / 100;
        float uniqueTable = RandAccur * (35 + 30 + 19 + 15) / 100;

        int Rand = Random.Range(1, (int)RandAccur + 1);

        if (Rand <= normalTable)
            result = Item.ItemRank.Normal;
        else if (normalTable < Rand && Rand <= rareTable)
            result = Item.ItemRank.Rare;
        else if (rareTable < Rand && Rand <= epicTable)
            result = Item.ItemRank.Epic;
        else if (epicTable < Rand && Rand <= uniqueTable)
            result = Item.ItemRank.Unique;
        else if (uniqueTable < Rand && Rand <= RandAccur)
            result = Item.ItemRank.Legendary;


        return result;

    }
}
