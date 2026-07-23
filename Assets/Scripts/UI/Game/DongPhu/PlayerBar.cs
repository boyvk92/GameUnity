using UnityEngine;

public class PlayerBar : MonoBehaviour
{
    public ProgressBar timeBar;
    public ProgressBar engineerBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.addEventCallback("time", this, _ => OnEventTime());
    }

    void OnDestroy()
    {
        GameManager.Instance.removeEventCallback("time", this);
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
}
