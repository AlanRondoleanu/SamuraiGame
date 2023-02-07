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
    public Transform[] teleportSpots;
    public GameObject deathExplosion;
    
    public float speed = 3;

    private float stopRunDistance = 7;
    private float runAwayDistance = 4;
    private float attackDistance = 2.5f;
    private int teleports = 2;
    private float maxHealth;
    private int attackCycle = 1;
    private bool explosionUsed = false;

    // Movement
    private Vector3 direction;
    private bool directionReady = true;
    private const float RANDOM_MOVEMENT_COOLDOWN = 2.0f;
    private float randomMovementTimer = RANDOM_MOVEMENT_COOLDOWN;

    // Fire Attack
    public GameObject attackHitBox;
    private const float ATTACK_COOLDOWN = 5.0f;
    private float attackTimer = ATTACK_COOLDOWN;
    private const float ATTACK_LIFETIME = 0.7f;
    private bool isAttackReady = false;
    private bool isAttacking = false;
    private float lifetimeTimer;

    // Cone Attack
    public GameObject enemyProjectile;
    private const float CONE_COOLDOWN = 5.5f;
    private float coneTimer = CONE_COOLDOWN;
    private bool coneAttackReady = false;

    // Skeleton Spawn
    public GameObject skull;
    private const float SPAWN_COOLDOWN = 4.0f;
    private float spawnTimer = SPAWN_COOLDOWN;
    private bool spawnReady = false;

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

        maxHealth = enemy.getHealth();
    }

    // Update is called once per frame
    void Update()
    {
        //========================================== - TIMERS - ====================================================|

        // Life Timer for fire attack hitbox
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0 && attackHitBox.activeInHierarchy == true)
            attackHitBox.gameObject.SetActive(false);

        // Cooldown timer for fire attack
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            isAttackReady = true;

        // Cooldown timer for random movement
        randomMovementTimer -= Time.deltaTime;
        if (randomMovementTimer <= 0)
            directionReady = true;

        // Cooldown Timer for the skull spawning
        if (attackCycle == 1)
            spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
            spawnReady = true;

        // Cooldown Timer for the cone attack
        if (attackCycle == 2)
            coneTimer -= Time.deltaTime;

        if (coneTimer <= 0)
            coneAttackReady = true;

        //===========================================================================================================|

        if (enemy.getDead() == false)
        {
            // Teleport
            if (isAttacking == false)
            {
                if (enemy.getHealth() < (maxHealth * 0.7) && teleports == 2)
                {
                    teleports--;
                    animator.SetTrigger("Spawn");

                    Invoke("teleport", 0.7f);
                }
                else if (enemy.getHealth() < (maxHealth * 0.4) && teleports == 1)
                {
                    teleports--;
                    animator.SetTrigger("Spawn");

                    Invoke("teleport", 0.7f);
                }
            }

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

            // Spawn Skelly
            if (spawnReady == true)
            {
                enemy.turnTowardsDirection(player.transform.position);
                Invoke("SpawnSkull", 0.7f);

                spawnReady = false;
                spawnTimer = SPAWN_COOLDOWN;

                animator.SetTrigger("Spawn");
                audioSource.PlayOneShot(clips[2]);

                attackCycle = 2;

                // Delay fire attack
                isAttackReady = false;
                attackTimer = 2;
            }

            // Cone Attack
            if (coneAttackReady == true)
            {
                enemy.turnTowardsDirection(player.transform.position);
                Invoke("ConeAttack", 0.7f);

                coneTimer = CONE_COOLDOWN;
                coneAttackReady = false;

                animator.SetTrigger("RangedAttack");

                attackCycle = 1;

                // Delay fire attack
                isAttackReady = false;
                attackTimer = 2;
            }

            // Fire Attack
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

            // Movement
            if (direction != Vector3.zero)
            {
                enemyBase.transform.Translate(direction * speed * Time.deltaTime);
            }
        }

        // Explosion Effects on death
        if (explosionUsed == false && enemy.getDead() == true)
        {
            GameObject explosions = Instantiate(deathExplosion);
            explosions.transform.position = transform.position;

            explosionUsed = true;

            GameManager.instance.StopTimer();

            audioSource.PlayOneShot(clips[1]);
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

    void SpawnSkull()
    {
        GameObject skullObj = Instantiate(skull);
        Vector2 skullSpawn = transform.position;
        skullSpawn.y += 1;
        skullObj.transform.position = skullSpawn;
    }

    public void ConeAttack()
    {
        Vector3 initalDirection = (player.transform.position - transform.position).normalized;
        float initalAngle = Mathf.Atan2(initalDirection.y, initalDirection.x) * Mathf.Rad2Deg;
        initalAngle += 20;
        Vector2 direction = DegreeToVector2(initalAngle); 

        for (int i = 0; i < 5; i++)
        {
            GameObject projectile = Instantiate(enemyProjectile);

            // Rotation Logic for the projectile
            float projectileRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            projectile.GetComponent<ProjectileScript>().ReadyProjectile(transform.position, direction, projectileRotation, GameData.instance.demonRangedDamage);

            // Updates next direction
            initalAngle -= 10;
            direction = DegreeToVector2(initalAngle);
        }

        audioSource.PlayOneShot(clips[3]);
    }

    public void teleport()
    {
        if (Vector2.Distance(enemyBase.transform.position, teleportSpots[0].position) > Vector2.Distance(enemyBase.transform.position, teleportSpots[1].position))
        {
            enemyBase.transform.position = teleportSpots[0].position;
        }
        else
        {
            enemyBase.transform.position = teleportSpots[1].position;
        }

        audioSource.PlayOneShot(clips[4]);
    }

    private Vector2 DegreeToVector2(float t_degree)
    {
        return RadianToVector2(t_degree * Mathf.Deg2Rad);
    }

    public Vector2 RadianToVector2(float t_radian)
    {
        return new Vector2(Mathf.Cos(t_radian), Mathf.Sin(t_radian));
    }

    public float Angle(Vector2 vector2)
    {
        return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
    }
}
