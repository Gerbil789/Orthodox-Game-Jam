using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState {
    Initialization,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public class BattleManager : MonoBehaviour{
    public static BattleManager Instance;
    public BattleState state;
    public static event Action<BattleState> OnBattleStateChanged;

    Ray ray;
    RaycastHit hit;
    Camera cam;
    [SerializeField] LayerMask layer;

    public Unit activeUnit = null; //set in inspector (for now)
    [SerializeField] Color yellow;
    [SerializeField] Color lightGreen;
    [SerializeField] Color darkgreen;
    [SerializeField] Color red;

    List<Tile> availableTiles = new List<Tile>();
    List<Tile> enemyRangeTiles = new List<Tile>();
    List<Tile> enemyOccupiedTiles = new List<Tile>();
    

    [Header("Debug")]
    public Tile destination = null; // tile to move
    public Tile target = null;      // tile to attack

    

    private void Awake()
    {
        Instance = this;
        cam = FindObjectOfType<Camera>();
    }

    public void UpdateBattleState(BattleState newState){
        state = newState;

        switch(newState){
            case BattleState.Initialization:
                Initialization();
                break;
            case BattleState.PlayerTurn:
                PlayerTurn();
                break;
            case BattleState.EnemyTurn:
                EnemyTurn();
                break;
            case BattleState.Victory:
                Victory();
                break;
            case BattleState.Defeat:
                Defeat();
                break; 
            default: 
                break;
        }
        OnBattleStateChanged?.Invoke(newState);
    }

    private void Start() {
        UpdateBattleState(BattleState.Initialization);
    }

    private void Update() {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if(state == BattleState.PlayerTurn){
            if(Input.GetKeyDown(KeyCode.Mouse0)){ // left mouse button
                if(destination != null){
                    activeUnit.Move(destination.pos);
                    destination = null;
                    UpdateBattleState(BattleState.EnemyTurn);
                    return;
                }
            }

            if(Physics.Raycast(ray, out hit, 100f, layer) == true){
                Tile t = hit.transform.GetComponent<Tile>();
                if(t == destination || t == target){
                    return;
                }

                if(enemyOccupiedTiles.Contains(t)){
                    target = t;
                    enemyRangeTiles = Pathfinding.Instance.GetAvailableTiles(t.unit.speed, t.pos);
                    foreach(Tile n in enemyRangeTiles){
                        if(!availableTiles.Contains(n)){
                            n.SetOutlineColor(darkgreen, 0.8f);
                        }
                        n.SetFillColor(darkgreen, 0.5f);
                    }
                    return;
                }

                foreach(Tile n in enemyRangeTiles){
                    n.SetPreviousColor();
                }
                enemyRangeTiles = new List<Tile>();
                target = null;
                
                if(availableTiles.Contains(t)){
                    destination?.SetPreviousColor();
                    destination = t;
                    destination.SetOutlineColor(yellow, 0.8f);
                    destination.SetFillColor(yellow, 0.5f);
                    return;
                }

                destination?.SetPreviousColor();
                destination = null;
               
                
                
            }else{
                destination?.SetPreviousColor();
                destination = null;
                target?.SetPreviousColor();
                target = null;
                foreach(Tile n in enemyRangeTiles){
                    n.SetPreviousColor();
                }
                enemyRangeTiles = new List<Tile>();
                return;
            }
        }

        
    }

    //no use for this function at the moment, but it will be usefull later, so pls dont delete it xd
    bool IsDiagonal(Vector3Int pos1, Vector3Int pos2){
        int x = pos1.x - pos2.x;
        int z = pos1.z - pos2.z;
        if(Mathf.Abs(x-z) % 2 == 1){
            return false;
        }else{
            return true;
        }
    }

    void Initialization(){
        foreach(var t in Pathfinding.Instance.tiles.Values){
            lightGreen.a = 0.1f;
            t.outlineColor = lightGreen;
            lightGreen.a = 0.05f;
            t.fillColor = lightGreen;
            t.SetOutlineColor(lightGreen, 0.1f);
            t.SetFillColor(lightGreen, 0.05f);
            if(t.unit){
                if(t.unit.enemy){
                    enemyOccupiedTiles.Add(t);
                }
            }
        } 

        UpdateBattleState(BattleState.PlayerTurn);
    }

    void PlayerTurn(){
        //set avaiable tiles color
        availableTiles = Pathfinding.Instance.GetAvailableTiles(activeUnit.speed, activeUnit.pos);
        foreach(Tile t in availableTiles){
            if(enemyOccupiedTiles.Contains(t)){
                continue;
            }
            lightGreen.a = 0.8f;
            t.outlineColor = lightGreen;
            lightGreen.a = 0.5f;
            t.fillColor = lightGreen;
            t.SetOutlineColor(lightGreen, 0.8f);
            t.SetFillColor(lightGreen, 0.5f);
        }

        foreach(Tile t in enemyOccupiedTiles){
            t.SetOutlineColor(red, 0.8f);
            t.SetFillColor(red, 0.5f);
        }
    }

    void EnemyTurn(){
        foreach(var t in Pathfinding.Instance.tiles.Values){ //set all tiles color
            lightGreen.a = 0.1f;
            t.outlineColor = lightGreen;
            lightGreen.a = 0.05f;
            t.fillColor = lightGreen;
            t.SetOutlineColor(lightGreen, 0.1f);
            t.SetFillColor(lightGreen, 0.05f);
        }
    }

    void Victory(){

    }

    void Defeat(){

    }
}
