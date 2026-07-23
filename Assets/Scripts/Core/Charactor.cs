
using System.Collections.Generic;

public class BaseStats {
    public int Level;
    public int Exp;
    public int MaxExp;

    public int Attack;
    public int Def;
    public int Speed;
    public int Evasion;
    public int HP;
    public int MP;

    public int Gold;

    public BaseStats() { 
        Level = 1;
        Exp = 0;
        MaxExp = 100;

        Attack = 10;
        Def = 10;
        Speed = 10;
        Evasion = 10;
        HP = 100;
        MP = 100;

        Gold = 0;
    }
}


public class DerivedStats {
    public int Enginer;
    public int TimeWeekRemaining;

    public DerivedStats() { 
        Enginer = 1000;
        TimeWeekRemaining = 25200; // 7 day x 24 hours x 60 minutes
    }
}

public class GenerationStats {
    public string name;
    public int age;
    public int lifeExpectancy;
    public List<string> linhCan;
    public string thienPhu;
}

public class CharactorStats {
    public BaseStats baseStats;
    public GenerationStats generationStats;
    public DerivedStats derivedStats;
    public static CharactorStats Current;
    public Cultivation Cultivation = new();
}

