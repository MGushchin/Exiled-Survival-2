using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private SkillMod baseSkillMod;
    public SkillMod BaseSkillMod => baseSkillMod;
    public int Level => BaseSkillMod.Level;
    [SerializeField]
    private string skillName;
    public string Name => skillName;
    [SerializeField]
    private Sprite skillIcon;
    public Sprite SkillIcon => skillIcon;
    public Transform SelfTransform;
    [SerializeField]
    private bool activeSkill; //�������� ���������� �� ��� �������� ������ �������� � ��������� �������
    public bool ActiveSkill => activeSkill;
    protected float cooldown = 0;
    public float Cooldown => cooldown;
    protected float skillCooldown;
    public float SkillCooldown => skillCooldown;
    protected List<StatTag> skillTags = new List<StatTag>();
    public List<StatTag> Tags => skillTags;

    protected UnitActions owner;

    public void SetBaseSkillMod(SkillMod mod)
    {
        baseSkillMod = mod;
    }

    public virtual void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
    }


    public virtual bool UseSkill(Vector3 castPoint)
    {
        return true;
    }

    public virtual void StopUseSkill()
    {

    }

    public virtual void RemoveSkill()
    {
        
    }

    public void GetUpgrades()
    {

    }

    public virtual void ApplyUpgrade(string name, int level)
    {
        Debug.Log("Upgraded " + name);
        baseSkillMod.UpgradeLevel();
    }

}