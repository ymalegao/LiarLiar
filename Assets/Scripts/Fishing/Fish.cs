using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float speed;
    public FishingManager fishingManager;
    public Camera FishCamera; 

    private void Awake()
    {
        speed = Random.Range(2,6);
    }
    private void Start(){
        fishingManager = GameObject.FindFirstObjectByType<FishingManager>();
        FishCamera = fishingManager.GetComponentInChildren<Camera>();
    }
    private void FixedUpdate()
    {

         transform.position += Vector3.left * speed * Time.deltaTime;
        // Destroy fish when it leaves the screen
        Destroy(gameObject, 6f);
    }
    private void Update(){
        Vector2 mouseWorldPos = FishCamera.ScreenToWorldPoint(Input.mousePosition);
        if(Vector2.Distance(mouseWorldPos, transform.position) < 0.4f){
            fishingManager.score += 1;
            fishingManager.UpdateUI();
            Destroy(gameObject);
        }
    }
}
