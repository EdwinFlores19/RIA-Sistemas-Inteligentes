using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    [SerializeField] protected GameObject tooltipObject;
    [SerializeField] protected TextMeshProUGUI tooltipText;

    [SerializeField] protected bool display, //segun la configuracion...deberia mostrar el tooltip? 
                                    showing = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        display = SettingsManager.GetShowTooltip();

        Debug.Log(display);

        Hide(); //por defecto estara escondido
    }

    private void Update()
    {
        if (showing) 
        {
            Vector2 mousePosition = Input.mousePosition;
            transform.position = mousePosition;
        }
    }

    public void Show(string text) 
    {
        if (!display) return;

        showing = true;
        tooltipObject.SetActive(showing);
        tooltipText.text = text;
    }

    public void Hide() 
    {
        showing = false;
        tooltipObject.SetActive(showing);
    }
}
