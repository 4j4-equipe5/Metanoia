using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHachures : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Hover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hover.SetActive(false);
    }
}
