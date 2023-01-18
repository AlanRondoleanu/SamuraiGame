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

    private Camera mainCam;
    private Animator animator;
    private AttackMode attackMode;
    private const float MELEE_DISTANCE = 1;
    private const float DEFLECT_DISTANCE = 0.5f;

    // Melee
    public GameObject playerMeleeHitBox;

    private float meleeAttackTimer;
    private const float ATTACK_LIFETIME = 0.2f;
    private bool isAttacking = false;

    // Deflect
    public GameObject deflectHitBox;

    private float deflectTimer;
    private const float DEFLECT_LIFETIME = 0.2f;
    private float deflectCooldownTimer;
    private const float DEFLECT_COOLDOWN = 1f;
    private bool isDeflectReady = true;
    private bool isDeflectUp = false;

    // Ranged 
    public GameObject playerProjectile;

    private const float MAX_PROJECTILE_SPEED = 25;

    // Dash Attack
    private float dashDistance = 1f;
    private float dashSpeed = 40f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private const float DASH_DURATION = 0.2f;
    private float dashTimer;
    private float dashCooldownTimer;
    private const float DASH_COOLDOWN = 2f;
    private bool isDashReady = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMeleeHitBox.transform.position = transform.position + transform.right * MELEE_DISTANCE;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
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

        // Rotate Melee Direction
        if (playerMeleeHitBox.active == false)
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
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dashDirection * dashDistance, dashSpeed * Time.deltaTime);

            // Dash timer
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                animator.SetBool("Dashing", false);
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
        if (meleeAttackTimer <= 0.0f)
        {
            playerMeleeHitBox.gameObject.SetActive(true);
            meleeAttackTimer = ATTACK_LIFETIME;
            isAttacking = true;

            animator.SetTrigger("Attack");
            meleeEffect.SetTrigger("MeleeEffect");
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
        }
    }

    public void RangedAttack()
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

    public void deflectSuccess()
    {
        animator.SetTrigger("Deflect");
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
}

