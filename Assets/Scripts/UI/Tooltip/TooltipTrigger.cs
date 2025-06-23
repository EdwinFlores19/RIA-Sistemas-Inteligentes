using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Agrega esta clase a todo elemento UI al que quieras que se muestre el tooltip

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected string text;
    [SerializeField] protected bool displayOnPlay = true;

    void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!displayOnPlay && LevelManager.instance.IsOnPlaying()) return;
        Tooltip.instance.Show(text);
    }

    void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Tooltip.instance.Hide();
    }
}
