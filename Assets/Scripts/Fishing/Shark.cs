using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    private float speed;

    private void Awake()
    {
        speed = Random.Range(2,4);
    }

    private void FixedUpdate()
    {

         transform.position += Vector3.left * speed * Time.deltaTime;
        // Destroy fish when it leaves the screen
        Destroy(gameObject, 7f);
    }
}
