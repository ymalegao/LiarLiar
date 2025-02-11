using UnityEngine;

public class Axe : MonoBehaviour
{
  private Rigidbody2D rb;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Wood"))
    {
      WoodGameManager.Instance.AddScore();
      Destroy(other.gameObject);
    }
  }
}