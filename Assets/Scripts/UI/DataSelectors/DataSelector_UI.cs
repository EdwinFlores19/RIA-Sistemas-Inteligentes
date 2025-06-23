using System;
using UnityEngine;

abstract public class DataSelector_UI : MonoBehaviour
{
    //REFEFERENCIA A DATAROW PADRE
    [SerializeField] protected DataRow_UI datarow;

    //REFEFERENCIAS A ANIMADOR
    [SerializeField] protected Animator anim; //state: 0->hidden, 1->showSelectedOption, 2->showAvailableOptions

    [Serializable] protected struct SelectableDataOption //estructura para contener imagenes y sprites de opciones juntas
    {
        [SerializeField] private UnityEngine.UI.Image optionImage;
        [SerializeField] private Sprite optionSprite;

        public void SetUp() //metodo para asignar sprite a la imagen de la opcion 
        {
            optionImage.sprite = optionSprite;
        }

        public Sprite GetSprite() { return optionSprite; } //metodo para obtener el sprite
    }

    //VISUALES
    [SerializeField] protected UnityEngine.UI.Image selectedOptionImage; //imagen de la opcion elegida
    [SerializeField] protected  SelectableDataOption[] options; //sprites a mostrar segun la opcion seleccionada

    public void SetUp() //metodo para que se muestre la imagen correspondiente en las opciones disponibles y seleccionada por defecto
    {
        int length = options.Length;
        for (int i = 0; i < length; i++)
        {
            options[i].SetUp();
        }

        selectedOptionImage.sprite = options[0].GetSprite();
        anim.SetInteger("state", 1); //animacion para mostrar opcion seleccionada por defecto
    }

    public void Display(bool display) 
    {
        if(display) { anim.SetInteger("state", 1); } //ocultar selector
        else { anim.SetInteger("state", 0); } //mostrar opcion seleccionada
    }

    public virtual void ShowOptions() //Mostrar opciones seleccionables
    {
        if (LevelManager.instance.IsOnPlaying()) return;
        

        //ya no mostramos la opcion seleccionada, sino que mostramos las opciones disponibles
        anim.SetInteger("state", 2); //animacion para mostrar opciones disponibles

        NotifySelecting();
    }

    protected abstract void NotifySelecting(); //solo cambia el numero

    public virtual void SelectOption(int option) //Seleccionar y mostrar opcion seleccionada
    {
        //cambiamos el aspecto de la opcion seleccionada
        selectedOptionImage.sprite = options[option].GetSprite();

        anim.SetInteger("state", 1); //animacion para mostrar opcion seleccionada

        AssignDataToDataRow(option); //asignamos la data seleccionada al datarow
        datarow.NotifySelectionMade(); //notificamos que ya terminamos de seleccionar
    }

    public virtual void CancelSelection() 
    {
        //ya no mostramos el submenu de opciones, nos quedamos con lo que habiamos seleccionado antes
        anim.SetInteger("state", 1); //animacion para mostrar opcion seleccionada
    }

    public abstract void AssignDataToDataRow(int option); //Asignar data seleccionada en el datarow (lo asignado cambia segun el tipo de dato)
}
