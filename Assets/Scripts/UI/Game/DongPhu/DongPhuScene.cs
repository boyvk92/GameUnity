using UnityEngine;
using TMPro;

public class DongPhuScene : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public GameObject luyenKhiPopupPrefab;
    public GameObject trainPopupPrefab;
    public Transform popupRoot;
    LuyenKhi luyenKhiPopup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateTIme();
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    void updateTIme()
    {
        TypeTimeGame currentTime = GameManager.Instance.getTimeManager().getTimeGame();
        timeText.text = string.Format("Tuần {0} - Tháng {1} - Năm {2}", currentTime.week, currentTime.month, currentTime.year);
    }

    public void OnNextWeek(){
        GameManager.Instance.newWeek();
        updateTIme();
    }

    public void OnUseTime(){
        GameManager.Instance.test();
    }

    public void OnShowDongPhu(){
        UIManager.Instance.Open(luyenKhiPopupPrefab);
    }

    public void OnShowTrain(){
        UIManager.Instance.Open(trainPopupPrefab);
    }



}
