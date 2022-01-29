using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public TMPro.TMP_Text health;
    public TMPro.TMP_Text attack;
    public TMPro.TMP_Text armor;
    public TMPro.TMP_Text speed;


    public void setValues(Unit u){
        health.text = u.health.ToString();
        attack.text = u.attackDamage.ToString();
        armor.text = u.armor.ToString();
        speed.text = u.speed.ToString();
    }
}
