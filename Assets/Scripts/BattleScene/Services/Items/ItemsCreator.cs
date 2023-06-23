using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ItemsCreator : MonoBehaviour
{
    public RuneItemData commonItemPrefab;
    public RuneItemData magicItemPrefab;
    public RuneItemData rareItemPrefab;

    public List<Affix> CommonModsPerOneMagnitude = new List<Affix>();
    public List<Affix> MagicModsPerOneMagnitude = new List<Affix>();
    public List<Affix> RareModsPerOneMagnitude = new List<Affix>();

    //Rules
    private float commonItemPositiveBonusMagnitudeMultiplier = 5;
    private float commonItemNegativeBonusMagnitudeMultiplier = 0;
    private float magicItemPositiveBonusMagnitudeMultiplier = 10;
    private float magicItemNegativeBonusMagnitudeMultiplier = 5;
    private float rareItemPositiveBonusMagnitudeMultiplier = 20;
    private float rareItemNegativeBonusMagnitudeMultiplier = 20;

    public void GetItemMods(int count, float magnitude, Rarities rarity)
    {
        List<RuneItemData> items = new List<RuneItemData>();
        List<Affix> possibleMods = new List<Affix>();

        for (int i=0; i < count; i++)
        {
            RuneItemData item;
            float positiveModifierWithRarityMod = magnitude;
            float negativemodifierWithRarityMod = magnitude;
            possibleMods.AddRange(CommonModsPerOneMagnitude);
            switch (rarity)
            {
                case (Rarities.Common):
                    {
                        item = Instantiate(commonItemPrefab);
                        positiveModifierWithRarityMod = commonItemPositiveBonusMagnitudeMultiplier;
                        negativemodifierWithRarityMod = commonItemNegativeBonusMagnitudeMultiplier;
                    }
                    break;
                case (Rarities.Magic):
                    {
                        item = Instantiate(magicItemPrefab);
                        possibleMods.AddRange(MagicModsPerOneMagnitude);
                        positiveModifierWithRarityMod = magicItemPositiveBonusMagnitudeMultiplier;
                        negativemodifierWithRarityMod = magicItemNegativeBonusMagnitudeMultiplier;
                    }
                    break;
                case (Rarities.Rare):
                    {
                        item = Instantiate(rareItemPrefab);
                        possibleMods.AddRange(MagicModsPerOneMagnitude);
                        possibleMods.AddRange(RareModsPerOneMagnitude);
                        positiveModifierWithRarityMod = rareItemPositiveBonusMagnitudeMultiplier;
                        negativemodifierWithRarityMod = rareItemNegativeBonusMagnitudeMultiplier;
                    }
                    break;
                default:
                    {
                        item = Instantiate(commonItemPrefab);
                        Debug.LogError("Default exception");
                    }break;
            }

            Affix randomPositiveAffix = possibleMods[Random.Range(0, possibleMods.Count)];
            List<Affix> positiveAffixes = new List<Affix>();
            positiveAffixes.Add(new Affix(randomPositiveAffix.Tag, randomPositiveAffix.ModType, randomPositiveAffix.Value * magnitude * positiveModifierWithRarityMod));
            possibleMods.Remove(positiveAffixes[0]);

            Affix randomNegativeAffix = possibleMods[Random.Range(0, possibleMods.Count)];
            List<Affix> negativeAffixes = new List<Affix> ();
            if (negativemodifierWithRarityMod > 0)
            {
                negativeAffixes.Add(new Affix(randomNegativeAffix.Tag, randomNegativeAffix.ModType, randomNegativeAffix.Value * magnitude * negativemodifierWithRarityMod));
                possibleMods.Remove(negativeAffixes[0]);
            }

            positiveAffixes.Add(randomPositiveAffix);
            negativeAffixes.Add(randomNegativeAffix);

            item.Init(positiveAffixes[0].Tag.ToString(), positiveAffixes, negativeAffixes);
        }
    }
}
