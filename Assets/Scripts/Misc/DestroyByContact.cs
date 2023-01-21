using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
		// Enemy Attacks
		if (tag == "EnemyAttack")
		{
			if (collision.gameObject.tag == "Enemy")
			{
				return;
			}

			if (collision.gameObject.tag == "Player")
			{
				Destroy(gameObject);
			}

			if (collision.tag == "Wall")
			{
				Destroy(gameObject);
			}
		}
		else // Player Attacks
		{
			if (collision.gameObject.tag == "Player")
			{
				return;
			}

			if (collision.gameObject.tag == "Enemy" && collision.GetComponent<EnemyScript>().getDead() == false)
			{
				Destroy(gameObject);
			}


			if (collision.tag == "Wall")
			{
				Destroy(gameObject);
			}
		}
	}
}