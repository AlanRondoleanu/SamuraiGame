using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MAX_FLOW_SPEED = 4;
    private const float MAX_ROCK_SPEED = 2;
    private float speed = MAX_ROCK_SPEED;
    private Rigidbody2D rb;
    private PlayerScript player;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Key Events
        if (Input.GetKeyDown("q"))
        {
            if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
            {
                player.ChangeAttackMode(PlayerScript.AttackMode.Rock);
                speed = MAX_ROCK_SPEED;
            }
            else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
            {
                player.ChangeAttackMode(PlayerScript.AttackMode.Flow);
                speed = MAX_FLOW_SPEED;
            }

            Debug.Log("Mode Change");
        }
        else if (Input.GetKeyDown("e"))
        {
            if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
            {
                player.DashAttack();
            }
            else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
            {
                player.Deflect();
            }
        }
        else if (Input.GetKeyDown("space"))
        {
            if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
            {
                player.RangedAttack();
            }
            else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
            {
                player.MeleeAttack();
            }

            Debug.Log("Attack");
        }


        // Movement
        if (player.getDashing() == false)
        {
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(xMove, zMove) * speed;

            // State Changing for movement 
            if (xMove != 0 || zMove != 0)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }
    }
}
