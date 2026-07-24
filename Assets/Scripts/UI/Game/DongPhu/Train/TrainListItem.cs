using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TrainListItem : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public ProgressBar levelProgress;
    public ProgressBar trainProgress;
    public TextMeshProUGUI title;
    public string id;

    public TrainList trainList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadUI(){
        Transform node = transform.Find("Image");
        icon = transform.Find("icon")?.GetComponent<Image>();
        levelProgress = transform.Find("levelProgress")?.GetComponent<ProgressBar>();
        trainProgress = transform.Find("trainProgress")?.GetComponent<ProgressBar>();
        title = transform.Find("title")?.GetComponent<TextMeshProUGUI>();
    }

    void Reset(){
        LoadUI();
        //SetupClickEvents();
    }

    void OnValidate(){
        LoadUI();
       // SetupClickEvents();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnShowDetail();
    }

    public void OnShowDetail(){
        trainList.OnShowDetail();
    }
}
