using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class RIA : MonoBehaviour
{
    //---REFERENCIAS DE TILEMAP
    [SerializeField] private Tilemap tilemap; //mapa de casillas
    private Vector2 tileSize = new Vector2(1, 1);

    //---REFERENCIAS PARA DETALLES ESTETICOS
    [SerializeField] private Animator anim;
    [SerializeField] protected SoundPlayer soundPlayer; //0->jump, 1->die, 2->eat

    //---ATRIBUTOS NUMERICOS
    [SerializeField] private float moveDuration = 0.15f; //cuanto dura cada paso (los saltos duran el doble)
    private int lastRandomNumber = -1; //puede ser 0 o 1, se intercalan

    //---ATRIBUTOS DE ESTADO
    [SerializeField] private enum ActState { OFF, IDLE, MOVING, ROTATING, EATING, JUMPING }
    [SerializeField] private ActState state; //estado de accion que se encuentra realizando actualmente

    private enum Action { NONE, EAT, JUMP, WALK, TURN_LEFT, TURN_RIGHT, DIE } //Acciones que puede realizar RIA
    private enum Orientation { NONE, LEFT, RIGHT, FRONT }; //Orientacion de una casilla respecto de la posicion de RIA

    //---ATRIBUTOS DE NAVEGACION
    private Vector2 startPosition;
    private float startRotation;
    [SerializeField] protected LayerMask detectableLayerMasks;

    private void OnEnable() //nada mas carga el nivel...
    {
        startPosition = transform.position; //guardamos posicion inicial
        startRotation = transform.eulerAngles.z; //guardamos rotacion inicial
    }

    //--------------------------------------------------------------------METODOS DE CICLO DE JUEGO
    public void StartNow() //Metodo para hacer que RIA se active
    {
        SetState(ActState.IDLE); //ponte en el modo inicial para comenzar a andar
    }

    public void ResetNow() //Metodo para que RIA vuelva a su posicion original y se quede quieto
    {
        StopAllCoroutines(); //deten todas las rutinas
        this.transform.position = startPosition; //ponte en la posicion inicial
        transform.rotation = Quaternion.Euler(0, 0, startRotation); //ponte en tu rotacion inicial
        SetState(ActState.OFF); //apagate
    }

    //--------------------------------------------------------------------METODOS PARA ACCION WALK
    private void Walk() //metodo para que RIA de un paso adelante
    {
        if (!IsOnIdle()) return;
        //Calculamos a donde va a moverse RIA
        Vector2 nextPosition = GetForwardPosition(1);

        //Si hay una casilla ahí, procede a moverse
        if (IsThereGroundAt(nextPosition))
        {
            SetState(ActState.MOVING);
            StartCoroutine(MoveTo(nextPosition, moveDuration));//transform.position = nextPosition;
        }
    }
    private IEnumerator MoveTo(Vector2 destiny, float duration)
    {
        float elapsedTime = 0f; //tiempo transcurrido
        Vector2 start = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(start, destiny, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destiny; // Asegura que llegue al destino exacto
        SetState(ActState.IDLE);
    }

    //--------------------------------------------------------------------METODOS PARA ACCION ROTAR / GIRAR
    private void RotateLeft() //girar a la izquierda
    {
        if (!IsOnIdle()) return;
        StartCoroutine(RotateTo(transform.eulerAngles.z + 90f));
    }
    private void RotateRight() //girar a la derecha
    {
        if (!IsOnIdle()) return;
        StartCoroutine(RotateTo(transform.eulerAngles.z - 90f));
    }
    private IEnumerator RotateTo(float rotation)
    {
        SetState(ActState.ROTATING);
        float elapsedTime = 0f, initialRotation = transform.eulerAngles.z;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float anguloActual = Mathf.Lerp(initialRotation, rotation, t);
            transform.rotation = Quaternion.Euler(0f, 0f, anguloActual);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, rotation); // Asegura rotación final exacta
        SetState(ActState.IDLE);
    }

    //--------------------------------------------------------------------METODOS PARA ACCION SALTO
    private void Jump()
    {
        //Si ya te estas moviendo o no hay suelo a 2 casillas de distancia, no hagas nada
        if (!IsOnIdle()) return;
        Vector2 nextPosition = GetForwardPosition(2);

        SetState(ActState.JUMPING);
        StartCoroutine(MoveTo(nextPosition, 2 * moveDuration));
        soundPlayer.PlaySound(0); //sonido de saltar
    }

    //--------------------------------------------------------------------METODOS PARA ACCION COMER
    private void Eat()
    {
        if (!IsOnIdle()) return;
        //Calculamos a donde va a moverse RIA
        Vector2 nextPosition = GetForwardPosition(1);

        //Si hay una casilla ahí, procede a moverse
        if (IsThereGroundAt(nextPosition))
        {
            SetState(ActState.EATING);

            //Consume el item en la casilla destino si es que se puede
            Collider2D hit = Physics2D.OverlapBox(nextPosition, tileSize, 0f, detectableLayerMasks);
            if (hit != null) 
            {
                hit.GetComponent<Item>().Consume();
                soundPlayer.PlaySound(2); //sonido de comer
            }

            StartCoroutine(MoveTo(nextPosition, moveDuration));//transform.position = nextPosition;
        }
    }

    //--------------------------------------------------------------------METODOS PARA ACCION MORIR
    private void Die()
    {
        if (!IsOnIdle()) return;
        anim.SetInteger("state", 3);
        soundPlayer.PlaySound(1); //sonido de morir
    }

    //--------------------------------------------------------------------METODOS DE ASIGNACION E INSTRUCCION
    private void SetState(ActState newState)
    {
        //Asigna el estado que indiquen y reproduce la animacion correspondiente
        state = newState;
        switch (newState)
        {
            case ActState.OFF: //0
                anim.SetInteger("state", 1);
                break;

            case ActState.IDLE: //1
                anim.SetInteger("state", 1);
                StartCoroutine(OptimalActionAwait()); //al volver a idle, piensa cual sera la siguiente accion a realizar
                break;

            case ActState.MOVING: //2
                anim.SetInteger("state", 2);
                break;

            case ActState.ROTATING: //3
                break;

            case ActState.EATING: //4
                anim.SetInteger("state", 4);
                break;

            case ActState.JUMPING: //5
                anim.SetInteger("state", 5);
                break;
        }
    }

    private IEnumerator OptimalActionAwait()
    {
        yield return new WaitForSeconds(0.15f);
        PerformOptiomalAction();
    }

    private void Perform(Action action)
    {
        //Realiza la accion correspondiente a la instruccion que te indican
        switch (action)
        {
            default:
                Debug.Log("No se realizo ninguna accion...");
                break;

            case Action.EAT:
                Eat();
                break;

            case Action.JUMP:
                Jump();
                break;

            case Action.WALK:
                Walk();
                break;

            case Action.TURN_LEFT:
                RotateLeft();
                break;

            case Action.TURN_RIGHT:
                RotateRight();
                break;

            case Action.DIE:
                Die();
                break;
        }
    }
    public void PerformOptiomalAction()
    {
        Perform(GetOptimalAction());
    }

    //-------------------------------------------------------------------- METODOS DE CONSULTA

    //-------------------------------- SOBRE TILES
    private Tile GetTileFrom(Vector2 position, Orientation orientation)
    {
        Tile tile = new Tile();
        tile.MakeNull();

        if (IsThereGroundAt(position))
            tile = new Tile(orientation, position, GetItemShapeColorAndConditionFrom(position));//GetReactionTo(GetShapeAndColor(position)));

        return tile;
    }
    private bool IsThereGroundAt(Vector2 position)
    {
        //Si no hay casilla de camino, no hay suelo...
        Vector3Int gridPosition = tilemap.WorldToCell(position);
        //Debug.Log("Se puede mover:" + tilemap.HasTile(gridPosition));
        return tilemap.HasTile(gridPosition);
    }

    //-------------------------------- SOBRE ITEMS
    private ItemShapeColorAndCondition GetItemShapeColorAndConditionFrom(Vector2 position)
    {
        ItemShapeColorAndCondition itemData = new ItemShapeColorAndCondition(ItemShape.NONE, ItemColor.NONE, ItemCondition.NONE);

        Collider2D hit = Physics2D.OverlapBox(position, tileSize, 0f, detectableLayerMasks);
        if (hit != null) itemData = hit.GetComponent<Item>().GetItemShapeColorAndCondition();

        return itemData;
    }

    private void NotifyItemThatHasBeenSteppedOn(Vector2 position) //notificar al item de que ah sido pisado 
    {
        Collider2D hit = Physics2D.OverlapBox(position, tileSize, 0f, detectableLayerMasks);
        if (hit != null) hit.GetComponent<Item>().NotifySteppedOn();
    }

    //-------------------------------- SOBRE ACCIONES
    private Action GetActionFromOrientation(Orientation orientation)
    {
        Action action = Action.NONE;
        switch (orientation)
        {
            case Orientation.LEFT:
                action = Action.TURN_LEFT;
                break;
            case Orientation.RIGHT:
                action = Action.TURN_RIGHT;
                break;
        }
        return action;
    }

    private Action GetActionFromReaction(ItemReaction reaction)
    {
        //Retorna una accion en funcion de una reaccion indicada
        switch (reaction)
        {
            case ItemReaction.NONE: return Action.WALK;

            case ItemReaction.EAT: return Action.EAT;

            case ItemReaction.JUMP: return Action.JUMP;

            case ItemReaction.EVADE: return Action.NONE;
        }

        return Action.NONE;
    }

    private Action GetOptimalAction() //metodo para obtener la accion optima a realizar segun el dataset en la posicion actual de RIA
    {
        //IMPLEMENTACION PREVIA: Deberia morir? ------------------------------------------------------------------------------------------------------------------------
        //Comprobamos el tile sobre el que estamos parados
        Tile currentTile;
        currentTile = GetTileFrom(transform.position, Orientation.NONE);

        if (!currentTile.IsNull()) { NotifyItemThatHasBeenSteppedOn(transform.position); } //si hay algun item, avisale que nos paramos sobre el
        if (currentTile.IsNull() || currentTile.IsLethal()) { return Action.DIE; } //si nos paramos sobre un item letal o donde no hay casilla, muere

        //PRIMERA IMPLEMENTACION: Tiles Consumibles ------------------------------------------------------------------------------------------------------------------
        //Conseguimos los tiles existentes...
        Vector2 frontPos = GetForwardPosition(1),
             leftPos = GetLeftPosition(1),
             rightPos = GetRightPosition(1);

        Tile frontTile = GetTileFrom(frontPos, Orientation.FRONT),
             leftTile = GetTileFrom(leftPos, Orientation.LEFT),
             rightTile = GetTileFrom(rightPos, Orientation.RIGHT);

        if (!frontTile.IsNull() && frontTile.IsEatable())
        {
            return Action.EAT;
        }
        else
        {
            //Sino juntamos las otras consumibles que hayan y elegimos una
            List<Tile> consumibles = new List<Tile>();
            if (leftTile.IsEatable()) consumibles.Add(leftTile);
            if (rightTile.IsEatable()) consumibles.Add(rightTile);

            //Si están las 2, elegimos al azar. Si solo hay una, elegimos la unica
            int totalConsumibles = consumibles.Count;
            if (totalConsumibles > 0) return GetActionFromOrientation(consumibles[(totalConsumibles == 2) ? GetRandomNumber() : 0].GetOrientation());
        }

        //SEGUNDA IMPLEMENTACION: Tiles Alternativas ------------------------------------------------------------------------------------------------------------------
        //Si no hay comestibles...Sigue la accion que te diga al frente (si hay casilla del frente y no te pide evitarla)!
        if (!frontTile.IsNull() && !frontTile.MustEvade()) return GetActionFromReaction(frontTile.GetReaction());

        //Si no se puede por el frente, busca otra ruta posible
        List<Tile> alternatives = new List<Tile>();

        //Considera las otras casillas como alternativas si es que existen y no deben evadirse
        if (!leftTile.IsNull() && !leftTile.MustEvade()) alternatives.Add(leftTile);
        if (!rightTile.IsNull() && !rightTile.MustEvade()) alternatives.Add(rightTile);

        //Dirigete a alguna de las alternativas que tengas
        int totalAlternatives = alternatives.Count;
        if (totalAlternatives > 0) return GetActionFromOrientation(alternatives[(totalAlternatives == 2) ? GetRandomNumber() : 0].GetOrientation());

        //TERCERA IMPLEMENTACION: Giro Aleatorio (ultima opcion) ----------------------------------------------------------------------------------------------------------------------
        //Si no hay ningún camino aparente, gira aleatoriamente
        return ((GetRandomNumber() == 0) ? Action.TURN_LEFT : Action.TURN_RIGHT);
    }

    //-------------------------------- OTRAS
    private Vector2 GetForwardPosition(int tilesAway) //posicion de aterrizaje al saltar
    {
        return GetDistantPosition(tilesAway, transform.up);
    }
    private Vector2 GetLeftPosition(int tilesAway) //posicion de aterrizaje al saltar
    {
        return GetDistantPosition(tilesAway, -transform.right);
    }
    private Vector2 GetRightPosition(int tilesAway) //posicion de aterrizaje al saltar
    {
        return GetDistantPosition(tilesAway, transform.right);
    }
    private Vector2 GetDistantPosition(int tilesAway, Vector2 direction)
    {
        //tilesAway = casillas de distancia entre RIA y la casilla consultada
        return transform.position + ((Vector3)direction * tilesAway);
    }
    private int GetRandomNumber() 
    {
        return UnityEngine.Random.Range(0, 2);
    }

    //------------------------------- DE ESTADO
    private bool IsOff() { return state == ActState.OFF; }
    private bool IsOnIdle() { return state == ActState.IDLE; }
    private bool IsMoving() { return state == ActState.MOVING; }
    private bool IsJumping() { return state == ActState.JUMPING; }
    private bool IsRotating() { return state == ActState.ROTATING; }
    private bool IsEating() { return state == ActState.EATING; }

    //---------------------------------------------------------------------ESTRUCTURAS ADICIONALES
    private struct Tile
    {
        private Orientation orientation;
        private Vector2 position;
        private ItemReaction reaction;
        private ItemCondition specialCondition;
        private bool isNull;

        public Tile(Orientation orientation, Vector2 position, ItemShapeColorAndCondition itemShapeColorAndCondition)
        {
            this.orientation = orientation;
            this.position = position;
            this.reaction = LevelManager.instance.GetDatasetReactionTo(itemShapeColorAndCondition.GetShapeAndColor());
            this.specialCondition = itemShapeColorAndCondition.GetCondition();
            isNull = false;
        }

        public void SetOrientation(Orientation orientation) { this.orientation = orientation; }
        public void SetPosition(Vector2 position) { this.position = position; }
        public void SetReaction(ItemReaction reaction) { this.reaction = reaction; }
        public void MakeNull()
        {
            isNull = true;
            this.orientation = Orientation.NONE;
            this.position = new Vector2(666, 666); //posicion invalida del diablo muajajajajaj
            this.reaction = ItemReaction.EVADE;
        }

        public Orientation GetOrientation() { return orientation; }
        public Vector2 GetPosition() { return position; }
        public ItemReaction GetReaction() { return reaction; }
        public bool IsNull() { return isNull; }

        //COMPROBACION RAPIDA DE REACCION
        public bool IsLethal() { return specialCondition == ItemCondition.LETHAL; }
        public bool IsEatable() { return reaction == ItemReaction.EAT; }
        public bool MustEvade() { return reaction == ItemReaction.EVADE; }
    }
}