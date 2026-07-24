using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TrainItem : BasePopup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTrain(){
        int timeConLai = GameManager.Instance.getTimeManager().getTimeWeek();
        int engine = GameManager.Instance.getTimeManager().getTimeWeek();

        CultivationManager cultivationManager = new CultivationManager();
        CultivationType cultivation = cultivationManager.TrainCalculator();

         if(timeConLai < cultivation.timeUse && engine < cultivation.enginerUse){
            OnStop();
            return;
        }else{
            GameManager.Instance.useTimeWeek(cultivation.timeUse);
        }



    }

    public void OnStart(){
        // if(tuLuyenCoroutine != null){
        //     StopCoroutine(tuLuyenCoroutine);
        // }
        // tuLuyenCoroutine = StartCoroutine(CultivationLoop());
        // btn_start.gameObject.SetActive(false);
        // btn_stop.gameObject.SetActive(true);
    }

    public void OnStop(){
        // if(tuLuyenCoroutine != null){
        //     StopCoroutine(tuLuyenCoroutine);
        //     tuLuyenCoroutine = null;
        // }
        // btn_start.gameObject.SetActive(true);
        // btn_stop.gameObject.SetActive(false);
    }

    IEnumerator CultivationLoop(){
        while(true){
            yield return new WaitForSeconds(0.03f);
            OnTrain();
        }
    }
}
