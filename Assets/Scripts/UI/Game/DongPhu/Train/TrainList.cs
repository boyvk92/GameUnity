using UnityEngine;

public class TrainList : BasePopup
{
    public GameObject detailPopup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void Close(){
        UIManager.Instance.CloseTop();
    }

    public void OnShowDetail(){
        UIManager.Instance.Open(detailPopup);
    }
}
