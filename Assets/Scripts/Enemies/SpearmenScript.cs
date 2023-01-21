using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmenScript : MonoBehaviour
{
    public float axisThreshold = 0.1f;
    public GameObject spear;
    public GameObject dashSmokeEffect;
    public AudioClip[] clips;

    private GameObject player;
    private EnemyScript enemy;
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerScript playerScript;
    private AudioSource audioSource;

    // Charge
    private float chargeLifetime;
    private const float CHARGE_LIFETIME = 0.5f;
    private float chargeCooldownTimer;
    private const float CHARGE_COOLDOWN = 3f;
    private const float CHARGE_SPEED = 45;
    private bool isChargeReady = true;

    // Movement
    private bool moving;
    public float speed;
    private float distance = 7;
    private Vector2 currentDirection;

    // Stunned
    private float stunTimer;
    private const float STUN_DURATION = 2f;
    private bool isStunned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        enemy = GetComponent<EnemyScript>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        InvokeRepeating("goTowardsPlayer", 1, 2);
    }

    void Update()
    {
        if (enemy.getDead() == false)
        {
            // Lifetimer Timers
            chargeLifetime -= Time.deltaTime;
            if (chargeLifetime <= 0.0f)
            {
                animator.SetBool("Attacking", false);
            }
            // Cooldown Timers
            chargeCooldownTimer -= Time.deltaTime;
            if (chargeCooldownTimer <= 0.0f)
            {
                isChargeReady = true;
            }
            // Stun Duration timer
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0.0f)
            {
                animator.SetBool("Stunned", false);
                isStunned = false;
                spear.gameObject.SetActive(true);
            }

            // Charge Ability
            if (isChargeReady == true &&
                isStunned == false &&
                playerScript.getDashing() == false && 
                Mathf.Abs(player.transform.position.y - transform.position.y) <= axisThreshold &&
                Vector2.Distance(player.transform.position, transform.position) < distance)
            {
                isChargeReady = false;
                chargeCooldownTimer = CHARGE_COOLDOWN;
                enemy.turnTowardsDirection(player.transform.position);
                enemy.resetAgent();

                Invoke("setChargeDelay", 1);

                // Animation for charge
                Invoke("setChargeAnimationDelay", 0.4f);

                moving = false;
                animator.SetBool("Moving", false);
                currentDirection = Vector2.zero;
            }

            // Movement
            if (moving && enemy.getNavigating() == false)
            {
                transform.Translate(currentDirection * speed * Time.deltaTime);
            }
        }
        // Handle spear tip damaging player
        if (enemy.getDead() == true)
        {
            spear.gameObject.SetActive(false);
        }
    }

    void setChargeDelay()
    {
        if (enemy.getDead() == false)
        {
            chargeLifetime = CHARGE_LIFETIME;

            Vector2 chargeDirection;
            if (player.transform.position.x > transform.position.x)
            {
                chargeDirection = Vector2.right;
                spear.transform.localPosition = new Vector2(Mathf.Abs(spear.transform.localPosition.x), spear.transform.localPosition.y);
            }
            else
            {
                chargeDirection = Vector2.left;
                spear.transform.localPosition = new Vector2(Mathf.Abs(spear.transform.localPosition.x) * -1, spear.transform.localPosition.y);
            }

            rb.velocity = (chargeDirection * CHARGE_SPEED);

            // Smoke effect
            GameObject smoke = Instantiate(dashSmokeEffect);
            smoke.transform.position = new Vector2(transform.position.x, transform.position.y - 0.2f);
            if (chargeDirection == Vector2.right)
            {
                smoke.transform.rotation = Quaternion.Euler(180, -90, 90);
            }
            else
            {
                smoke.transform.rotation = Quaternion.Euler(0, -90, 90);
            }

            // Audio
            audioSource.PlayOneShot(clips[0]);
        }
    }

    void setChargeAnimationDelay()
    {
        if (enemy.getDead() == false)
        {
            animator.SetBool("Attacking", true);
        }
    }

    void goTowardsPlayer()
    {
        if (isChargeReady == true && enemy.getDead() == false)
        {
            moving = true;
            enemy.turnTowardsDirection(player.transform.position);
            animator.SetBool("Moving", true);

            // Get random direction towards player

            RaycastHit2D hit = Physics2D.Raycast(transform.position,(player.transform.position - transform.position).normalized, distance);
            if (hit.collider != null &&
                hit.collider.tag != "Player")
            {
                enemy.goTo(player.transform.position, speed);
            }
            else if (player.transform.position.y > transform.position.y &&
                     Vector2.Distance(player.transform.position, transform.position) < distance)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    currentDirection = Vector2.up + Vector2.right;
                }
                else
                {
                    currentDirection = Vector2.up + Vector2.left;
                }
            }
            else
            {
                if (player.transform.position.x > transform.position.x)
                {
                    currentDirection = Vector2.down + Vector2.right;
                }
                else
                {
                    currentDirection = Vector2.down + Vector2.left;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerDeflect" && chargeLifetime > 0)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce((transform.position - collision.transform.position).normalized * 800);

            // Animates the player to show successful deflect
            player.GetComponent<PlayerScript>().deflectSuccess();

            // Stun Spearmen
            isStunned = true;
            stunTimer = STUN_DURATION;
            animator.SetBool("Stunned", true);
            spear.gameObject.SetActive(false);
            enemy.resetAgent();
        }
    }
}
