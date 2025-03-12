using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float speed;
    public FishingManager fishingManager;

    private void Awake()
    {
        speed = Random.Range(2,6);
    }
    private void Start(){
        fishingManager = GameObject.FindFirstObjectByType<FishingManager>();
    }
    private void FixedUpdate()
    {

         transform.position += Vector3.left * speed * Time.deltaTime;
        // Destroy fish when it leaves the screen
        Destroy(gameObject, 6f);
    }
    private void Update(){
    }

    private void OnMouseDown(){
        fishingManager.score += 1;
        fishingManager.UpdateUI();
        Destroy(gameObject);
    }
}
