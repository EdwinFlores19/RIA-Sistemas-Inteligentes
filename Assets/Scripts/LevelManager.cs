using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    /*
     * - Se encargara de iniciar y reiniciar a ría, asi como a los ítems en el escenario
     * - Inicializa elementos imprescindibles en el nivel (dataset, interfaz, etc)
     * - Funge de medio de comunicacion estatica entre distintos scripts
     */

    //---REFERENCIAS A CLASES
    public static LevelManager instance; //referencia estatica a si mismo (para que todos lo puedan llamar)

    [SerializeField] protected DataSet dataSet; //modelo de dataset
    [SerializeField] protected MissionManager missionManager; //gestor de la mision del nivel
    [SerializeField] protected RIA ria; //ria el raton

    [SerializeField] protected InterfaceManager interfaceManager;
    [SerializeField] protected ResultsManager resultsManager;

    [SerializeField] protected AudioSource ostAudio; //reproductor de musica

    //---ATRIBUTOS DE ESTADO
    [Serializable] protected enum LevelState //estado del nivel segun pase el tiempo y el jugador interactue con el juego
    {
        SETUP, //ocurre antes de comenzar, inicializa todo en el nivel
        IDLE, //ocurre cuando no haz seleccionado nada y no haz iniciado la simulacion de RIA
        SELECTING, //ocurre cuando seleccionas algo en el dataset
        LOADING_DATA, //ocurre cuando comienzas la simulacion de RIA
        PLAYING, //ocurre durante la simulacion de RIA
        ENDING //ocurre cuando ganas el nivel y te aparece la pantalla de felicitaciones
    };
    [SerializeField] protected LevelState state; //estado actual del nivel

    //---REFERENCIAS A OBJETOS DE JUEGO
    [SerializeField] protected Item[] items; //lista de items en el nivel

    private void Start()
    {
        SetUp();
    }

    //--------------------------------------------------------------------------------------------------------METODOS DE CICLO DE JUEGO
    public void SetUp() //Inicializar, asignar y configurar todo lo necesario antes de jugar
    {
        instance = this;
        SetState(LevelState.SETUP);
        interfaceManager.SetUp();
        ostAudio.enabled = SettingsManager.GetEnableOst();
        //esperamos a que la interfaz nos confirme que se acabo de iniciar todo...
    }

    public void NotifySetUpCompleted() 
    {
        SetState(LevelState.IDLE);
        resultsManager.StartCountingTime();
    }

    public void Begin() //Iniciar simulacion con RIA
    {
        SetState(LevelState.LOADING_DATA);


        interfaceManager.ShowRetryButton(); //mostramos boton para reintenetar
        interfaceManager.LoadDatasetUIData(); //cargamos el dataset asignado por el jugador
        interfaceManager.DisplayDatasetUI(false); //ocultar dataset visual

        //Iniciamos a RIA...
        SetState(LevelState.PLAYING);
        ria.StartNow();
    }
    public void Retry() //Detener simulacion y volver a manipular el dataset
    {
        SetState(LevelState.IDLE);

        interfaceManager.ShowPlayButton(); //mostramos boton para jugar

        ria.ResetNow();
        missionManager.ResetScore();
        interfaceManager.ShowMission();

        interfaceManager.DisplayDatasetUI(true);

        //Restarurar todos los items destruidos
        int length = items.Length;
        if (length == 0) return;
        for (int i = 0; i < length; i++)
        {
            if (items[i].IsDestroyed()) items[i].SetDestroyed(false);
        }

    }
    public void EndLevel() //Mostrar pantalla de nivel completado
    {
        SetState(LevelState.ENDING);

        resultsManager.StopCountingTime();
        interfaceManager.ShowLevelCompleteMenu(true);
    }
    public void RestartLevel() //Volver a probar el nivel
    {
        interfaceManager.ShowLevelCompleteMenu(false);
        interfaceManager.ClearAllDatasetUIData();
        interfaceManager.ShowMission();
        resultsManager.ResetAllCounters();
        resultsManager.StartCountingTime();

        Retry();
    }
    public void GoToMainMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //--------------------------------------------------------------------------------------------------------METODOS CAMBIO DE ESTADO
    //------------------------------------------------------------METODOS DE CAMBIO DE ESTADO
    protected void SetState(LevelState state)
    {
        this.state = state;
    }
    public void SetStateSelecting() { SetState(LevelState.SELECTING); }
    public void SetStateLoadingData() { SetState(LevelState.LOADING_DATA); }
    public void SetStatePlaying() { SetState(LevelState.PLAYING); }
    
    //------------------------------------------------------------METODOS DE NOTIFICACION
    public void NotifySelectingAt(int dataRowIndex, int dataColumnIndex) 
    {
        SetState(LevelState.SELECTING);
        interfaceManager.NotifyDatasetUISelectingAt(dataRowIndex, dataColumnIndex);
    }
    public void NotifySelectionMade() 
    {
        SetState(LevelState.IDLE);
        interfaceManager.NotifyDatasetUISelectionMade();
    }

    public void NotifyLevelComplete() 
    {
        interfaceManager.NotifyMissionComplete();
        Invoke("EndLevel", 1f);
        //EndLevel();
    }

    public void TrembleLevelVisualizer() { interfaceManager.TrembleLevelVisualizer(); }

    //------------------------------------------------------------METODOS DE CONSULTA
    public bool IsOnIdle() { return state == LevelState.IDLE; }
    public bool IsOnSelecting() { return state == LevelState.SELECTING; }
    public bool IsOnPlaying() { return state == LevelState.PLAYING; }
    public string GetMissionStatement() { return missionManager.GetMissionStatement(); }
    public int GetTotalDatasetRows() { return dataSet.GetTotalDataRows(); }
    public string GetWinTimeAsFormatedString() { return resultsManager.GetWinTimeAsFormatedString(); }

    //------------------------------------------------------------METODOS DE ASIGNACION
    public void SetDatasetRow(int dataRowIndex, DataRow dataRow) { dataSet.SetDataRow(dataRowIndex, dataRow); }
    public ItemReaction GetDatasetReactionTo(ItemShapeAndColor shapeAndColor) { return dataSet.GetReactionTo(shapeAndColor); }
    public void ClearDataset() 
    {
        if (!IsOnIdle()) return;
        interfaceManager.ClearAllDatasetUIData(); 
    }
    public void AddScore() 
    {
        missionManager.AddScore();
        interfaceManager.ShowMission();
    }
}