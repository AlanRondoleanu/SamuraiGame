using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinScript : MonoBehaviour
{
    public GameObject enemyProjectile;
    public float speed;

    private GameObject player;
    private Animator animator;
    private EnemyScript enemy;
    private float rangedTimer;
    private const float MAX_RANGED_TIMER = 3.0f;
    private const float MAX_PROJECTILE_SPEED = 10;
    private Vector2 currentDirection = Vector2.left;
    private bool moving = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponent<EnemyScript>();
        animator = GetComponent<Animator>();

        InvokeRepeating("changeDirection", 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.getDead() == false)
        {
            // Ranged Timer
            rangedTimer += Time.deltaTime;
            if (rangedTimer >= MAX_RANGED_TIMER)
            {
                enemy.turnTowardsDirection(player.transform.position);
                animator.SetTrigger("Attack");
                moving = false;

                Invoke("RangedAttack", 0.1f);
                Invoke("RangedAttack", 0.4f);
                Invoke("startMoving", 0.8f);

                rangedTimer = 0;
            }

            if (moving)
            {
                // Movement
                if (currentDirection.x != 0 && currentDirection.y != 0)
                {
                    transform.Translate(currentDirection * (speed * 0.5f) * Time.deltaTime);
                }
                else
                {
                    transform.Translate(currentDirection * speed * Time.deltaTime);
                }
            }
        }
    }

    void RangedAttack()
    {
        GameObject projectile = Instantiate(enemyProjectile);

        // Rotation Logic for the projectile
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
        Vector3 rotation = transform.position - playerPosition;
        // Rotation
        float projectileRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        // Direction
        Vector2 projectileDirection = new Vector3(direction.x, direction.y).normalized * MAX_PROJECTILE_SPEED;

        // Weapon Position
        Vector2 weaponPos = new Vector2(transform.position.x, transform.position.y - 0.2f);

        projectile.GetComponent<ProjectileScript>().ReadyProjectile(weaponPos, projectileDirection, projectileRotation, GameData.instance.assassinDamage);
    }

    void changeDirection()
    {
        Vector3[] directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.left, Vector3.up + Vector3.right, Vector3.up + Vector3.left, Vector3.down + Vector3.right, Vector3.down + Vector3.left };
        currentDirection = directions[Random.Range(0, 8)];
    }

    void startMoving()
    {
        moving = true;
    }
}
