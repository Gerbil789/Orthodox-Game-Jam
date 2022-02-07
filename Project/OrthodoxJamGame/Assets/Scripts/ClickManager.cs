using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    public Transform cam;
    [SerializeField] LayerMask layer;

    public List<Tile> neighbourTiles;

    private void Awake() {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        
    }

    private void OnDestroy() {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    void GameManagerOnGameStateChanged(GameState state){
        
    }

    public Tile target = null;
    public Tile attack = null;

    [SerializeField] List<Tile> neighbours = new List<Tile>();

    void Update(){
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(GameManager.Instance.state == GameState.PlayerTurn){
            if(Physics.Raycast(ray, out hit, 100f, layer)){
                Tile t = hit.transform.GetComponent<Tile>();
                
                if(t != null){
                    
                    Pathfinding.instance.GetTile(GameManager.Instance.activeUnit.pos).Yellow();
                    SetColors(); //set colors of "target" and "attack", set them to NULL (to prevent some stupid errors :D)

                    if(IsEnemy(t)){
                        if(FindNeighbours(t, 2)){ // find "empty" and "in range" tiles
                            attack = t;
                            attack.Red();
                            FindClosestTile(t, hit.point, 0); // sets "target"
                            target.Yellow();
                            Pathfinding.instance.GetPath(target.pos);
                           
                        }
                    }else{
                        if(t.inRange){
                            target = t;
                            target.Yellow();
                            Pathfinding.instance.GetPath(target.pos);
                            if(FindNeighbours(target, 0)){ // find enemies
                                if(FindClosestTile(target, hit.point, 1)){ // sets "attack"
                                    attack.Red();
                                } 
                                
                            }
                        }else{
                            Pathfinding.instance.RemoveLine();
                        }
                    }  
                
                }

                if(target != null){
                    if(Input.GetMouseButtonDown(0)){
                        GameManager.Instance.activeUnit.Move(target.pos, attack);
                    }
                }
                
            }else{
                SetColors();
                Pathfinding.instance.RemoveLine();
            }
        }
    }

    bool IsEnemy(Tile t){
        return t.unit;
    }

    bool FindNeighbours(Tile t, int index){
        neighbours = new List<Tile>();
        

        if(index == 0){ //find enemies
            List<Tile> newNeighbours = new List<Tile>();
            newNeighbours = Pathfinding.instance.FindNeighbours(t.pos);
            for(int i = 0; i < newNeighbours.Count; i++){
                if(newNeighbours[i].unit != null){
                    neighbours.Add(newNeighbours[i]);
                }
            }

        }else if(index == 1){ // find empty tiles
            neighbours = Pathfinding.instance.FindNeighbours(t.pos);
            for(int i = 0; i < neighbours.Count; i++){
                if(neighbours[i].unit != null){
                    neighbours.Remove(neighbours[i]);
                }
            }
        }else if(index == 2){ // find empty and in range tiles
            neighbours = Pathfinding.instance.FindNeighbours(t.pos);
            for(int i = 0; i < neighbours.Count; i++){
                if(neighbours[i].unit != null || !neighbours[i].inRange){
                    neighbours.Remove(neighbours[i]);
                }
            }
        }
        if(neighbours.Count > 0){
             return true;
        }
        return false;
    }

    void SetColors(){
        if(attack){attack.DarkGreen();}
        if(target){target.LightGreen();}
        
        attack = null;
        target = null;
    }

    bool FindClosestTile(Tile t ,Vector3 mousePos, int index){


        if(index == 0){
            float shortestDist = 10f;
                for(int i = 0; i < neighbours.Count; i++){
                    float dist = Vector3.Distance(mousePos, neighbours[i].pos);
                    if(IsDiagonal(t.pos, neighbours[i].pos)){
                        dist -= 0.4f;
                    }
                    if(dist < shortestDist){
                        shortestDist = dist;
                        target = neighbours[i];
                        
                    }
                }
            neighbours = new List<Tile>();
            return true;
        }else if(index == 1){
            float shortestDist = 10f;
            for(int i = 0; i < neighbours.Count; i++){
                float dist = Vector3.Distance(mousePos, neighbours[i].pos);
                if(IsDiagonal(t.pos, neighbours[i].pos)){
                    dist -= 0.4f;
                }
                if(dist < shortestDist && dist <= 0.85f){
                    shortestDist = dist;
                    attack = neighbours[i];
                }
                    
            }
            neighbours = new List<Tile>();
            if(shortestDist <= 0.85f){
                return true;
            }else{
                return false;
            } 
        }
        return false;
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
}
