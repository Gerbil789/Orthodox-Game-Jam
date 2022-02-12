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

    LineRenderer line;

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
        line = GetComponent<LineRenderer>();
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
                    UpdateBattleState(BattleState.EnemyTurn);
                    activeUnit.Move(destination.pos);
                    destination = null;
                    DeleteLine();
                    
                    return;
                }
            }

            if(Physics.Raycast(ray, out hit, 100f, layer) == true){
                Tile t = hit.transform.GetComponent<Tile>();
                if(t == destination){
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

                    List<Tile> neighbours = Pathfinding.Instance.FindNeighbours(target.pos);
                    List<Tile> availableNeighbours = new List<Tile>();
                    foreach(Tile n in neighbours){
                        if(availableTiles.Contains(n)){
                            availableNeighbours.Add(n);
                        }else if(Pathfinding.Instance.GetTile(activeUnit.pos) == n){
                            availableNeighbours.Add(n);
                        }
                    }
                    
                    
                    
                    if(availableNeighbours.Count > 0){
                        Tile dest = availableNeighbours[0];
                        float distance = 10f;
                        foreach(Tile n in availableNeighbours){
                            float dist = Vector3.Distance(n.pos, hit.point);
                            if(IsDiagonal(target.pos, n.pos)){
                                dist -= 0.41f;
                            }
                            if(dist < distance){
                                distance = dist;
                                dest = n;
                            }
                        }
                        destination?.SetPreviousColor();
                        destination = dest;
                        DrawLine(Pathfinding.Instance.GetPath(activeUnit.pos, destination.pos));
                        destination.SetOutlineColor(yellow, 0.8f);
                        destination.SetFillColor(yellow, 0.5f);
                    }
                    return;
                }

                foreach(Tile n in enemyRangeTiles){
                    n.SetPreviousColor();
                }
                //enemyRangeTiles = new List<Tile>();
                target = null;
                
                if(availableTiles.Contains(t)){
                    destination?.SetPreviousColor();
                    destination = t;
                    DrawLine(Pathfinding.Instance.GetPath(activeUnit.pos, destination.pos));
                    destination.SetOutlineColor(yellow, 0.8f);
                    destination.SetFillColor(yellow, 0.5f);
                    return;
                }

                destination?.SetPreviousColor();
                destination = null;
                DeleteLine();
               
                
                
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

    void DrawLine(Stack<Vector3Int> path){
        if(path.Count <= 0){
            return;
        }
        line.positionCount = path.Count;
        for(int i = 0; i < line.positionCount; i++){
            Vector3 pos = path.Pop();
            pos.y = 0.1f;
            line.SetPosition(i, pos);
        }
        
    }

    void DeleteLine(){
        line.positionCount = 0;
    }

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

        foreach(var t in Pathfinding.Instance.inactiveTiles.Values){
            lightGreen.a = 0.1f;
            t.outlineColor = lightGreen;
            lightGreen.a = 0.05f;
            t.fillColor = lightGreen;
            t.SetOutlineColor(lightGreen, 0.1f);
            t.SetFillColor(lightGreen, 0.05f);
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
