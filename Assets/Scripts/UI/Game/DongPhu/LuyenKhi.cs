using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class LuyenKhi : BasePopup
{
    public ProgressBar realmProgress;
    public ProgressBar cycleProgress;

    public Button btn_start;
    public Button btn_stop;
    public int currentCycle = 0;
    public int maxCycle = 10;
    public float maxRealm = 100f;

    Coroutine tuLuyenCoroutine;
    public TextMeshProUGUI cycleText;


    protected override void LoadUI(){
        base.LoadUI();
        Transform node = transform.Find("Image");
        if(node != null){
            realmProgress = node.Find("realmProgress")?.GetComponent<ProgressBar>();
            cycleProgress = node.Find("cycleProgress")?.GetComponent<ProgressBar>();
            btn_start = node.Find("btn-start")?.GetComponent<Button>();
            btn_stop = node.Find("btn-stop")?.GetComponent<Button>();
            cycleText = node.Find("canh-gioi")?.GetComponent<TextMeshProUGUI>();
        }
    }

    protected override void SetupClickEvents(){
        base.SetupClickEvents();
#if UNITY_EDITOR
        if(btn_start != null){
            UnityEventTools.RemovePersistentListener(btn_start.onClick, OnStart);
            UnityEventTools.AddPersistentListener(btn_start.onClick, OnStart);
            EditorUtility.SetDirty(btn_start);
        }

        if(btn_stop != null){
            UnityEventTools.RemovePersistentListener(btn_stop.onClick, OnStop);
            UnityEventTools.AddPersistentListener(btn_stop.onClick, OnStop);
            EditorUtility.SetDirty(btn_stop);
            btn_stop.gameObject.SetActive(false);
        }
#endif
    }

    protected override void BindRuntimeClickEvents(){
        base.BindRuntimeClickEvents();
        if(btn_start != null){
            btn_start.onClick.RemoveListener(OnStart);
            btn_start.onClick.AddListener(OnStart);
        }

        if(btn_stop != null){
            btn_stop.onClick.RemoveListener(OnStop);
            btn_stop.onClick.AddListener(OnStop);
        }

        if(realmProgress && cycleProgress){
            double currentRealm = Mathf.Round((float)GameManager.Instance.getCharactorStats().Cultivation.CultivationExp);
            double nextRealm = Mathf.Round((float)GameManager.Instance.getCharactorStats().Cultivation.NextBreakthroughExp);
            
            realmProgress.setProgress(currentRealm, nextRealm);
            cycleProgress.setProgress(currentCycle, maxCycle);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Open(){
        UpdateUI();
    }

    public override void Close()
    {
        OnStop();
        gameObject.SetActive(false);
    }

    public void UpdateUI(){
        double currentRealm = Mathf.Round((float)GameManager.Instance.getCharactorStats().Cultivation.CultivationExp);
        double nextRealm = Mathf.Round((float)GameManager.Instance.getCharactorStats().Cultivation.NextBreakthroughExp);
        
        realmProgress.setProgress(currentRealm, nextRealm);
        cycleProgress.setProgress(currentCycle, maxCycle);

        RealmData realm = GameManager.Instance.getCharactorStats().Cultivation.Realm;
        int level = GameManager.Instance.getCharactorStats().Cultivation.RealmLevel;

        cycleText.text = realm.Id + " - " + level;
    }

    void OnCultivate(){
        int timeConLai = GameManager.Instance.getTimeManager().getTimeWeek();
        //20*4
        CultivationManager cultivationManager = new CultivationManager();
        CultivationType cultivation = cultivationManager.CultivationCalculator();

        if(timeConLai < cultivation.timeUse){
            OnStop();
            return;
        }else{
            GameManager.Instance.useTimeWeek(cultivation.timeUse);
        }

        if(currentCycle >= maxCycle){
            currentCycle = 0;
        }

        currentCycle += 1;
        GameManager.Instance.getCharactorStats().Cultivation.addExp(cultivation.realm);
       
        UpdateUI();
    }

    public void OnStart(){
        if(tuLuyenCoroutine != null){
            StopCoroutine(tuLuyenCoroutine);
        }
        tuLuyenCoroutine = StartCoroutine(CultivationLoop());
        btn_start.gameObject.SetActive(false);
        btn_stop.gameObject.SetActive(true);
        
    }

    public void OnStop(){
        if(tuLuyenCoroutine != null){
            StopCoroutine(tuLuyenCoroutine);
            tuLuyenCoroutine = null;
        }
        btn_start.gameObject.SetActive(true);
        btn_stop.gameObject.SetActive(false);
    }

    IEnumerator CultivationLoop(){
        while(true){
            yield return new WaitForSeconds(0.03f);
            OnCultivate();
        }
    }
}
