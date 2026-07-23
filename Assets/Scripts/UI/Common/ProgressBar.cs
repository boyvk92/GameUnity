using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image bar;
    public TMP_Text text;

    void Awake()
    {
        ResolveReferences();
    }

    void ResolveReferences()
    {
        bar = transform.Find("bar")?.GetComponent<Image>();
        text = transform.Find("text")?.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setProgress(double current, double max){
        if(!this){
            return;
        }

        if(bar == null || text == null){
            ResolveReferences();
        }

        if(bar == null || text == null || max == 0f){
            return;
        }

        text.text = string.Format("{0} / {1}", current, max);
        bar.fillAmount = (float)(current / max);
    }
}
