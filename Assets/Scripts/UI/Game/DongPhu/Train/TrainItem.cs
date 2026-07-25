using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class TrainItem : BasePopup
{
    public Button btn_start;
    public Button btn_stop;
    public int currentCycle = 0;
    public int maxCycle = 10;

    public TextMeshProUGUI description;

    Coroutine trainCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.addEventCallback("stat", this, _ => updateDescription());
        updateDescription();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        GameManager.Instance.removeEventCallback("stat", this);
    }

    void Close(){
        UIManager.Instance.CloseTop();
    }

    void OnTrain(){
        int timeConLai = GameManager.Instance.getTimeManager().getTimeWeek();
        int energy = GameManager.Instance.getEnergyManager().getEnergyWeek();

        CultivationManager cultivationManager = new CultivationManager();
        TrainType cultivation = cultivationManager.TrainCalculator();

         if(timeConLai < cultivation.timeUse || energy < cultivation.energyUse){
            OnStop();
            return;
        }else{
            GameManager.Instance.useTimeWeek(cultivation.timeUse);
            GameManager.Instance.useEnergyWeek(cultivation.energyUse);
            GameManager.Instance.updateStatCharacter(cultivation.attribute);

        }
    }


    public void updateDescription(){
        description.text = "Sức mạnh: " +   GameManager.Instance.getCharactorStats().attributes.Strength;
    }



    public void OnStart(){
        if(trainCoroutine != null){
            StopCoroutine(trainCoroutine);
        }
        trainCoroutine = StartCoroutine(CultivationLoop());
        btn_start.gameObject.SetActive(false);
        btn_stop.gameObject.SetActive(true);
    }

    public void OnStop(){
        if(trainCoroutine != null){
            StopCoroutine(trainCoroutine);
            trainCoroutine = null;
        }
        btn_start.gameObject.SetActive(true);
        btn_stop.gameObject.SetActive(false);
    }

    IEnumerator CultivationLoop(){
        while(true){
            yield return new WaitForSeconds(0.03f);
            OnTrain();
        }
    }
}
