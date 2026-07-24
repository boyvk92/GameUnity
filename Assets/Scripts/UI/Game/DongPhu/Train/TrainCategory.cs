using UnityEngine;

public class TrainCategory : BasePopup
{
    public GameObject trainList;

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

    public void OnShowList(){
        Debug.Log("OnShowList");
        UIManager.Instance.Open(trainList);
    }

}
