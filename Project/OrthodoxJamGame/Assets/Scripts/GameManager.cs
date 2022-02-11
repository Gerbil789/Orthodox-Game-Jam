using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    Exploration,
    Battle
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void UpdateGameState(GameState newState){
        state = newState;

        switch(newState){
            case GameState.MainMenu:
                MainMenu();
                break;
            case GameState.Exploration:
                Exploration();
                break;
            case GameState.Battle:
                Battle();
                break;
            default: 
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }

    void MainMenu(){
        
    }

    void Exploration(){
           
    }

    void Battle(){

    }


}
