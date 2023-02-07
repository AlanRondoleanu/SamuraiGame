using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSmoke : MonoBehaviour
{
    private SpriteRenderer renderer;
    private Vector3 direction;
    private float elapsedTime = 0;
    private float speed = 0.5f;
    private Color color;

    public float duration = 5f;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        color = renderer.color;

        direction = Random.insideUnitCircle;
        speed = Random.Range(0.2f, 0.4f);

        float scale = Random.Range(0.05f, 0.5f);
        transform.localScale = new Vector2(scale, scale * 0.5f);
    }

    
    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;

        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            color.a = alpha;
            renderer.color = color; 
        }
    }
}
