using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 50;
    public bool dead = false;
    public GameObject dashCutEffect;
    public GameObject dashKillEffect;

    //Ranged Attack
    public GameObject enemyProjectile;

    private float rangedTimer;
    private const float MAX_RANGED_TIMER = 3.0f;
    private Animator animator;
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Ranged Timer
        rangedTimer += Time.deltaTime;
        if (rangedTimer >= MAX_RANGED_TIMER)
        {
            RangedAttack();
        }

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

            Debug.Log(t_damage + " Damage");
        }
    }

    void RangedAttack()
    {
        GameObject projectile = Instantiate(enemyProjectile);

        // Direction
        Vector2 projectileDirection = Vector3.left;

        projectile.GetComponent<ProjectileScript>().ReadyProjectile(transform.position, projectileDirection, 0, GameData.instance.enemyRangedDamage);

        rangedTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead == false)
        {
            if (collision.gameObject.tag == "PlayerAttack")
            {
                TakeDamage(GameData.instance.meleeDamage);
                rb.AddForce((transform.position - player.transform.position).normalized * GameData.instance.meleeKnockback);
            }
            else if (collision.gameObject.tag == "PlayerRangedAttack")
            {
                TakeDamage(collision.gameObject.GetComponent<ProjectileScript>().getDamage());
                rb.AddForce((transform.position - player.transform.position).normalized * GameData.instance.rangedKnockback);
            }

            if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerScript>().getDashing() == true)
            {
                TakeDamage(GameData.instance.dashDamage);

                if (health <= 0)
                {
                    collision.gameObject.GetComponent<PlayerScript>().resetDash();

                    GameObject killEffect = Instantiate(dashKillEffect);
                    killEffect.transform.position = transform.position;
                }
                else
                {
                    GameObject cutEffect = Instantiate(dashCutEffect);
                    cutEffect.transform.position = transform.position;
                }
            }
        }
    }

    private void turnTowardsDirection(Vector2 t_vector)
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
}
