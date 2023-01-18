using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    // Player
    public float meleeDamage;
    public float meleeKnockback;
    public float dashDamage;
    public float dashKnockback;
    public float rangedDamage;
    public float rangedKnockback;
    public float reflectSpeedMultipler;
    public float reflectDamageMultipler;

    // Enemy
    public float enemyRangedDamage;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

}