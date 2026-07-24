using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Root")]
    [SerializeField] private Transform popupRoot;
    [SerializeField] private Transform hudRoot;
    [SerializeField] private Transform toastRoot;

    private readonly Stack<BasePopup> popupStack = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private T OpenPopup<T>(T popup) where T : BasePopup
    {
        popup.Open();
        popupStack.Push(popup);

        return popup;
    }

    public T Open<T>(T prefab) where T : BasePopup
    {
        T popup = Instantiate(prefab, popupRoot, false);

        return OpenPopup(popup);
    }

    public BasePopup Open(GameObject prefab)
    {
        BasePopup popup = Instantiate(prefab, popupRoot, false).GetComponent<BasePopup>();

        if (popup == null)
        {
            Debug.LogError($"UIManager.Open expected a BasePopup on prefab {prefab.name}");
            return null;
        }

        return OpenPopup(popup);
    }

    public void Close(BasePopup popup)
    {
        if (popup == null)
            return;

        if (popupStack.Count > 0)
        {
            Stack<BasePopup> tempStack = new();

            while (popupStack.Count > 0)
            {
                BasePopup current = popupStack.Pop();

                if (current == popup)
                {
                    break;
                }

                tempStack.Push(current);
            }

            while (tempStack.Count > 0)
            {
                popupStack.Push(tempStack.Pop());
            }
        }
        Destroy(popup.gameObject);
    }

    public void CloseTop()
    {
        if (popupStack.Count == 0)
            return;

        Close(popupStack.Pop());
    }

    public void CloseAll()
    {
        while (popupStack.Count > 0)
        {
            CloseTop();
        }
    }
}
