using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemCondition //dato enum sobre condicion especial de un item (se hace publica aqui para que otras clases puedan usarlo y reconocerlo)
{
    NONE, //ninguna
    CONSUMIBLE, //significa que sera destruido si RIA trata de comerselo
    LETHAL //significa que matara a RIA si se coloca sobre el
}

public class Item : MonoBehaviour
{
    //ATRIBUTOS DE CARACTERISTICAS
    [SerializeField] private ItemShape shape; //forma del item
    [SerializeField] private ItemColor color; //color del item
    [SerializeField] private ItemCondition condition; //condicion especial del item

    //ATRIBUTOS DE ESTADO
    [SerializeField] private bool destroyed = false; //boleano para saber si el objeto ha sido destruido

    //REFERENCIAS FUNCIONALES
    [SerializeField] private Collider2D col; //componente de colision (hitbox, permite que RIA detecte al item)

    //DETALLES ESTETICOS
    [SerializeField] protected Animator anim; //state: 0->destroyed, 1->idle

    //EVENTOS ESPECIALES
    [SerializeField] protected UnityEvent eatenEvent, //metodo especial a ejecutar si el objeto es comido
                                          steppedOnEvent; //metodo especial a ejecutar si el objeto es pisado

    public Item() //Constructor por defecto (con todo en NONE)
    {
        shape = ItemShape.NONE;
        color = ItemColor.NONE;
        condition = ItemCondition.NONE;
    }

    //-------------------------------------------------------------------------------- METODOS DE CONSULTA
    public ItemShape GetItemShape() { return shape; }
    public ItemColor GetColor() { return color; }
    public ItemCondition GetSpecialCondition() { return condition; }
    public ItemShapeAndColor GetShapeAndColor() { return new ItemShapeAndColor(shape, color); }
    public ItemShapeColorAndCondition GetItemShapeColorAndCondition() { return new ItemShapeColorAndCondition(shape, color, condition); }
    public bool IsLethal() { return condition == ItemCondition.LETHAL; }
    public bool IsConsumible() { return condition == ItemCondition.CONSUMIBLE; }
    public bool IsDestroyed() { return destroyed; }

    //------------------------------------------------------------------------------ METODOS DE ASIGNACION
    public void SetShape(ItemShape shape) { this.shape = shape; }
    public void SetColor(ItemColor color) { this.color = color; }
    public virtual void SetDestroyed(bool destroyed) //Metodo para "destruir" o "regenerar" un objeto
    {
        //De ser el caso, destruyelo, ya no muestres el item e inhabilita su colisionador (y haz lo opuesto si te dicen que NO esta destruido)
        this.destroyed = destroyed;
        anim.SetInteger("state", (destroyed) ? 0 : 1); //0 si destruido y 1 si no destruido
        col.enabled = !destroyed;

        //cuando un item se "destruye" en realidad solo oculta su imagen y desactiva su hitbox, para que el item no sea visto ni detectado
        //pero como tal el item sigue ahi en el laberinto, de modo que si se destruye pueda ser "regenerado" (que hace lo opuesto a lo anterior)
    }

    public void Consume() //metodo para cuando RIA se come el objeto
    {
        if (condition != ItemCondition.CONSUMIBLE) return;
        SetDestroyed(true);
        NotifyEaten();
    }

    //----------------------------------------------------------------------------- METODOS DE NOTIFICACION
    public void NotifyEaten() 
    {
        eatenEvent.Invoke();
    }

    public void NotifySteppedOn() 
    {
        steppedOnEvent.Invoke();
    }
}
