using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool enemy = true;

    public int health;
    public int attackDamage;
    public int armor;
    public int speed;
    public int initiative = 5;

    public Vector3Int pos;
    Vector3 dirNormalized = Vector3.zero;
    [SerializeField] Transform unitBody;
    Vector3 dir = Vector3.zero;
    public Vector3Int target; 
    public Tile attackTarget;
    Stack<Vector3Int> path;
    [SerializeField] float movementSpeed = 1f;
    public bool moving = false;

   
    void Update()
    {
        if(moving){
            if(path.Count > 0){
            
                target = path.Peek();
                dir = (target - transform.position);
                dirNormalized = dir.normalized;
                unitBody.transform.rotation = Quaternion.Slerp (unitBody.transform.rotation, Quaternion.LookRotation (dir), Time.deltaTime * 40f);
            
                if(Vector3.Distance(target, transform.position) <= 0.03f){
                    target = path.Pop();  
                }else{
                    transform.Translate(dirNormalized * Time.deltaTime * movementSpeed, Space.Self);
                }
            }else{
                moving = false;
                
                Pathfinding.instance.GetTile(pos).unit = null;
                pos = Vector3Int.RoundToInt(transform.localPosition);
                transform.localPosition = pos;
                Pathfinding.instance.GetTile(pos).unit = this;
                
                Pathfinding.instance.DisableAllTiles(true);
                if(attackTarget != null){
                    attackTarget.unit.TakeDamage(attackDamage);
                }
                GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
                
            }  

        } 
    }

    public void Move(Vector3Int _target, Tile _attackTarget){
        target = _target;
        attackTarget = _attackTarget;
        path = new Stack<Vector3Int>(Pathfinding.instance.GetPath(target));
        Pathfinding.instance.DisableAllTiles(false);
        moving = true;
        Pathfinding.instance.RemoveLine();
        GameManager.Instance.UpdateGameState(GameState.Action);
        
    }

    public void TakeDamage(int damage){
        health -= damage;
        if(health <= 0){
            Pathfinding.instance.GetTile(pos).unit = null;
            Destroy(gameObject);
        }
    }



}
