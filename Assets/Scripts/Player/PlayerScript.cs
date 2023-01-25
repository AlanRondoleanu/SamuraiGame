using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{ 
    public enum AttackMode
    {
        Rock, Flow
    }
    public Animator meleeEffect;
    public GameObject meleeEffectObject;
    public Animator stanceEffect;
    public GameObject dashSmokeEffect;
    public GameObject healthBar;
    public CanvasRenderer bloodyScreen;

    private Camera mainCam;
    private Animator animator;
    private Rigidbody2D rb;

    // Player Vars
    private AttackMode attackMode;
    private float health;
    private const float MAX_HEALTH = 100;
    private bool alive = true;

    // Melee
    public GameObject playerMeleeHitBox;

    private float meleeAttackTimer;
    private const float MELEE_LIFETIME = 0.2f;
    private float meleeCooldownTimer;
    private const float MELEE_COOLDOWN = 0.5f;
    private const float MELEE_DISTANCE = 1f;
    private bool isAttacking = false;
    private bool isMeleeReady = true;

    // Deflect
    public GameObject deflectHitBox;

    private float deflectTimer;
    private const float DEFLECT_LIFETIME = 0.2f;
    private float deflectCooldownTimer;
    private const float DEFLECT_COOLDOWN = 1f;
    private const float DEFLECT_DISTANCE = 0.5f;
    private bool isDeflectReady = true;
    private bool isDeflectUp = false;

    // Ranged 
    public GameObject playerProjectile;

    private const float MAX_PROJECTILE_SPEED = 25;
    private float rangedCooldownTimer;
    private const float RANGED_COOLDOWN = 0.8f;
    private bool isRangedReady = true;

    // Dash Attack
    private float dashSpeed = 40f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private const float DASH_DURATION = 0.2f;
    private float dashTimer;
    private float dashCooldownTimer;
    private const float DASH_COOLDOWN = 4f;
    private bool isDashReady = true;

    // Invulnerability
    private bool immune = false; 
    private float immuneLifetime;
    private const float IMMUNE_LIFETIME = 0.2f;

    // Audio
    public AudioClip[] clips;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMeleeHitBox.transform.position = transform.position + transform.right * MELEE_DISTANCE;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        bloodyScreen.SetColor(new Color(1, 1, 1, 0));

        GameManager.instance.setScene();
        setHealth(GameManager.instance.getHealth());
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if (health <= 0)
            {
                alive = false;
                animator.SetBool("Dead", true);
                GameManager.instance.displayRestartText();
            }

            // Lifetimer Timers
            meleeAttackTimer -= Time.deltaTime;
            if (meleeAttackTimer <= 0.0f)
            {
                playerMeleeHitBox.gameObject.SetActive(false);
                isAttacking = false;
            }
            deflectTimer -= Time.deltaTime;
            if (deflectTimer <= 0.0f)
            {
                deflectHitBox.gameObject.SetActive(false);
                isDeflectUp = false;
                animator.SetBool("DeflectUp", false);
            }
            immuneLifetime -= Time.deltaTime;
            if (immuneLifetime <= 0.0f)
            {
                immune = false;
            }

            // Cooldown Timers
            deflectCooldownTimer -= Time.deltaTime;
            if (deflectCooldownTimer <= 0.0f)
            {
                isDeflectReady = true;
            }
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0.0f)
            {
                isDashReady = true;
            }
            meleeCooldownTimer -= Time.deltaTime;
            if (meleeCooldownTimer <= 0.0f)
            {
                isMeleeReady = true;
            }
            rangedCooldownTimer -= Time.deltaTime;
            if (rangedCooldownTimer <= 0.0f)
            {
                isRangedReady = true;
            }

            // Bloody Screen
            Color tempColor = bloodyScreen.GetColor();
            if (tempColor != new Color(1,1,1,0))
            {
                tempColor.a -= 0.01f;
                bloodyScreen.SetColor(tempColor);
            }

            // Rotate Melee Direction
            if (playerMeleeHitBox.activeInHierarchy == false)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = mainCam.transform.position.z;
                Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);

                Vector3 direction = mouseWorldPos - transform.position;
                direction.z = 0;
                direction = direction.normalized;

                // Melee
                Vector3 desiredPos = transform.position + direction * MELEE_DISTANCE;
                playerMeleeHitBox.transform.position = Vector3.MoveTowards(playerMeleeHitBox.transform.position, desiredPos, 1000 * Time.deltaTime);
                // Deflect
                Vector3 desiredPos2 = transform.position + direction * DEFLECT_DISTANCE;
                deflectHitBox.transform.position = Vector3.MoveTowards(playerMeleeHitBox.transform.position, desiredPos2, 1000 * Time.deltaTime);

                meleeEffectObject.transform.position = playerMeleeHitBox.transform.position;
            }

            // Dash movement when dash is activated
            if (isDashing)
            {
                // Raycasting wall detection
                detectWalls(dashDirection);

                if (isDashing)
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + dashDirection, dashSpeed * Time.deltaTime);

                // Dash timer
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                    animator.SetBool("Dashing", false);
                }
            }
        }
    }

    public void ChangeAttackMode(AttackMode t_mode)
    {
        attackMode = t_mode;
    }

    public AttackMode getAttackMode()
    {
        return attackMode;
    }

    public void MeleeAttack()
    {
        if (isMeleeReady)
        {
            playerMeleeHitBox.gameObject.SetActive(true);

            meleeAttackTimer = MELEE_LIFETIME;
            meleeCooldownTimer = MELEE_COOLDOWN;
            isAttacking = true;
            isMeleeReady = false;

            animator.SetTrigger("Attack");
            meleeEffect.SetTrigger("MeleeEffect");

            // Audio
            audioSource.PlayOneShot(clips[0]);
        }
    }

    public void Deflect()
    {
        if (isDeflectReady)
        {
            deflectHitBox.gameObject.SetActive(true);
            deflectTimer = DEFLECT_LIFETIME;
            deflectCooldownTimer = DEFLECT_COOLDOWN;

            isDeflectReady = false;
            isDeflectUp = true;

            animator.SetBool("DeflectUp", true);

            // Audio
            audioSource.PlayOneShot(clips[3]);
        }
    }

    public void RangedAttack()
    {
        if (isRangedReady)
        {
            GameObject projectile = Instantiate(playerProjectile);

            // Rotation Logic for the projectile
            Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            Vector3 rotation = transform.position - mousePosition;
            // Rotation
            float projectileRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            // Direction
            Vector2 projectileDirection = new Vector3(direction.x, direction.y).normalized * MAX_PROJECTILE_SPEED;

            projectile.GetComponent<ProjectileScript>().ReadyProjectile(transform.position, projectileDirection, projectileRotation, GameData.instance.rangedDamage);

            animator.SetTrigger("Attack");

            isRangedReady = false;
            rangedCooldownTimer = RANGED_COOLDOWN;

            // Audio
            audioSource.PlayOneShot(clips[1]);
        }
    }

    public void DashAttack()
    {
        if (!isDashing && isDashReady)
        {
            isDashing = true;
            dashDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            dashDirection.z = 0;
            dashTimer = DASH_DURATION;
            dashCooldownTimer = DASH_COOLDOWN;
            animator.SetBool("Dashing", true);

            isDashReady = false;

            // Smoke effect
            GameObject smoke = Instantiate(dashSmokeEffect);
            smoke.transform.position = new Vector2(transform.position.x, transform.position.y - 0.3f);

            // Rotation Logic for the smoke
            Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = transform.position - mousePosition;
            float smokeRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            smoke.transform.rotation = Quaternion.Euler(smokeRotation, -90, 90);

            // Immunity while dashing
            immuneLifetime = IMMUNE_LIFETIME;
            immune = true;

            // Audio
            audioSource.PlayOneShot(clips[2]);
        }

    }

    public bool getDashing()
    {
        return isDashing;
    }

    public void resetDash()
    {
        isDashReady = true;
    }

    public bool getAttacking()
    {
        return isAttacking;
    }

    public bool getDeflecting()
    {
        return isDeflectUp;
    }

    public bool getAlive()
    {
        return alive;
    }

    public void deflectSuccess()
    {
        animator.SetTrigger("Deflect");
        isDeflectReady = true;

        // Audio
        audioSource.PlayOneShot(clips[4]);
    }

    public void changeStance()
    {
        if (attackMode == AttackMode.Flow)
        {
            stanceEffect.SetTrigger("Flow");
        }
        else
        {
            stanceEffect.SetTrigger("Rock");
        }
    }

    void detectWalls(Vector2 t_direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + t_direction, t_direction, 0.4f);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Wall")
            {
                isDashing = false;
                animator.SetBool("Dashing", false);
            }
        }
    }

    public void TakeDamage( Vector3 t_enemyPos, float t_damage, float t_knockback)
    {
        if (immune == false)
        {
            health -= t_damage;
            rb.AddForce((transform.position - t_enemyPos).normalized * t_knockback);

            // Health Bar
            float percent = (health / MAX_HEALTH) * 1;
            percent -= 1;
            healthBar.GetComponent<EnemyHealthBar>().setMask(percent, true);

            // Bloody screen
            bloodyScreen.SetColor(new Color(1,1,1,1));

            // Camera shake
            mainCam.GetComponent<ScreenShake>().ShakeCamera(0.8f);

            // Immunity frames
            immuneLifetime = IMMUNE_LIFETIME;
            immune = true;

            // Audio
            audioSource.PlayOneShot(clips[5]);
        }
    }

    public bool getImmune()
    {
        return immune;
    }

    public float getHealth()
    {
        return health;
    }

    public void setHealth(float t_health)
    {
        health = t_health;

        // Health Bar
        float percent = (health / MAX_HEALTH) * 1;
        percent -= 1;
        healthBar.GetComponent<EnemyHealthBar>().setMask(percent, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyAttack")
        {
            TakeDamage(collision.transform.position, collision.gameObject.GetComponent<ProjectileScript>().getDamage(), GameData.instance.assassinKnockback);
        }
        else if (collision.gameObject.tag == "EnemySpear" && isDeflectUp == false)
        {
            TakeDamage(collision.transform.position, GameData.instance.spearmenDamage, GameData.instance.spearmenKnockback);
        }
        else if (collision.gameObject.tag == "FireBreath")
        {
            TakeDamage(collision.transform.position, GameData.instance.demonDamage, GameData.instance.demonKnockback);
        }
    }
}

