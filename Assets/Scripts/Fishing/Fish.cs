using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private RectTransform rectTransform;
    private float speed;

    public void Initialize()
    {
        speed = Random.Range(100,200);
    }

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void Update()
    {

        // Move fish to the left
        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Destroy fish when it leaves the screen
        if (rectTransform.anchoredPosition.x < -Screen.width / 2)
        {
            Destroy(gameObject);
        }
    }
}
