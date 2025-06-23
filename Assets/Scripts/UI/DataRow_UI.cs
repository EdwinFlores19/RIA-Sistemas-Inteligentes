using UnityEngine;

public class DataRow_UI : MonoBehaviour
{
    //---ATRIBUTOS NUMERICOS
    [SerializeField] protected int index = 0; //numero de fila

    //---ATRIBUTOS DE DATOS
    [SerializeField] protected ItemColor color; //NONE, RED, BLUE, YELLOW
    [SerializeField] protected ItemShape shape; //NONE, ROUND, SQUARE, TRIANGLE
    [SerializeField] protected ItemReaction reaction; //NONE, EAT, EVADE, JUMP

    //---REFERENCIAS A SELECTORES DE DATOS
    [SerializeField] protected ShapeSelector_UI shapeSelector; //permite al jugador cambiar el dato "forma" de la fila
    [SerializeField] protected ColorSelector_UI colorSelector; //permite al jugador cambiar el dato "color" de la fila
    [SerializeField] protected ReactionSelector_UI reactionSelector; //permite al jugador cambiar el dato "reaccion" de la fila

    //---DETALLES ESTETICOS
    [SerializeField] protected Animator anim; //bool display, true->mostrar, false->ocultar

    //----------------------------------------------------------------METODOS DE ASIGNACION
    public void SetIndex(int index) { this.index = index; }
    public void SetShape(ItemShape shape) { this.shape = shape; }
    public void SetColor(ItemColor color) { this.color = color; }
    public void SetReaction(ItemReaction reaction) { this.reaction = reaction; }
    public void Clear() //Limpiar todos los campos
    {
        //Seleccionamos la opcion NONE en todas las columnas
        shapeSelector.SelectOption(0);
        colorSelector.SelectOption(0);
        reactionSelector.SelectOption(0);
    }
    public void SetUpOptions() //metodo para que todos los selectores carguen sus opciones correspondientes
    {
        shapeSelector.SetUp();
        colorSelector.SetUp();
        reactionSelector.SetUp();
    }

    //----------------------------------------------------------------METODOS DE CONSULTA
    public DataRow GetDataRow() //metodo para obtener datos como struct "datarow"
    {
        return new DataRow(shape, color, reaction);
    }

    //----------------------------------------------------------------METODOS DE NOTIFICACION
    public void NotifySelectingOn(int dataColumnIndex) //metodo para notificar que columna se esta seleccionando en la fila
    {
        LevelManager.instance.NotifySelectingAt(index, dataColumnIndex); //notificamos al nivel donde se esta seleccionando data
        Tooltip.instance.Hide();
    }

    public void NotifySelectionMade() //metodo para notificar cuando ya se eligio una opcion
    {
        LevelManager.instance.NotifySelectionMade(); //notificamos al nivel de que ya se eligio una opcion
        Tooltip.instance.Hide();
    }

    public void DeselectGapAt(int dataColumnIndex) //oculta el menu de seleccion en uno de los selectores de datos
    {
        //Cancelamos la seleccion en un selector segun la columna que nos diga...
        switch (dataColumnIndex) 
        {
            case 0: //0 es SELECTOR DE FORMA
                shapeSelector.CancelSelection();
                break;
            
            case 1://1 es SELECTOR DE COLOR
                colorSelector.CancelSelection();
                break;

            case 2://2 es SELECTOR DE REACCION
                reactionSelector.CancelSelection();
                break;
        }
    }

    public void DisplaySelectorsExcept(bool display, int exceptionIndex) 
    {
        if(exceptionIndex != 0) shapeSelector.Display(display);
        if (exceptionIndex != 1) colorSelector.Display(display);
        if (exceptionIndex != 2) reactionSelector.Display(display);
    }

    public void DisplaySelectors(bool display) 
    {
        shapeSelector.Display(display);
        colorSelector.Display(display);
        reactionSelector.Display(display);
    }

    public void Display(bool display) //mostrar o no la fila y sus selectores
    {
        anim.SetBool("display", display);
        shapeSelector.Display(display);
        colorSelector.Display(display);
        reactionSelector.Display(display);
    }
}
