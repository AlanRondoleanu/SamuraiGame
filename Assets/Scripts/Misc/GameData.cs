using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    // Player
    public float playerHealth;
    public float meleeDamage;
    public float meleeKnockback;
    public float dashDamage;
    public float dashKnockback;
    public float rangedDamage;
    public float rangedKnockback;
    public float reflectSpeedMultipler;
    public float reflectDamageMultipler;

    // Assassin
    public float assassinDamage;
    public float assassinKnockback;

    // Spearmen
    public float spearmenDamage;
    public float spearmenKnockback;

    // Skull
    public float skullDamage;
    public float skullKnockback;

    // Demon
    public float demonDamage;
    public float demonKnockback;
    public float demonRangedDamage;
}
