using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public float duration = 1.0f;
    public AnimationCurve curve;

    // Update is called once per frame
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
        Vector3 startPos = transform.position;
        float timePast = 0.0f;

        while (timePast < duration)
        {
            timePast += Time.deltaTime;
            float strength = curve.Evaluate(timePast / duration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPos;
    }
}
