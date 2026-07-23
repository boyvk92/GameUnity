using System;
using System.Collections.Generic;
using UnityEngine;

public class Cultivation
{
    public RealmData Realm;               // Luyện Khí, Trúc Cơ...
    public int RealmLevel;            // Tầng 1-9
    public double CultivationExp;       // Tu vi hiện tại
    public double NextBreakthroughExp;  // Tu vi cần để đột phá

    public float CultivationSpeed;    // Tốc độ tu luyện
    public float BreakthroughRate;    // Tỷ lệ thành công

    public float SpeedBonus;



    public Cultivation(){
       
        List<RealmData> Realms =
            DatabaseJsonLoader.Read<RealmData>(DataPath.RealmURL);
        Realm = Realms[0];
        Debug.Log($"Realm: {Realm.Id}, AgeStart: {Realm.AgeStart}, ExpStart: {Realm.ExpStart}, StartSpeedBonus: {Realm.StartSpeedBonus}, NextSpeedBonus: {Realm.NextSpeedBonus}, NextExp: {Realm.NextExp}, LevelUpRate: {Realm.LevelUpRate}, LevelUpFailRate: {Realm.LevelUpFailRate}");

        RealmLevel = 1;
        CultivationExp = 0;
        NextBreakthroughExp = Realm.ExpStart;
        CultivationSpeed = Realm.StartSpeedBonus;
        BreakthroughRate = 0.1f;
        SpeedBonus = Realm.StartSpeedBonus;

    }

    public double addExp(double exp){
        CultivationExp += exp;
        if(CultivationExp >= NextBreakthroughExp){
           nextLevel();
        }
        return CultivationExp;
    }

    private void nextLevel(){
        RealmLevel+=1;
        CultivationExp -= NextBreakthroughExp;
        NextBreakthroughExp = NextBreakthroughExp * Realm.NextExp;

        CultivationSpeed *= Realm.NextSpeedBonus;

        if(CultivationExp >= NextBreakthroughExp){
           nextLevel();
        }
    }
}
