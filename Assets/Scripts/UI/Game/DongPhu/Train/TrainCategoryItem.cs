using UnityEngine;

public class TrainCategoryItem : MonoBehaviour
{
    public TrainCategory trainCategory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShowList(){
        trainCategory.OnShowList();
    }
}
