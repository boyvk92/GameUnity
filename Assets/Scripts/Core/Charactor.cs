
using System.Collections.Generic;

public class BaseStats {
    public int Level;
    public int Exp;
    public int MaxExp;

    public int Gold;

    public BaseStats() { 

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

    public static CharactorStats Current;

    public BaseStats baseStats;
    public CharactorAttributesType attributes;
    public GenerationStats generationStats;
    public DerivedStats derivedStats;
    
    public Cultivation Cultivation;

    public CharactorStats(){
        baseStats = new BaseStats();
        attributes = new CharactorAttributesType();
        generationStats = new GenerationStats();
        derivedStats = new DerivedStats();
        Cultivation = new Cultivation();
    }

    public void AddStat(CharactorAttributesType _attributes){
        attributes.AddStat(_attributes);
    }
}

