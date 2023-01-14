using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{ 
    public enum AttackMode
    {
        Rock, Flow
    }
    public GameObject playerMeleeHitBox;

    private Camera mainCam;
    private Animator animator;

    // Melee
    private AttackMode attackMode;
    private float meleeAttackTimer;
    private const float MAX_ATTACK_COOLDOWN = 0.5f;
    private const float MELEE_DISTANCE = 1;

    // Dash Attack
    private float dashDistance = 1f;
    private float dashSpeed = 40f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private const float DASH_DURATION = 0.2f;
    private float dashTimer;

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
        // Cooldown Timers
        meleeAttackTimer -= Time.deltaTime;
        if (meleeAttackTimer <= 0.0f)
        {
            playerMeleeHitBox.gameObject.SetActive(false);
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
            playerMeleeHitBox.transform.up = direction;

            Vector3 desiredPos = transform.position + direction * MELEE_DISTANCE;
            playerMeleeHitBox.transform.position = Vector3.MoveTowards(playerMeleeHitBox.transform.position, desiredPos, 100 * Time.deltaTime);
        }

        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dashDirection * dashDistance, dashSpeed * Time.deltaTime);

            // Dash timer
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
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
            meleeAttackTimer = MAX_ATTACK_COOLDOWN;
        }
    }

    public void DashAttack()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            dashDirection.z = 0;
            dashTimer = DASH_DURATION;
        }

    }

    public bool getDashing()
    {
        return isDashing;
    }

}
