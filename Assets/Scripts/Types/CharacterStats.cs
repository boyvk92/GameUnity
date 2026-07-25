using System.Reflection;

public class CharactorAttributesType
{
    public int Strength;
    public int Def;
    public int Speed;
    public int Evasion;
    public int HP;
    public int MP;

    public CharactorAttributesType AddStat(CharactorAttributesType _attributes){
        var fields = typeof(CharactorAttributesType).GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType != typeof(int)) continue;

            var currentValue = (int)field.GetValue(this);
            var addValue = (int)field.GetValue(_attributes);
            field.SetValue(this, currentValue + addValue);
        }
        return this;
    }
}

public class CharactorLifeSkillsTyle
{
    public int Strength;
    public int Def;
    public int Speed;
    public int Evasion;
    public int HP;
    public int MP;
}

public class CharactorCombatSkillsTyle
{
    public int Attack;
    public int Def;
    public int Speed;
    public int Evasion;
    public int HP;
    public int MP;
}

public class CharactorTechniqueTyle
{
    public int Attack;
    public int Def;
    public int Speed;
    public int Evasion;
    public int HP;
    public int MP;
}
