using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullScript : MonoBehaviour
{
    private GameObject player;
    private Animator animator;
    private EnemyScript enemy;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    public AudioClip[] clips;

    public float speed = 3;
    public float distance = 5;
    private float backAwayDistance = 3;

    private const float ATTACK_COOLDOWN = 5.0f;
    private float attackTimer = ATTACK_COOLDOWN;
    private bool isAttackReady = false;
    private bool isAttacking = false;
    private Vector3 target;
    private const float ATTACK_SPEED = 30;

    private const float ATTACK_LIFETIME = 0.5f;
    private float lifetimeTimer;
    private bool attackActive = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<EnemyScript>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        animator.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown Timer
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0)
        {
            attackActive = false;
        }

        // Cooldown Timer
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            isAttackReady = true;
        }

        if (enemy.getDead() == false)
        {

            if (isAttacking == false &&
                Vector2.Distance(player.transform.position, transform.position) > distance + 1)
            {
                enemy.goTo(player.transform.position, speed);
                enemy.turnTowardsDirection(player.transform.position);

                animator.SetBool("Moving", true);
            }
            else if (Vector2.Distance(player.transform.position, transform.position) < distance)
            {
                enemy.resetAgent();
            }


            if (Vector2.Distance(player.transform.position, transform.position) < backAwayDistance)
            {
                Vector3 direction = player.transform.position - transform.position;
                direction = direction.normalized * 1.5f;

                transform.Translate(-direction * Time.deltaTime);
            }

            if (isAttackReady == true && Vector2.Distance(player.transform.position, transform.position) < distance)
            {
                enemy.turnTowardsDirection(player.transform.position);
                animator.SetBool("Moving", false);

                isAttackReady = false;
                attackTimer = ATTACK_COOLDOWN;
                isAttacking = true;
                target = player.transform.position;

                // Moves the enemy before attacking, for flavor
                Vector3 direction = (target - transform.position).normalized;
                rb.velocity = (-direction * 3);

                Invoke("Attack", 0.5f);
            }
        }
    }

    void Attack()
    {
        if (enemy.getDead() == false)
        {
            Vector3 direction = (target - transform.position).normalized;
            rb.velocity = (direction * ATTACK_SPEED);

            isAttacking = false;
            attackActive = true;
            lifetimeTimer = ATTACK_LIFETIME;

            audioSource.PlayOneShot(clips[0]);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (enemy.getDead() == false && collision.gameObject.tag == "PlayerDeflect" && attackActive)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce((transform.position - collision.transform.position).normalized * 400);

            enemy.TakeDamage(GameData.instance.skullDamage * GameData.instance.reflectDamageMultipler);

            // Animates the player to show successful deflect
            player.GetComponent<PlayerScript>().deflectSuccess();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemy.getDead() == false && collision.gameObject.tag == "Player" && attackActive)
        {
            player.GetComponent<PlayerScript>().TakeDamage(transform.position, GameData.instance.skullDamage, GameData.instance.skullKnockback);

            Vector3 direction = (target - transform.position).normalized;
            rb.velocity = (-direction * ATTACK_SPEED * 0.2f);
            animator.SetBool("Moving", true);
        }
    }
}
