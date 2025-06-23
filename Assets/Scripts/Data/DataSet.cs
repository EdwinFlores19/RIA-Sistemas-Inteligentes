using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using UnityEngine.UIElements;
using System.Linq;

//Datos que puede tener un data row
public enum ItemShape { NONE, ROUND, SQUARE, TRIANGLE };
public enum ItemColor { NONE, RED, YELLOW, BLUE };
public enum ItemReaction { NONE, EAT, EVADE, JUMP }

public class DataSet : MonoBehaviour //es basicamente una tabla de multiples datarows
{
    //---LISTA DE FILAS
    [SerializeField] protected DataRow[] dataRows;

    //---------------------------------------------------------------------------------METODOS DE ASIGNACION 
    public void SetDataRow(int dataRowIndex, DataRow datarow) 
    {
        dataRows[dataRowIndex] = datarow;
    }

    //---------------------------------------------------------------------------------METODOS DE CONSULTA
    public ItemReaction GetReactionTo(ItemShapeAndColor shapeAndColor)
    {
        return GetReactionTo(shapeAndColor.GetColor(), shapeAndColor.GetShape());
    }

    public ItemReaction GetReactionTo(ItemColor detectedColor, ItemShape detectedShape)
    {
        int totalDataRows = GetTotalDataRows();
        int maxSimilitude = 0;

        // Paso 1: encontrar similitud máxima mayor a 0
        for (int i = 0; i < totalDataRows; i++)
        {
            int similitude = GetSimilitudeRate(dataRows[i].GetColor(), dataRows[i].GetShape(), detectedColor, detectedShape);
            if (similitude > maxSimilitude)
            {
                maxSimilitude = similitude;
            }
        }

        // Si no se encontró ninguna similitud válida, retornamos NONE
        if (maxSimilitude == 0) return ItemReaction.NONE;

        // Paso 2: recolectar reacciones con esa similitud
        List<ItemReaction> reactions = new List<ItemReaction>();
        for (int i = 0; i < totalDataRows; i++)
        {
            int similitude = GetSimilitudeRate(dataRows[i].GetColor(), dataRows[i].GetShape(), detectedColor, detectedShape);
            if (similitude == maxSimilitude)
            {
                reactions.Add(dataRows[i].GetReaction());
            }
        }

        if (reactions.Count == 0) return ItemReaction.NONE;

        // Paso 3: contar votos de cada uno de las reacciones candidatas
        //Diccionario para contar votos, tiene un dato clave (la reaccion) y un dato numerico (los votos)
        Dictionary<ItemReaction, int> candidates = new Dictionary<ItemReaction, int>();

        //Revisaremos cada una de las reacciones de las filas con mayor grado de similitud
        foreach (ItemReaction reaction in reactions)
        {
            if (!candidates.ContainsKey(reaction)) //si la reaccion revisada no esta en el diccionario
                candidates[reaction] = 0; //la agregamos y...

            candidates[reaction]++; //sumamos un voto a la reaccion en el diccionario
        }

        // Paso 4: determinar reacciones candidatas con más votos
        int maxVotes = candidates.Values.Max(); //obtenemos la cantidad mas alta de votos

        //Enlistamos como finalistas las reacciones del diccionario con mayor cantidad de votos
        List<ItemReaction> finalists = candidates //declarar lista
        .Where(pair => pair.Value == maxVotes) //buscar reacciones del diccionario con maxima cantidad de votos
        .Select(pair => pair.Key) //seleccionar su valor de "reaccion"
        .ToList(); //y colocarlo en la lista

        // Paso 5: Preferimos comer si está entre los finalistas
        if (finalists.Contains(ItemReaction.EAT))
            return ItemReaction.EAT;

        // Sino, retornamos un finalista aleatorio
        return finalists[UnityEngine.Random.Range(0, finalists.Count)];
    }

