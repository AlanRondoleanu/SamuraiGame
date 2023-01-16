using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 50;

    //Ranged Attack
    public GameObject enemyProjectile;

    private float rangedTimer;
    private const float MAX_RANGED_TIMER = 3.0f;

    private const float FLASH_DURATION = 0.1f;
    private float flashTimer;
    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;
    }

    void Update()
    {
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            enemyRenderer.material.color = Color.white;
            if (flashTimer <= 0)
            {
                enemyRenderer.material.color = originalColor;
            }
        }

        // Ranged Timer
        rangedTimer += Time.deltaTime;
        if (rangedTimer >= MAX_RANGED_TIMER)
        {
            RangedAttack();
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float t_damage)
    {
        flashTimer = FLASH_DURATION;
        health -= t_damage;

        Debug.Log(t_damage + " Damage");
    }

    void RangedAttack()
    {
        GameObject projectile = Instantiate(enemyProjectile);

        // Direction
        Vector2 projectileDirection = Vector3.left;

        projectile.GetComponent<ProjectileScript>().ReadyProjectile(transform.position, projectileDirection, 0);

        rangedTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            TakeDamage(15);
        }

        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerScript>().getDashing() == true)
        {
            TakeDamage(25);

            if (health <= 0)
            {
                collision.gameObject.GetComponent<PlayerScript>().resetDash();
            }
        }
    }
}
