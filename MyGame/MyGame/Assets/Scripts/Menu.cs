using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject menu;

    private void Start() {
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(Time.timeScale == 1){
                Time.timeScale = 0;
                menu.SetActive(true);
            }else{   
                menu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene(0);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
