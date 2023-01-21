using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public GameObject mask;

    public void setMask(float t_offset, bool t_player)
    {
        if (t_player)
        {
            Vector2 maskLocation = new Vector2(mask.transform.localPosition.x + t_offset, mask.transform.localPosition.y);
            maskLocation.x = t_offset;
            mask.transform.localPosition = maskLocation;
        }
        else
        {
            mask.transform.position = new Vector2(transform.position.x + t_offset, transform.position.y);

            if (t_offset <= -1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}