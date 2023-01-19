using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmenScript : MonoBehaviour
{
    public float axisThreshold = 0.1f;

    private GameObject player;
    private EnemyScript enemy;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerScript playerScript;

    // Charge
    private float chargeLifetime;
    private const float CHARGE_LIFETIME = 0.2f;
    private float chargeCooldownTimer;
    private const float CHARGE_COOLDOWN = 3f;
    private bool charging = false;
    private const float CHARGE_SPEED = 25;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        enemy = GetComponent<EnemyScript>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (enemy.getDead() == false)
        {
            // Lifetimer Timers
            chargeLifetime -= Time.deltaTime;
            if (chargeLifetime <= 0.0f)
            {
                charging = false;
                animator.SetBool("Attacking", false);
            }

            // Cooldown Timers
            chargeCooldownTimer -= Time.deltaTime;
            if (chargeCooldownTimer <= 0.0f)
            {
                float playerY = player.transform.position.y;
                float enemyY = transform.position.y;

                if (playerScript.getDashing() == false && Mathf.Abs(playerY - enemyY) <= axisThreshold)
                {
                    enemy.turnTowardsDirection(player.transform.position);

                    Invoke("setChargeDelay", 1);

                    // Animation for charge
                    if (chargeCooldownTimer <= 0.0f)
                        Invoke("setChargeAnimationDelay", 0.4f);
                }
            }
        }
    }

    void setChargeDelay()
    {
        charging = true;
        chargeLifetime = CHARGE_LIFETIME;

        Vector2 chargeDirection;
        if (player.transform.position.x > 0)
        {
            chargeDirection = Vector2.right;
        }
        else
        {
            chargeDirection = Vector2.left;
        }

        rb.velocity = (chargeDirection * CHARGE_SPEED);
    }

    void setChargeAnimationDelay()
    {
        chargeCooldownTimer = CHARGE_COOLDOWN;
        animator.SetBool("Attacking", true);
    }
}
