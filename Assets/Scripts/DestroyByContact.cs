using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.tag == "Player")
		{
			return;
		}

		// Check for pickup and player collision, else it returns
		if (collision.gameObject.tag == "Enemy")
		{
			Destroy(gameObject);
		}
	}
}