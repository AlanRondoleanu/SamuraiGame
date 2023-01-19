using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 50;
    public GameObject dashCutEffect;
    public GameObject dashKillEffect;
    public GameObject healthBar;

    private bool dead = false;
    private float MAX_HEALTH;
    private Animator animator;
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCam;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");

        MAX_HEALTH = health;
    }

    void Update()
    {
        if (health <= 0 && dead == false)
        {
            animator.SetBool("Dead", true);
            dead = true;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void TakeDamage(float t_damage)
    {
        if (dead == false)
        {
            health -= t_damage;
            animator.SetTrigger("Hit");

            turnTowardsDirection(player.transform.position);

            // Health Bar
            float percent = (health / MAX_HEALTH) * 1;
            percent -= 1;
            healthBar.GetComponent<EnemyHealthBar>().setMask(percent);

            // Debug
            Debug.Log(t_damage + " Damage");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead == false)
        {
            if (collision.gameObject.tag == "PlayerAttack")
            {
                TakeDamage(GameData.instance.meleeDamage);
                rb.AddForce((transform.position - player.transform.position).normalized * GameData.instance.meleeKnockback);

                // Screenshake
                mainCam.GetComponent<ScreenShake>().ShakeCamera(0.4f);
            }
            else if (collision.gameObject.tag == "PlayerRangedAttack")
            {
                TakeDamage(collision.gameObject.GetComponent<ProjectileScript>().getDamage());
                rb.AddForce((transform.position - player.transform.position).normalized * GameData.instance.rangedKnockback);

                // Screenshake
                mainCam.GetComponent<ScreenShake>().ShakeCamera(0.2f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead == false)
        {
            if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerScript>().getDashing() == true)
            {
                TakeDamage(GameData.instance.dashDamage);

                if (health <= 0)
                {
                    collision.gameObject.GetComponent<PlayerScript>().resetDash();

                    GameObject killEffect = Instantiate(dashKillEffect);
                    killEffect.transform.position = transform.position;
                    // Screenshake
                    mainCam.GetComponent<ScreenShake>().ShakeCamera(0.4f);
                }
                else
                {
                    GameObject cutEffect = Instantiate(dashCutEffect);
                    cutEffect.transform.position = transform.position;
                    // Screenshake
                    mainCam.GetComponent<ScreenShake>().ShakeCamera(0.8f);
                }
            }
        }
    }

    public void turnTowardsDirection(Vector2 t_vector)
    {
        if (t_vector.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

    }

    public bool getDead()
    {
        return dead;
    }
}
