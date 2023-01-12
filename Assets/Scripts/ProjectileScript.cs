using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private float speed;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadyProjectile(float t_speed, Vector2 t_direction, float t_rotation)
    {
        speed = t_speed;
        direction = t_direction;
        transform.rotation = Quaternion.Euler(0, 0, t_rotation);

        GetComponent<Rigidbody2D>().velocity = new Vector3(direction.x, direction.y).normalized * speed;
    }
}
