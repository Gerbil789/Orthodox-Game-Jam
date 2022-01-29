using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    LoadScene, 
    PlayerTurn,
    EnemyTurn,
    Action,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    public GameState state;

    public static event Action<GameState> OnGameStateChanged;

    public Transform unitsGO;
    List<Unit> units = new List<Unit>();
    public Unit activeUnit = null;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start() {
        foreach (Transform t in unitsGO){
            Unit u = t.GetComponent<Unit>();
            if(u != null){
                units.Add(u);
            }
        } 
        activeUnit = units[0];

        foreach (Unit u in units)
        {
            Pathfinding.instance.GetTile(u.pos).unit = u;
            //Pathfinding.instance.GetTile(u.pos).inRange = false;
        }
        UpdateGameState(GameState.PlayerTurn);

        
       
        
        
    }


    public void UpdateGameState(GameState newState){
        state = newState;

        switch(newState){
            case GameState.MainMenu:
                break;
            case GameState.LoadScene:
                HandleLoadScene();
                break;
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Action:
                HandleAction();
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default: 
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    void HandleLoadScene(){
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("GameScene");
        
        UpdateGameState(GameState.PlayerTurn);
    }

    void HandlePlayerTurn(){
        Debug.Log("PlayerTurn!");

        

  
       
        
        Pathfinding.instance.GetAvailableTiles(activeUnit.speed, activeUnit.pos);
        
       
        


        
    }

    void HandleAction(){
        print("Action!");
        
    }
}
