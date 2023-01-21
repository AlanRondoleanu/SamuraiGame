using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public float duration = 1.0f;
    public AnimationCurve curve;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shake());
        }
    }

    public void ShakeCamera(float t_duration)
    {
        start = true;
        duration = t_duration;
    }

    IEnumerator Shake()
    {
        float timePast = 0.0f;

        while (timePast < duration)
        {
            timePast += Time.deltaTime;
            float strength = curve.Evaluate(timePast / duration);

            Vector3 cameraPos = player.transform.position;
            cameraPos.z = -10;
            transform.position = cameraPos + Random.insideUnitSphere * strength;

            yield return null;
        }

        Vector3 playerPos = player.transform.position;
        playerPos.z = -10;
        transform.position = playerPos;
    }
}
