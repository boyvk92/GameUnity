using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class PopupLayer : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        ApplyStretch();
    }

    private void OnEnable()
    {
        ApplyStretch();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyStretch();
    }
#endif

    private void ApplyStretch()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        if (_rectTransform == null)
            return;

        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.one;

        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;

        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.localScale = Vector3.one;
        _rectTransform.localRotation = Quaternion.identity;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
      //  UIManager.Instance.CloseTop();
    }
}
