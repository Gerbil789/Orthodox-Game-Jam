using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3Int pos;
    public Unit unit = null;
    MeshRenderer outline;
    MeshRenderer fill;

    public int G {get; set;}
    public int H {get; set;}
    public int F {get; set;}
    public Tile parent {get; set;}

    public Color outlineColor;
    public Color fillColor;

    void Awake(){
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        outline = renderers[0];
        fill = renderers[1];
        pos = Vector3Int.RoundToInt(transform.localPosition);  
    }

    public void SetOutlineColor(Color col, float alpha){
        col.a = alpha;
        outline.material.color = col;
    }

    public void SetFillColor(Color col, float alpha){   
        col.a = alpha;
        fill.material.color = col;
    }

    public void SetPreviousColor(){
        outline.material.color = outlineColor;
        fill.material.color = fillColor;
    }
}
