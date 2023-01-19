using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float projectileSpeed = 20;
    public GameObject impact; 

    private GameObject player;
    private Camera mainCam;
    private Vector2 direction;
    private float damage;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ReadyProjectile(Vector2 t_position, Vector2 t_direction, float t_rotation, float t_damage)
    {
        transform.position = t_position;
        direction = t_direction;
        transform.rotation = Quaternion.Euler(0, 0, t_rotation);
        damage = t_damage;

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
            direction = direction.normalized * projectileSpeed * GameData.instance.reflectSpeedMultipler;

            // Rotation
            Vector3 rotation = transform.position - mouseWorldPos;
            float projectileRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, projectileRotation);
            GetComponent<Rigidbody2D>().velocity = direction;

            // Changes tag to player projectile tag
            gameObject.tag = "PlayerRangedAttack";

            // Animates the player to show successful deflect
            player.GetComponent<PlayerScript>().deflectSuccess();

            // Gives extra damage to projectile
            damage *= GameData.instance.reflectDamageMultipler;
        }

        if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameObject impactTemp = Instantiate(impact);

        float rotation = transform.eulerAngles.z + 180;
        impactTemp.transform.rotation = Quaternion.Euler(0, 0, rotation);
        impactTemp.transform.position = transform.position;
    }

    public float getDamage()
    {
        return damage;
    }
}
