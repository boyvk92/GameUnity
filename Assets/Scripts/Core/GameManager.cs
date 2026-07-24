using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class GameManager {
    private static GameManager instance;

    private CharactorStats charactor;

    private TimeManager timeManager;
    private EnergyManager energyManager;


    private EventCallBackManager eventCallback;

    private GameManager(CharactorStats _charactor){
        this.charactor = _charactor;
        this.timeManager = new TimeManager();
        this.enginerManager = new EnginerManager();
        eventCallback = new EventCallBackManager();
    }

    public static GameManager Instance{
        get{
            if(instance == null){
                instance = new GameManager(null);
            }

            return instance;
        }
    }
    
    public void newGame(){
        this.timeManager = new TimeManager();
    }

    public CharactorStats getCharactorStats(){
        return charactor;
    }

    public TimeManager getTimeManager(){
        return timeManager;
    }

    public TimeManager getEnergyManager(){
        return energyManager;
    }

    

    public void setCharactorStats(CharactorStats _charactor){
        charactor = _charactor;
    }

    // time manager
    public void newWeek(){
        this.timeManager.nextWeek();
        eventCallback.trigger("time");
    }
    public void useTimeWeek(int timeUse){
        timeManager.useTimeWeek(timeUse);
        eventCallback.trigger("time");
    }


    //event callback
    public void addEventCallback(string key, UnityObject owner, Action<object> func){
        eventCallback.subscribeEvent(key, owner, func);
    }

    public void removeEventCallback(string key, UnityObject owner){
        eventCallback.removeEvent(key, owner);
    }


    //test
    public void test(){
        timeManager.useTimeWeek(10);
        eventCallback.trigger("time");
    }
}
