using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float projectileSpeed = 20;

    private Camera mainCam;
    private Vector2 direction;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void ReadyProjectile(Vector2 t_position, Vector2 t_direction, float t_rotation)
    {
        transform.position = t_position;
        direction = t_direction;
        transform.rotation = Quaternion.Euler(0, 0, t_rotation);

        GetComponent<Rigidbody2D>().velocity = new Vector3(direction.x, direction.y).normalized * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerDeflect")
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCam.transform.position.z;
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);

            Vector3 direction = mouseWorldPos - transform.position;
            direction.z = 0;
            direction = direction.normalized * projectileSpeed * 2;

            // Rotation
            Vector3 rotation = transform.position - mouseWorldPos;
            float projectileRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, projectileRotation);
            GetComponent<Rigidbody2D>().velocity = direction;

            // Changes tag to player projectile tag
            gameObject.tag = "PlayerAttack";
        }
    }
}
