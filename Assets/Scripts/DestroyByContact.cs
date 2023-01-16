using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
		}
		else
		{
			if (collision.gameObject.tag == "Player")
			{
				return;
			}

			if (collision.gameObject.tag == "Enemy")
			{
				Destroy(gameObject);
			}
		}
	}
}