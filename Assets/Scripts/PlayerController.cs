using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public GameObject playerProjectile;
    public float projectileSpeed = 0;
    public GameObject playerAttackPoint;
    public GameObject playerMeleeHitBox;

    private const float MAX_FLOW_SPEED = 4;
    private const float MAX_ROCK_SPEED = 2;
    private float speed = MAX_ROCK_SPEED;
    private Rigidbody2D rb;
    private PlayerScript player;
    private Animator animator;
    private Camera mainCam;
    private float meleeAttackTimer;
    private const float MAX_ATTACK_COOLDOWN = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        // Key Events
        if (Input.GetKeyDown("q"))
        {
            if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
            {
                player.ChangeAttackMode(PlayerScript.AttackMode.Rock);
                speed = MAX_ROCK_SPEED;
            }
            else
            {
                player.ChangeAttackMode(PlayerScript.AttackMode.Flow);
                speed = MAX_FLOW_SPEED;
            }

            Debug.Log("Mode Change");
        }
        if (Input.GetKeyDown("space"))
        {
            if (player.getAttackMode() == PlayerScript.AttackMode.Flow)
            {
                GameObject projectile = Instantiate(playerProjectile, transform);

                // Rotation Logic for the projectile
                Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = mousePosition - player.transform.position;
                Vector3 rotation = player.transform.position - mousePosition;
                // Rotation
                float projectileRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                // Direction
                Vector2 projectileDirection = new Vector3(direction.x, direction.y).normalized * speed;

                projectile.GetComponent<ProjectileScript>().ReadyProjectile(projectileSpeed, projectileDirection, projectileRotation);
            }
            else if (player.getAttackMode() == PlayerScript.AttackMode.Rock)
            {
                playerMeleeHitBox.gameObject.SetActive(true);
                meleeAttackTimer = MAX_ATTACK_COOLDOWN;
            }

            Debug.Log("Attack");
        }


        // Movement
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(xMove, zMove) * speed;

        // State Changing for movement 
        if (xMove != 0 || zMove != 0)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        // Changes attakc point depending on direction
        float range = 0.5f;
        Vector3 attackDirection;

        if (playerMeleeHitBox.gameObject.active == false)
        {
            //Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            //attackDirection = mousePosition - player.transform.position;
            //playerMeleeHitBox.transform.position = playerAttackPoint.transform.position + (attackDirection.normalized * range);
        }

        // Cooldown Timers
        meleeAttackTimer -= Time.deltaTime;
        if (meleeAttackTimer <= 0.0f)
        {
            playerMeleeHitBox.gameObject.SetActive(false);
        }
    }
}
