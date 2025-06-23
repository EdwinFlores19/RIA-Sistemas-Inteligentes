using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    //---REFERENCIAS A ELEMENTOS DE LA INTERFAZ
    [SerializeField] protected DataSet_UI dataSet_UI; //componente de interfaz del dataset
    [SerializeField] protected Animator levelVisualizer, missionVisualizer; //pantalla del mapa
    [SerializeField] protected GameObject buttonsPad; //objeto con los botones de control del nivel
    [SerializeField]
    protected TextMeshProUGUI missionText, //texto de la mision
                               timeCounterText; //texto en el que se muestra el tiempo que te tomo terminar el nivel
    [SerializeField] protected SoundPlayer soundPlayer; //0->play, 1->retry, 2->clearDataSet, 3->missionComplete

    [SerializeField] protected GameObject playButton, retryButton, LevelCompleteMenu; //objetos de la interfaz

    protected Coroutine setingUp; //variable de rutina de inicio

    //---ATRIBUTOS NUMERICOS
    [SerializeField] protected float setUpDelayTime = 0.6f;

    public void SetUp() 
    {
        if (setingUp != null) return; //si ya se esta iniciando todo, no procedas
        buttonsPad.SetActive(false); //por defecto no se deberian de ver los botones
        setingUp = StartCoroutine(SetingUp()); //inicamos todo, y asignamos el proceso a una variable para tener constancia de que se esta realizando
    }

    protected IEnumerator SetingUp() //rutina de "montado" de la interfaz
    {
        missionText.text = LevelManager.instance.GetMissionStatement(); //mostramos el texto de la mision!
        missionVisualizer.SetBool("show", true);

        yield return new WaitForSeconds(setUpDelayTime); //esperamos un tiempo antes de mostrar el dataset
        dataSet_UI.GenerateDataRows(LevelManager.instance.GetTotalDatasetRows()); //mostramos el dataset!
        buttonsPad.SetActive(true); //mostramos los botones de control

        yield return new WaitForSeconds(setUpDelayTime); //esperamos un tiempo antes de mostrar el nivel
        levelVisualizer.SetBool("display", true); //mostramos el nivel!


        LevelManager.instance.NotifySetUpCompleted(); //avisamos al levelManager, que la interfaz ya se inicio!
        setingUp = null; //vaciamos la variable de referencia del proceso
    }

    public void TrembleLevelVisualizer() { levelVisualizer.SetTrigger("tremble"); }

    public void LoadDatasetUIData() 
    {
        dataSet_UI.LoadData();
    }

    public void DisplayDatasetUI(bool display) 
    {
        dataSet_UI.Display(display);
    }

    public void ClearAllDatasetUIData() 
    {
        dataSet_UI.ClearAllData();
        soundPlayer.PlaySound(2);
    }

    public void NotifyDatasetUISelectingAt(int row, int column) 
    {
        dataSet_UI.NotifySelectingAt(row, column);
    }

    public void NotifyDatasetUISelectionMade() 
    {
        dataSet_UI.NotifySelectionMade();
    }

    public void ShowPlayButton() 
    {
        playButton.SetActive(true);
        retryButton.SetActive(false);
        soundPlayer.PlaySound(1);
    }
    public void ShowRetryButton()
    {
        playButton.SetActive(false);
        retryButton.SetActive(true);
        soundPlayer.PlaySound(0);
    }

    public void ShowLevelCompleteMenu(bool display) 
    {
        LevelCompleteMenu.SetActive(display);
        timeCounterText.text = LevelManager.instance.GetWinTimeAsFormatedString();
    }

    public void NotifyMissionComplete() 
    {
        missionText.text = "Misión Completada";
        soundPlayer.PlaySound(3);
        missionVisualizer.SetTrigger("motion");
    }

    public void ShowMission() 
    {
        missionText.text = LevelManager.instance.GetMissionStatement(); //mostramos el texto de la mision!
        missionVisualizer.SetTrigger("motion");
    }
}
