using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainListItem : MonoBehaviour
{
    public Image icon;
    public ProgressBar levelProgress;
    public ProgressBar trainProgress;
    public TextMeshProUGUI title;
    public string id;
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

    public void OnShowDetail(){
        
    }
}
