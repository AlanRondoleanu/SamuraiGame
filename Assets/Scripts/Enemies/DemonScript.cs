using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonScript : MonoBehaviour
{
    private GameObject player;
    private Animator animator;
    private EnemyScript enemy;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    public AudioClip[] clips;
    public GameObject enemyBase;

    public GameObject attackHitBox;
    public float speed = 3;

    private float stopRunDistance = 7;
    private float runAwayDistance = 4;
    public float attackDistance = 2;

    // Movement
    private Vector3 direction;
    private bool directionReady = true;
    private const float RANDOM_MOVEMENT_COOLDOWN = 2.0f;
    private float randomMovementTimer = RANDOM_MOVEMENT_COOLDOWN;

    // Fire Attack
    private const float ATTACK_COOLDOWN = 5.0f;
    private float attackTimer = ATTACK_COOLDOWN;
    private const float ATTACK_LIFETIME = 0.7f;
    private bool isAttackReady = false;
    private bool isAttacking = false;
    private float lifetimeTimer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<EnemyScript>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(enemyBase.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        animator.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {
        // Life Timer
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0 && attackHitBox.activeInHierarchy == true)
        {
            attackHitBox.gameObject.SetActive(false);
        }

        // Cooldown Timer
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            isAttackReady = true;
        }
        randomMovementTimer -= Time.deltaTime;
        if (randomMovementTimer <= 0)
        {
            directionReady = true;
        }

        if (enemy.getDead() == false)
        {
            //// Move towards player if out of range
            //if (isAttacking == false &&
            //    Vector2.Distance(player.transform.position, transform.position) > chasePlayerDistance )
            //{
            //    enemy.goTo(player.transform.position, speed);
            //    enemy.turnTowardsDirection(player.transform.position);

            //    animator.SetBool("Moving", true);
            //}
            //else if (Vector2.Distance(player.transform.position, transform.position) < distance)
            //{
            //    enemy.resetAgent();
            //}

            Vector3 adjustedEnemyPosition = enemyBase.transform.position;
            adjustedEnemyPosition.x += 1.3f;

            if (directionReady == true &&
                Vector2.Distance(player.transform.position, enemyBase.transform.position) < runAwayDistance && 
                isAttacking == false)
            {
                directionReady = false;
                randomMovementTimer = RANDOM_MOVEMENT_COOLDOWN;

                if (player.transform.position.x > adjustedEnemyPosition.x)
                {
                    Vector3[] directionsLeft = { Vector3.left, Vector3.up + Vector3.left, Vector3.down + Vector3.left };
                    direction = directionsLeft[Random.Range(0, 3)];
                }
                else
                {
                    Vector3[] directionsRight = { Vector3.right, Vector3.up + Vector3.right, Vector3.down + Vector3.right };
                    direction = directionsRight[Random.Range(0, 3)];
                }
            }
            else if (Vector2.Distance(player.transform.position, adjustedEnemyPosition) > stopRunDistance)
            {
                direction = Vector3.zero;
            }

            if (isAttackReady == true && 
                isAttacking == false && 
                Vector2.Distance(player.transform.position, adjustedEnemyPosition) < attackDistance)
            {
                enemy.turnTowardsDirection(player.transform.position);
                animator.SetBool("Moving", false);
                direction = Vector3.zero;

                if (player.transform.position.x > adjustedEnemyPosition.x)
                {
                    animator.SetTrigger("AttackRight");
                }
                else
                {
                    animator.SetTrigger("AttackLeft");
                }

                isAttackReady = false;
                attackTimer = ATTACK_COOLDOWN;

                isAttacking = true;
                Invoke("Attack", 0.5f);
            }

            if (direction != Vector3.zero)
            {
                enemyBase.transform.Translate(direction * speed * Time.deltaTime);
            }
        }
    }

    void Attack()
    {
        if (enemy.getDead() == false)
        {
            lifetimeTimer = ATTACK_LIFETIME;
            attackHitBox.SetActive(true);
            audioSource.PlayOneShot(clips[0]);

            Invoke("startMovingOnDelay", 1.4f);
        }
    }

    void startMovingOnDelay()
    {
        animator.SetBool("Moving", true);
        isAttacking = false;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (enemy.getDead() == false && collision.gameObject.tag == "Player" && attackActive)
    //    {
    //        player.GetComponent<PlayerScript>().TakeDamage(transform.position, GameData.instance.skullDamage, GameData.instance.skullKnockback);

    //        Vector3 direction = (target - transform.position).normalized;
    //        rb.velocity = (-direction * ATTACK_SPEED * 0.2f);
    //        animator.SetBool("Moving", true);
    //    }
    //}
}
