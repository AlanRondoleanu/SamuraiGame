using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MAX_FLOW_SPEED = 4.25f;
    private const float MAX_ROCK_SPEED = 3.5f;
    private float speed = MAX_ROCK_SPEED;
    private bool ready = false;
    private PlayerScript player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public ParticleSystem particleSystem;
    private ParticleSystemRenderer psr;
    private Color originalColor;

    void Start()
    {
        player = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        psr = particleSystem.GetComponent<ParticleSystemRenderer>();
        originalColor = spriteRenderer.color;
        particleSystem.Stop();
        Invoke("playerReady", 1);
    }

    void Update()
    {
        if (player.getAlive() && ready)
        {
            // Key Events
            if (Input.GetKeyDown("q"))
            {
                if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
                {
                    player.ChangeAttackMode(PlayerScript.AttackMode.Rock);
                    speed = MAX_ROCK_SPEED;
                    particleSystem.Stop();
                }
                else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
                {
                    player.ChangeAttackMode(PlayerScript.AttackMode.Flow);
                    speed = MAX_FLOW_SPEED;
                    particleSystem.Play();
                }

                player.changeStance();
            }
            else if ((Input.GetMouseButtonDown(1)))
            {
                if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
                {
                    player.DashAttack();

                    turnTowardsMouse();
                }
                else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
                {
                    player.Deflect();

                    turnTowardsMouse();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
                {
                    player.RangedAttack();

                    turnTowardsMouse();
                }
                else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
                {
                    player.MeleeAttack();

                    turnTowardsMouse();
                }
            }


            // Movement
            if (player.getDashing() == false)
            {
                float xMove = Input.GetAxisRaw("Horizontal");
                float zMove = Input.GetAxisRaw("Vertical");

                Vector2 velocity = new Vector2(xMove, zMove) * speed;

                // Movement
                if (xMove != 0 && zMove != 0)
                {
                    transform.Translate(velocity * 0.66f * Time.deltaTime);
                }
                else
                {
                    transform.Translate(velocity * Time.deltaTime);
                }

                // State Changing for movement 
                if (xMove != 0 || zMove != 0)
                {
                    animator.SetBool("Running", true);
                    particleSystem.enableEmission = true;
                }
                else
                {
                    animator.SetBool("Running", false);
                    particleSystem.enableEmission = false;
                }

                // Sprite Flip
                if (player.getAttacking() == false && player.getDashing() == false)
                {
                    if (xMove > 0)
                    {
                        spriteRenderer.flipX = false;
                        Vector3 flip = new Vector3(0, 0, 0);
                        psr.flip = flip;
                    }
                    else if (xMove < 0)
                    {
                        spriteRenderer.flipX = true;
                        Vector3 flip = new Vector3(1, 0, 0);
                        psr.flip = flip;
                    }
                }
            }

            if (player.getImmune())
            {
                Color temp = spriteRenderer.color;
                temp.a = 0.5f;
                spriteRenderer.color = temp;
            }
            else
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void turnTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void playerReady()
    {
        ready = true;
        animator.SetBool("Ready", true);
    }
}
