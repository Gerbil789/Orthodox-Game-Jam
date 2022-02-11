using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool enemy;

    public int health = 50;
    public int attackDamage = 10;
    public int armor = 5;
    public int speed = 5; // how many tiles can unit move
    public int initiative = 5;

    [SerializeField] float movementSpeed = 1f; // how fast can unit move
    [SerializeField] Transform unitBody;

    
    Vector3 dirNormalized = Vector3.zero;
    Vector3 dir = Vector3.zero;
    Stack<Vector3Int> path;

    [Header("Debug:")]
    [SerializeField] bool moving = false;
    public Vector3Int pos;
    public Vector3Int destination; 

   
    void Update(){
        if(moving){
            if(path.Count > 0){
                destination = path.Peek();
                dir = (destination - transform.position);
                dirNormalized = dir.normalized;
                unitBody.transform.rotation = Quaternion.Slerp (unitBody.transform.rotation, Quaternion.LookRotation (dir), Time.deltaTime * 40f);
            
                if(Vector3.Distance(destination, transform.position) <= 0.02f){
                    destination = path.Pop();  
                }else{
                    transform.Translate(dirNormalized * Time.deltaTime * movementSpeed, Space.Self);
                }
            }else{
                moving = false;
                Pathfinding.Instance.GetTile(pos).unit = null;
                pos = Vector3Int.RoundToInt(transform.localPosition);
                transform.localPosition = pos;
                Pathfinding.Instance.GetTile(pos).unit = this;    
                BattleManager.Instance.UpdateBattleState(BattleState.PlayerTurn);    
            }  
        } 
    }

    public void Move(Vector3Int _destination){
        //this.destination = destination;
        path = new Stack<Vector3Int>(Pathfinding.Instance.GetPath(_destination));
        moving = true;
    }

    public void TakeDamage(int damage){
        damage -= armor;
        if(damage > 0){
            health -= damage;
            if(health <= 0){
                Pathfinding.Instance.GetTile(pos).unit = null;
                Debug.Log(gameObject.name + " was destroyed.");
                Destroy(gameObject);
            }
        }
    }
}
