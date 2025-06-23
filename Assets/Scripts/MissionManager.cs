using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour //Gestiona requisitos y progreso para completar el nivel
{
    [SerializeField] protected int goalScore = 1, //puntaje a conseguir para ganar
                                   currentScore = 0; //puntaje actual del jugador

    [SerializeField] protected string missionHint;

    public string GetMissionStatement() 
    {
        if (currentScore == goalScore) return "¡Misión completada!";
        else return "Misión: " + missionHint + " " + ((goalScore > 1) ? currentScore + " / " + goalScore : "");
    }

    public void AddScore() //Metodo para agregar puntaje
    {
        if (currentScore >= goalScore) return; //No hagas nada si ya se alcanzo la meta...
        currentScore++; //de lo contrario, suma puntaje

        if (currentScore == goalScore) LevelManager.instance.NotifyLevelComplete(); //si se alcazo la meta, termina el nivel
    }

    public void ResetScore() //Metodo para reiniciar el puntaje
    {
        currentScore = 0;
    }
}
