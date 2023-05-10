using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Statuses;

public class StatusDisplay : MonoBehaviour
{
    public GameObject IconPrefab;
    public List<StatusIcon> Icons = new List<StatusIcon>();
    public Sprite PoisonImage;
    public Sprite DivineAuraImage;
    public Sprite DefaultImage;
    public Sprite BleedingImage;

    public void UpdateIcons(List<StatusType> statusType, List<string> statusValues)// Возможно избежать строк
    {
        compareIconsCount(statusType.Count);
        int counter = 0;
        for(int i=0; i < statusType.Count && i < statusValues.Count && i < Icons.Count; i++, counter++)
        {
            Sprite displayedImage = DefaultImage;
            switch(statusType[0])
            {
                case(StatusType.Poison):
                    {
                        displayedImage = PoisonImage;
                    }
                    break;
                case (StatusType.Bleeding):
                    {
                        displayedImage = BleedingImage;
                    }
                    break;
                case (StatusType.DivineAura):
                    {
                        displayedImage = DivineAuraImage;
                    }
                    break;
                default:
                    {
                        displayedImage = DefaultImage;
                    }
                    break;
            }
            Icons[i].gameObject.SetActive(true);
            Icons[i].SetIcon(displayedImage, statusValues[i]);
        }
        if(counter < Icons.Count)
        {
            for (int i = counter; i < Icons.Count; i++)
                Icons[i].gameObject.SetActive(false);
        }
    }

    private void compareIconsCount(int requirementIcons)
    {
        while(Icons.Count < requirementIcons)
        {
            StatusIcon statusIcon = Instantiate(IconPrefab, transform).GetComponent<StatusIcon>();
            Icons.Add(statusIcon);
        }
    }
}
