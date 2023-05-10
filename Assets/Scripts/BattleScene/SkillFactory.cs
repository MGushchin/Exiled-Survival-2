using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory : MonoBehaviour
{
    public GameObject ThrowingSkillPrefab;

    public GameObject GetSkill(SkillList skill)
    {
        GameObject currentPrefab = Instantiate(findPrefabByList(skill));
        return currentPrefab;
    }

    private GameObject findPrefabByList(SkillList skill)
    {
        GameObject prefab = ThrowingSkillPrefab;
        switch(skill)
        {
            case (SkillList.EtherealWeapons):
                {
                    prefab = ThrowingSkillPrefab;
                }
                break;
        }
        return prefab;
    }
}
