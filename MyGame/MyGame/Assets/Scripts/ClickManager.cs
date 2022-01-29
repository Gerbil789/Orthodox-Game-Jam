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

    List<Tile> neighbours = new List<Tile>();

    void Update(){
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(GameManager.Instance.state == GameState.PlayerTurn){
            if(Physics.Raycast(ray, out hit, 100f, layer)){
                Tile t = hit.transform.GetComponent<Tile>();
                
                if(t != target && t != null){
                    Pathfinding.instance.GetTile(GameManager.Instance.activeUnit.pos).Yellow();
                    if(t.inRange){
                        if(target != null) target.LightGreen();
                        
                        if(t.unit == null){
                            t.Yellow();
                            target = t;
                        }else{
                            
                            neighbours = Pathfinding.instance.FindNeighbours(t.pos); //find empty tiles
                            if(neighbours.Count > 0){
                
                                if(attack != null){
                                    attack.DarkGreen();
                                }
                                attack = t;
                                attack.Red();
                                float shortestDist = 10f;
                                for(int i = 0; i < neighbours.Count; i++){
                                    if(neighbours[i].unit != null){
                                        continue;
                                    }
                                    float dist = Vector3.Distance(hit.point, neighbours[i].pos);
                                    if(IsDiagonal(t.pos, neighbours[i].pos)){
                                        dist -= 0.4f;
                                    }
                                    if(dist < shortestDist){
                                        shortestDist = dist;
                                        target = neighbours[i];
                                    }

                                   
                                    
                                }
                                target.Yellow();
                                neighbours = new List<Tile>();
                            }

                        }

                       
                        
                        Pathfinding.instance.GetPath(target.pos);
                        
                    }else{
                        if(target != null) target.LightGreen();
                        target = null;
                        if(attack != null){
                            attack.DarkGreen();
                            attack = null;
                        }
                        Pathfinding.instance.RemoveLine();
                    }
                }

                
                    

                if(target != null){
                    if(Input.GetMouseButtonDown(0)){
                        GameManager.Instance.activeUnit.Move(target.pos, attack);
                    }
                }
                
            }else{
                if(target != null) target.LightGreen();
                target = null;
                if(attack != null){
                    attack.DarkGreen();
                    attack = null;
                }
                Pathfinding.instance.RemoveLine();
            }
        }
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
