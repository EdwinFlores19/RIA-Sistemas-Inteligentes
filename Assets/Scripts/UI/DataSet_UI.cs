using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DataSet_UI : MonoBehaviour
{
    //---REFERENCIAS A OBJETOS DE JUEGO
    [SerializeField] protected GameObject dataHeaderPrefab, dataRowPrefab;

    //---ENCABEZADOS Y FILAS DE DATOS
    [SerializeField] protected List<DataHeader_UI> dataHeaders;
    [SerializeField] protected List<DataRow_UI> dataRows;

    //---ATRIBUTOS NUMERICOS
    [SerializeField] protected float verticalOffset = 0.15f, horizontalOffset;
    [SerializeField] protected int lastEditedRowIndex = -1, lastEditedColumnIndex = -1; //fila y columna del ultimo dato modificado
    [SerializeField] protected Vector2 currentHeaderPosition;

    //---REFERENCIAS A DETALLES ESTETICOS
    [SerializeField] protected SoundPlayer soundPlayer; //0->showOptions, 1->selectOption

    //------------------------------------------------------------METODOS DE VISUALIZACION

    public void Display(bool display) //metodo para mostrar o no las filas y los encabezados
    {
        int length = dataRows.Count, toThreeCount = 3, j = 0;
        for (int i = 0; i < length; i++)
        {
            if (toThreeCount == 3) 
            {
                dataHeaders[j].Display(display);
                toThreeCount = 0;
                j++;
            }

            dataRows[i].Display(display);
            toThreeCount++;
        }
    }

    //------------------------------------------------------------METODOS DE CARGA DE DATOS
    public void GenerateDataRows(int totalDataRows) //Generar las filas de datos a llenar
    {
        int toThreeCount = 3;
        for (int i = 0; i < totalDataRows; i++)
        {
            if (toThreeCount == 3) 
            {
                GameObject newHeaderObj = Instantiate(dataHeaderPrefab, this.transform);

                currentHeaderPosition = new Vector2(dataHeaders.Count * horizontalOffset, currentHeaderPosition.y);
                newHeaderObj.GetComponent<RectTransform>().anchoredPosition = currentHeaderPosition;

                dataHeaders.Add(newHeaderObj.GetComponent<DataHeader_UI>());
                dataHeaders[dataHeaders.Count - 1].Display(true);

                lastEditedColumnIndex = i;
                toThreeCount = 0;
            }

            GameObject newRowObj = Instantiate(dataRowPrefab, this.transform);

            newRowObj.GetComponent<RectTransform>().anchoredPosition = new Vector2
                (currentHeaderPosition.x, currentHeaderPosition.y + (verticalOffset * (i+1-(dataHeaders.Count-1)*3)));

            dataRows.Add(newRowObj.GetComponent<DataRow_UI>());

            dataRows[i].SetIndex(i);
            dataRows[i].SetUpOptions();

            toThreeCount++;
        }
    }

    public void LoadData() //Cargar todas las filas de datos en el data set real
    {
        DeselectDataRow(lastEditedRowIndex, lastEditedColumnIndex);

        //Cargamos todas las filas dentro del modelo...
        int length = dataRows.Count;
        for (int i = 0; i < length; i++)
        {
            LevelManager.instance.SetDatasetRow(i, dataRows[i].GetDataRow());
        }
    }

    //------------------------------------------------------------METODOS DE FILAS DE DATOS
    public void NotifySelectingAt(int dataRowIndex, int dataColumnIndex) 
    {
        //Deseleccionamos el gap anterior (si es que es diferente al que estamos seleccionando ahora)
        if (dataRowIndex != lastEditedRowIndex || dataColumnIndex != lastEditedColumnIndex) 
            DeselectDataRow(lastEditedRowIndex, lastEditedColumnIndex);

        ///Registramos la fila y columna del ultimo dato editado
        lastEditedRowIndex = dataRowIndex;
        lastEditedColumnIndex = dataColumnIndex;

        //Todos los otros selectores pasan a esconderse
        int length = dataRows.Count;
        for (int i = 0; i < length; i++)
        {
            if(i == lastEditedRowIndex) { dataRows[i].DisplaySelectorsExcept(false, lastEditedColumnIndex); }
            else { dataRows[i].DisplaySelectors(false); }
        }

        soundPlayer.PlaySound(0); //sonido al mostrar opciones
    }

    public void NotifySelectionMade() 
    {
        soundPlayer.PlaySound(1); //sonido al seleccionar una opcion

        //Todos los otros selectores pasan a mostrarse
        int length = dataRows.Count;
        for (int i = 0; i < length; i++)
        {
            if (i == lastEditedRowIndex) { dataRows[i].DisplaySelectorsExcept(true, lastEditedColumnIndex); }
            else { dataRows[i].DisplaySelectors(true); }
        }
    }

    public void DeselectDataRow(int dataRowIndex, int dataColumnIndex) //quita el submenu de seleccion de alguna fila 
    {
        //Esto solo debe ocurrir si seleccionas un dato, teniendo otro ya seleccionado previamente
        //y evidentemente si la fila que se indica existe...
        int length = dataRows.Count;

        if (dataRowIndex < 0 || dataRowIndex >= length) return;
        dataRows[dataRowIndex].DeselectGapAt(dataColumnIndex);
    }

    public void ClearAllData() 
    {
        int length = dataRows.Count;
        for (int i = 0; i < length; i++)
        {
            dataRows[i].Clear();
        }
    }
}