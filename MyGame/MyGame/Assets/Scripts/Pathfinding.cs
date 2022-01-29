using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding instance;

    public Vector3Int start, goal;
    public int speed;
    public Transform tilesObject;
    Stack<Vector3Int> path;
    HashSet<Tile> openList;
    HashSet<Tile> closedList;
    Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();
    LineRenderer lineRenderer;

    private void Awake() {
        instance = this;
        foreach (Transform t in tilesObject)
        {
            Tile tile = t.GetComponent<Tile>();
            if(tile.isActiveAndEnabled){
                tiles.Add(tile.pos, tile);
            }
            
        } 
    }

    void Start()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
    }

    public Tile GetTile(Vector3Int pos){
        if(tiles.ContainsKey(pos)){
            return tiles[pos];
        }
        return null;
    }

    public Stack<Vector3Int> GetPath(Vector3Int _goal){
        if(_goal == start){
            return null;
        }

        goal = _goal;


        Tile current;
        current = GetTile(start);
        openList = new HashSet<Tile>();
        closedList = new HashSet<Tile>();
        path = null;
        openList.Add(current);

        while(path == null){
            List<Tile> neighbours = FindNeighbours(current.pos);
            ExamineNeighbours(neighbours, current);
            UpdateCurrentTile(ref current);
            if(current.pos == goal){
                Stack<Vector3Int> reversePath = new Stack<Vector3Int>();
                while(current.pos != start){
                    reversePath.Push(current.pos);  
                    current = current.parent;
                }  
            reversePath.Push(start);
            path = new Stack<Vector3Int>();
            while(reversePath.Count > 0){
                path.Push(reversePath.Pop());
            }
            DrawLine();
            return path;
            }
        }
        Debug.Log("No path available");
        return null;
    }

    public List <Tile> FindNeighbours(Vector3Int parentPos){
        List<Tile> neighbors = new List<Tile>();
        for(int x = -1 ; x <= 1; x++){
            for(int z = -1 ; z <= 1; z++){
                Vector3Int neighborPos = new Vector3Int(parentPos.x + x, 0, parentPos.z + z);
                if(x !=0 || z !=0){ 
                    if(neighborPos != start){
                        if(tiles.ContainsKey(neighborPos)){
                            Tile t = GetTile(neighborPos);
                            neighbors.Add(t);
                            
                        }
                    }   
                }
            }
        }
        return neighbors;
    }

    void ExamineNeighbours(List<Tile> neighbours, Tile current){
        for(int i = 0; i < neighbours.Count; i++){
            if(neighbours[i].unit != null){
                continue;
            }
            Tile neighbour = neighbours[i];
            int gScore = DetermineGScore(neighbours[i].pos, current.pos);
            if(openList.Contains(neighbour)){
                if(current.G + gScore < neighbour.G){
                    CalcValues(current, neighbour, gScore);
                }
            }else if(!closedList.Contains(neighbour)){
                CalcValues(current, neighbour, gScore);
                openList.Add(neighbour);
            }  
        }   
    }

    int DetermineGScore(Vector3Int neighbourPos, Vector3Int currentPos){
        int gScore = 0;
        int x = currentPos.x - neighbourPos.x;
        int z = currentPos.z - neighbourPos.z;
        if(Mathf.Abs(x-z) % 2 == 1){
            gScore = 10;
        }else{
            gScore = 14;
        }
        return gScore;
    }

    void CalcValues(Tile parent, Tile neighbour, int cost){
        neighbour.parent = parent;
        neighbour.G = parent.G + cost;
        neighbour.H = ((Mathf.Abs((neighbour.pos.x - goal.x)) + (Mathf.Abs(neighbour.pos.y - goal.y))) * 10);
        neighbour.F = neighbour.G + neighbour.H;
    }

    void UpdateCurrentTile(ref Tile current){
        openList.Remove(current);
        closedList.Add(current);

        if(openList.Count > 0){
            current = openList.OrderBy(x => x.F).First();
        }
    }

    void DrawLine(){
        lineRenderer.positionCount = 0;
        Stack<Vector3Int> linePoints = new Stack<Vector3Int>(path);
        lineRenderer.positionCount = linePoints.Count;
        int count = linePoints.Count;
        for(int i = 0; i < count; i++){
            lineRenderer.SetPosition(i, linePoints.Pop() + new Vector3(0f, 0.1f, 0f));
        }
    }

    public void RemoveLine(){
        lineRenderer.positionCount = 0;
    }



    public void GetAvailableTiles(int _speed, Vector3Int _start){
        setAllTiles(false);
        foreach(var t in tiles.Values){
            t.G = 0;
        }
        start = _start;
        speed = _speed;
        List<Tile> availableTiles = new List<Tile>();
        availableTiles.Add(GetTile(start));
        int i = 0;
        while(availableTiles.Count > i){
            Tile current = availableTiles[i];
            List<Tile> neighbours = FindNeighbours(current.pos);
            for(int j = 0; j < neighbours.Count; j++){
                Tile neighbour = neighbours[j];
                int gScore = DetermineGScore(neighbour.pos, current.pos);
                if(availableTiles.Contains(neighbour)){
                    if(current.G + gScore < neighbour.G){
                        neighbour.parent = current;
                        neighbour.G = current.G + gScore;
                    }
                }else{
                    neighbour.parent = current;
                    neighbour.G = current.G + gScore;
                    if(neighbour.G <= speed * 10){
                        availableTiles.Add(neighbour);
                    }
                }         
            }
            i++;
        }
        availableTiles.Remove(GetTile(start));
        
        
        foreach (Tile t in availableTiles){
            
            t.inRange = true;
            if(t.unit == null){
                t.LightGreen();
            }else{
                t.DarkGreen();
            }

             
        }  
        
    }


    void setAllTiles(bool a){
        foreach (var t in tiles.Values)
        {
            t.inRange = a;
            t.DarkGreen();
        }
    }

    public void DisableAllTiles(bool active){
        //tilesObject.gameObject.SetActive(active);

        foreach (var t in tiles.Values)
        {
            
            t.DarkGreen();
        }
    }
}
