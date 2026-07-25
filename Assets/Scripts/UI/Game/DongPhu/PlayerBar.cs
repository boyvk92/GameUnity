using UnityEngine;

public class PlayerBar : MonoBehaviour
{
    public ProgressBar timeBar;
    public ProgressBar engineerBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.addEventCallback("time", this, _ => OnEventTime());
        GameManager.Instance.addEventCallback("energy", this, _ => OnEventEnergy());
    }

    void OnDestroy()
    {
        GameManager.Instance.removeEventCallback("time", this);
         GameManager.Instance.removeEventCallback("energy", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEventTime()
    {
        if(timeBar == null){
            return;
        }

        int currentTime = GameManager.Instance.getTimeManager().getTimeWeek();
        int getMaxTime = GameManager.Instance.getTimeManager().getTimeFullWeek();
        timeBar.setProgress(currentTime,getMaxTime);
    }
    public void OnEventEnergy()
    {
        if(timeBar == null){
            return;
        }

        int currentEnergy = GameManager.Instance.getEnergyManager().getEnergyWeek();
        int getMaxEnergy = GameManager.Instance.getEnergyManager().getEnergyWeekMax();
        engineerBar.setProgress(currentEnergy,getMaxEnergy);
    }

    
}
