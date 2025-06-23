using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTileTrigger : MonoBehaviour
{
    //los triggers son como detectores invisibles (pueden identificador objetos que entren en ellos)

    private void OnTriggerEnter2D(Collider2D other) //metodo que se activa si el trigger detecta algo
    {
        //los layers son como clasificaciones que les puedes dar a los objetos de juego, tienen nombre y numero

        if (other.gameObject.layer == 7) //si el objeto detectado es de la categoria RIA (layer numero 7)
        {
            LevelManager.instance.AddScore(); //agrega puntaje
        }
    }
}
