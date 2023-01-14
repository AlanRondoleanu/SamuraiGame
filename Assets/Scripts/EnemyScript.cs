using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
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
    }

    public void TakeDamage()
    {
        flashTimer = FLASH_DURATION;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            TakeDamage();
        }

        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerScript>().getDashing() == true)
        {
            TakeDamage();
        }
    }
}