    private int GetSimilitudeRate(ItemColor rowColor, ItemShape rowShape, ItemColor detectedColor, ItemShape detectedShape) //metodo para obtener similitud entre un item y un datarow
    {
        // Caso A: Ambos atributos de fila son NONE y ambos detectados tambien lo son - retorna 2 puntos
        if (rowColor == ItemColor.NONE && rowShape == ItemShape.NONE &&
            detectedColor == ItemColor.NONE && detectedShape == ItemShape.NONE)
        {
            return 2;
        }

        // Caso B: Ambos atributos de fila son NONE, pero los detectados no lo son - retorna 0 puntos
        if (rowColor == ItemColor.NONE && rowShape == ItemShape.NONE &&
            (detectedColor != ItemColor.NONE || detectedShape != ItemShape.NONE))
        {
            return 0;
        }

        //int similitude = 0;

        // Caso C: Uno de los atributos de fila esta definido y el otro es NONE
        // Si el atributo definido coincide con el detectado - retorna 1 punto
        if (rowColor != ItemColor.NONE && rowShape == ItemShape.NONE && rowColor == detectedColor)
        {
            return 1;//similitude = 1;
        }
        else if (rowShape != ItemShape.NONE && rowColor == ItemColor.NONE && rowShape == detectedShape)
        {
            return 1;//similitude = 1;
        }
        // Caso D: Ambos atributos de fila estan definidos y ambos coinciden con los detectados - retorna 2 puntos
        else if (rowColor != ItemColor.NONE && rowShape != ItemShape.NONE &&
                 rowColor == detectedColor && rowShape == detectedShape)
        {
            return 2; //similitude = 2;
        }
        // Otros casos no suman puntos

        return 0; //return similitude;
    }

    public int GetTotalDataRows()
    { return dataRows.Length; }
}

[Serializable]
public struct ItemShapeAndColor //Estructura para pasar ambos atributos como un solo parametro
{
    [SerializeField] private ItemShape shape;
    [SerializeField] private ItemColor color;

    public ItemShapeAndColor(ItemShape shape, ItemColor color)
    {
        this.shape = shape;
        this.color = color;
    }

    public ItemShape GetShape() { return shape; }
    public ItemColor GetColor() { return color; }
    public bool IsNull() { return (shape == ItemShape.NONE && color == ItemColor.NONE); }

    public void SetShape(ItemShape shape) { this.shape = shape; }
    public void SetColor(ItemColor color) { this.color = color; }

    public string GetStringDesc()
    {
        return "Shape: " + shape.ToString() + ", Color: " + color.ToString();
    }
}

[Serializable]
public struct ItemShapeColorAndCondition 
{
    [SerializeField] private ItemShape shape;
    [SerializeField] private ItemColor color;
    [SerializeField] private ItemCondition condition;

    public ItemShapeColorAndCondition(ItemShape shape, ItemColor color, ItemCondition condition)
    {
        this.shape = shape;
        this.color = color;
        this.condition = condition;
    }

    public ItemShape GetShape() { return shape; }
    public ItemColor GetColor() { return color; }
    public ItemCondition GetCondition() { return condition; }
    public ItemShapeAndColor GetShapeAndColor() { return new ItemShapeAndColor(shape, color); }
    public bool IsNull() { return (shape == ItemShape.NONE && color == ItemColor.NONE); }

    public void SetShape(ItemShape shape) { this.shape = shape; }
    public void SetColor(ItemColor color) { this.color = color; }
    public void SetCondition(ItemCondition condition) { this.condition = condition;  }

    public string GetStringDesc()
    {
        return "Shape: " + shape.ToString() + ", Color: " + color.ToString();
    }
}

[Serializable]
public struct DataRow //fila de data
{
    [SerializeField] private ItemShape shape;
    [SerializeField] private ItemColor color;
    [SerializeField] private ItemReaction reaction;

    //CONSTRUCTOR
    public DataRow(ItemShape shape, ItemColor color, ItemReaction reaction) 
    {
        this.shape = shape;
        this.color = color;
        this.reaction = reaction;
    }

    //METODOS GET
    public ItemShape GetShape() { return shape; }
    public ItemColor GetColor() { return color; }
    public ItemReaction GetReaction() { return reaction; }

    //METODOS SET
    public void SetShape(ItemShape shape) { this.shape = shape; }
    public void SetColor(ItemColor color) { this.color = color; }
    public void SetReaction(ItemReaction reaction) { this.reaction = reaction; }
    public void Clear() //Limpiar todos los campos
    {
        SetShape(ItemShape.NONE);
        SetColor(ItemColor.NONE);
        SetReaction(ItemReaction.NONE);
    }
}
