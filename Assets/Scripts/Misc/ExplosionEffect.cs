using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public GameObject circlePrefab;
    public GameObject smokePrefab;

    public float spawnInterval = 2f;
    public float stopAfter = 5;

    private Camera mainCam;
    private float timePassed = 0f;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        StartCoroutine("CreateCircle");
    }

    void Update()
    {
        timePassed += Time.deltaTime;
    }
 
    private IEnumerator CreateCircle()
    {
        while (timePassed < stopAfter)
        {
            // Choose a random angle between 0 and 360 degrees
            float angle = Random.Range(0f, 360f);

            // Convert the angle to radians
            float radians = angle * Mathf.Deg2Rad;

            // Calculate the x and y offset based on the angle
            float x = Mathf.Cos(radians);
            float y = Mathf.Sin(radians);

            Vector3 offset = new Vector3(x, y, 0f);
            GameObject newCircle = Instantiate(circlePrefab, transform.position + offset, Quaternion.identity);
            GameObject newSmoke = Instantiate(smokePrefab);
            newSmoke.transform.position = newCircle.transform.position;
            Destroy(newCircle, 4);

            float scale = Random.Range(1f, 3f);
            newCircle.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer renderer = newCircle.GetComponent<SpriteRenderer>();
            renderer.color = Color.black;

            mainCam.GetComponent<ScreenShake>().ShakeCamera(0.3f);

            // Fade the new circle sprite away and destroy it after 1 second
            StartCoroutine(FadeAndDestroy(renderer, 4f));

            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    IEnumerator FadeAndDestroy(SpriteRenderer renderer, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        renderer.color = Color.white;
        float scale = renderer.gameObject.transform.localScale.x;

        // Fades away the alpha color
        float elapsedTime2 = 0f;
        while (elapsedTime2 < duration)
        {
            elapsedTime2 += Time.deltaTime;

            scale = Mathf.Lerp(scale, 0, elapsedTime2 / duration);
            renderer.gameObject.transform.localScale = new Vector2(scale, scale);

            yield return null;
        }
    }
}