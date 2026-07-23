using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class BasePopup : MonoBehaviour
{
    public Button btn_close;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Open() {}
    public virtual void Close() {}
    
    void Reset(){
        LoadUI();
        SetupClickEvents();
    }

    void OnValidate(){
        LoadUI();
        SetupClickEvents();
    }

    protected virtual void LoadUI()
    {
        btn_close = transform.Find("close")?.GetComponent<Button>();
    }

    void Awake(){
        LoadUI();
        BindRuntimeClickEvents();
    }

    protected virtual void SetupClickEvents(){
        #if UNITY_EDITOR
            if(btn_close != null){
                UnityEventTools.RemovePersistentListener(btn_close.onClick, Close);
                UnityEventTools.AddPersistentListener(btn_close.onClick, Close);
                EditorUtility.SetDirty(btn_close);
            }
        #endif
    }

    protected virtual void BindRuntimeClickEvents(){
        if(btn_close != null){
            btn_close.onClick.RemoveListener(Close);
            btn_close.onClick.AddListener(Close);
        }
    }
}
