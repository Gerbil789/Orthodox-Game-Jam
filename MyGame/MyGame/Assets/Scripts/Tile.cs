using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Color darkGreen;
    [SerializeField] Color lightGreen;
    [SerializeField] Color yellow;
    [SerializeField] Color red;

    public bool inRange = false;
    public Unit unit = null;
    public Vector3Int pos;
    MeshRenderer meshRenderer;

    public int G {get; set;}
    public int H {get; set;}
    public int F {get; set;}
    public Tile parent {get; set;}

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        pos = Vector3Int.RoundToInt(transform.localPosition);
        SetColor(darkGreen);
    }

    public void SetColor(Color col){
        meshRenderer.material.color = col;
    }

    public void DarkGreen(){
        SetColor(darkGreen);
    }

    public void LightGreen(){
        SetColor(lightGreen);
        
    }

    public void Red(){
        SetColor(red);
    }

    public void Yellow(){
        SetColor(yellow);
        
    }
}
