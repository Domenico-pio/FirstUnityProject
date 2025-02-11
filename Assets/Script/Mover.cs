using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    // Movimenti base
    protected BoxCollider2D BoxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;

    // Sistema Wandering (cammina in maniera random senza allontanarsi troppo dal punto di partenza)
    public float wanderingMoveSpeed = 0.08f;
    public float wanderingMinTime=1.0f, wanderingMaxTime=4.0f;
    protected float wanderingDecisionTimeCount = -1.0f;
    public float maxWanderingDistance = 0.5f;

    protected Vector3 startingPosition;

    // Possibili direzioni in cui l'oggetto può muoversi. Zero c'è quattro volte per aumentare la probabilità che stia fermo
    protected Vector3[] moveDirections = new Vector3[] {  new Vector3(1.0f,0,0), 
                                                        new Vector3(-1.0f,0,0), 
                                                        new Vector3(0,1.0f,0), 
                                                        new Vector3(0,-1.0f,0), 
                                                        new Vector3(0,0,0), 
                                                        new Vector3(0,0,0), 
                                                        new Vector3(0,0,0), 
                                                        new Vector3(0,0,0)      };
    protected int firstCurrentWanderingMoveDirection = 0;
    protected int secondCurrentWanderingMoveDirection = 0; //uso due scelte per aumentare la varietà di mosse (adesso può andare anche in diagonale e può variare anche la velocità)
    protected Vector3 currentWanderingMove;
    /*  Con le scelte correnti le probabilità sono le seguenti
        *   Oggetto fermo = 25% + 6.25% = 31.25%
        *   Oggetto in movimento nelle 4 direzioni principali (lento) = 50%
        *   Oggetto in movimento nelle 4 direzioni principali (veloce) = 6.25%
        *   Oggetto in movimento in diagonale = 12.5%
    */    

    protected virtual void Start(){
        BoxCollider = GetComponent<BoxCollider2D>();
        
        startingPosition=this.transform.position;
    }

    protected void Wandering(){
        currentWanderingMove=moveDirections[firstCurrentWanderingMoveDirection]+moveDirections[secondCurrentWanderingMoveDirection];
        // Bisogna aggiungere che se supera una certa distanza dalla posizione di partenza allora devve tornare indietro
        // la soluzione più semplice a livello di codice probabilmente è se è lontano e questa sclta lo farà allontanare ancora allora cambia scelta
        if(Vector3.Distance(startingPosition,transform.position)>maxWanderingDistance){
            int i=0; //in genere non occorre ma un po' ogni tanto capita che rimanga un po' bloccato, quindi se in 10 iterazioni non riesce non fa niente (ci riproverà al prossimo frame)
            while (Vector3.Distance(startingPosition,transform.position+currentWanderingMove)>=Vector3.Distance(startingPosition,transform.position)& i<10)
            {
                i++;
                ChooseWanderingMoveDirection();
                currentWanderingMove=moveDirections[firstCurrentWanderingMoveDirection]+moveDirections[secondCurrentWanderingMoveDirection];
            }
        }

        // Faccio muovere l'oggetto
        UpdateMotor(currentWanderingMove* wanderingMoveSpeed);
        ComputeNextWanderingMove();
    }

    protected void ComputeNextWanderingMove(){
        if (wanderingDecisionTimeCount > 0) {
            wanderingDecisionTimeCount -= Time.deltaTime;
        }
        else{
            // Setto il tempo per cui durerà la prossima mossa
            wanderingDecisionTimeCount = Random.Range(wanderingMinTime, wanderingMaxTime);
 
            // Scelgo la direzione in cui si muoverà per i prossimi decisionTimeCount secondi
            ChooseWanderingMoveDirection();
        }
    }
 
    protected void ChooseWanderingMoveDirection()
    {
        // Scelgo randomicamente la direzione tra quelle impostate
        firstCurrentWanderingMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
        secondCurrentWanderingMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
    }


    protected virtual void UpdateMotor(Vector3 input){
        //Reset moveDelta
        moveDelta=input;

        //flip del personaggio a seconda che si vada a destra o a sinistra
        if(moveDelta.x>0){
            transform.localScale = new Vector3(1,1,1);
        }
        else{
            if (moveDelta.x<0){ 
                transform.localScale = new Vector3(-1,1,1);
            }
        }

        //controllo collisioni asse y
        hit=Physics2D.BoxCast(transform.position,BoxCollider.size,0,new Vector2(0,moveDelta.y),Mathf.Abs(moveDelta.y*Time.deltaTime),LayerMask.GetMask("Actor","Blocking"));

        if (hit.collider==null){
            transform.Translate(0,moveDelta.y*Time.deltaTime,0);        
        }

        //controllo collisioni asse x
        hit=Physics2D.BoxCast(transform.position,BoxCollider.size,0,new Vector2(moveDelta.x,0),Mathf.Abs(moveDelta.x*Time.deltaTime),LayerMask.GetMask("Actor","Blocking"));

        if (hit.collider==null){
            transform.Translate(moveDelta.x*Time.deltaTime,0,0);        
        }
    }
}
