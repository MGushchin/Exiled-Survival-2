using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rune Item Data", menuName = "ScriptableObjects/Rune Item Data", order = 1)]
public class RuneItemData : ScriptableObject
{
    public string Name => name;

    //[SerializeField]
    //private Sprite image;
    //public Sprite Image => image;

    [SerializeField]
    private Rarities rarity;
    public Rarities Rarities => rarity;

    public List<Affix> positiveAffixes = new List<Affix>();

    public List<Affix> negativeAffixes = new List<Affix>();

    public void Init(string name, List<Affix> positiveAffixes, List<Affix> negativeAffixes)
    {
        this.name = name;
        this.positiveAffixes = positiveAffixes;
        this.negativeAffixes = negativeAffixes;
    }

}
