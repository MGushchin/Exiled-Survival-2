using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlobalData : MonoBehaviour
{
    public CharacterList PlayerCharacter;
    public SkillList PlayerSkill;
    public GameObject Player;
    public int Level = 1;
    private int skillPoints = 0;
    public int SkillPoints => skillPoints;

    public void LevelUp()
    {
        skillPoints++;
        Level++;
    }

    public bool TakeSkillPoint()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            return true;
        }
        else
            return false;
    }
}
