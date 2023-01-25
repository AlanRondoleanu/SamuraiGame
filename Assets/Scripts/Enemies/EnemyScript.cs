using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public float health = 50;
    public GameObject dashCutEffect;
    public GameObject dashKillEffect;
    public GameObject healthBar;
    public AudioClip[] clips;
    public GameObject blood;

    private bool dead = false;
    private float MAX_HEALTH;
    private Animator animator;
    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCam;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private bool navigating = false;

    // Invulnerability
    private bool immune = false;
    private float immuneLifetime;
    private const float IMMUNE_LIFETIME = 0.1f;

    // Misc
    private bool checkCollider = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

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
            checkCollider = true;
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            agent.ResetPath();

            GameManager.instance.ReduceEnemies();
        }

        immuneLifetime -= Time.deltaTime;
        if (immuneLifetime <= 0.0f)
        {
            immune = false;
        }

        // Turn off collider on death when the body is stationary
        if (checkCollider == true && rb.velocity == Vector2.zero)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            checkCollider = false;
        }
    }

    public void TakeDamage(float t_damage)
    {
        if (dead == false && immune == false)
        {
            turnTowardsDirection(player.transform.position);

            health -= t_damage;
            immune = true;
            immuneLifetime = IMMUNE_LIFETIME;

            // Health Bar
            float percent = (health / MAX_HEALTH) * 1;
            percent -= 1;
            healthBar.GetComponent<EnemyHealthBar>().setMask(percent, false);

            // Animation
            animator.SetTrigger("Hit");

            // Blood Animation
            GameObject bloodObj = Instantiate(blood);
            bloodObj.transform.position = transform.position;
            int randomBlood = Random.Range(1, 9);
            bloodObj.GetComponent<Animator>().SetInteger("Blood", randomBlood);

            // Audio
            audioSource.PlayOneShot(clips[0]);
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

    public void goTo(Vector2 t_target, float t_speed)
    {
        if (dead == false)
        {
            agent.speed = t_speed;
            agent.SetDestination(t_target);
            navigating = true;
        }
    }

    public void resetAgent()
    {
        if (dead == false)
        {
            agent.ResetPath();
            navigating = false;
        }
    }

    public bool getNavigating()
    {
        return navigating;
    }

    public float getHealth()
    {
        return health;
    }    
}
