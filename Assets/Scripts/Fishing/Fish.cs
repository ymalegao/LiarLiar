using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private RectTransform rectTransform;
    private float speed;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        speed = Random.Range(100,200);
    }

    private void FixedUpdate()
    {

        rectTransform.anchoredPosition +=  Vector2.left * speed * Time.fixedDeltaTime;
        // Destroy fish when it leaves the screen
        if (rectTransform.anchoredPosition.x < -Screen.width / 2)
        {
            Destroy(gameObject);
        }
    }
}
