using System.Diagnostics;
using System.Collections;
using UnityEngine;
using System;

public class ResultsManager : MonoBehaviour
{
    //---ATRIBUTOS CONTADORES DE RESULTADOS DEL JUGADOR DURANTE UN NIVEL
    [SerializeField] protected bool countingTime = false;
    protected Coroutine timeCoutingRoutine;

    //---ATRIBUTOS DE DIAGNOSTICO DE TIEMPOS
    protected Stopwatch stopwatch = new Stopwatch(); //medidor de tiempo

    //--------------------------------------------METODOS DE ASIGNACION
    public void ResetAllCounters() 
    {
        StopCountingTime();
        stopwatch.Reset();
    }

    public void StartCountingTime() 
    {
        if (countingTime) return;

        countingTime = true;
        stopwatch.Start();
    }

    public void StopCountingTime() 
    {
        if (!countingTime) return;

        countingTime = false;
        stopwatch.Stop();
    }

    //--------------------------------------------METODOS DE CONSULTA
    public float GetWinTime()
    {
        // Convertir el tiempo total en milisegundos a minutos con decimales
        return (float)stopwatch.Elapsed.TotalMinutes;
    }

    public string GetWinTimeAsFormatedString()
    {
        TimeSpan elapsed = stopwatch.Elapsed;
        return string.Format("{0:D2}:{1:D2}", elapsed.Minutes, elapsed.Seconds);
    }
}