using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    private float speed;
    public FishingManager fishingManager;

    private void Awake()
    {
        speed = Random.Range(3f,4.6f);
    }

    private void Start(){
        fishingManager = GameObject.FindFirstObjectByType<FishingManager>();
    }

    private void FixedUpdate()
    {

         transform.position += Vector3.left * speed * Time.deltaTime;
        // Destroy fish when it leaves the screen
        Destroy(gameObject, 7f);
    }

    private void OnMouseDown(){
        fishingManager.EndGame();
        Destroy(gameObject);
    }
}
