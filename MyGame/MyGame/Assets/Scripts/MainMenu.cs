using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    


    public void NewGame(){
        GameManager.Instance.UpdateGameState(GameState.LoadScene);
    }
}