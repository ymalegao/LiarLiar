using UnityEngine;

public class Obstackle : MonoBehaviour
{
  private Rigidbody2D rb;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Obstackle"))
    {
      EggGameManager.Instance.subtractScore();
      Destroy(gameObject);
    }
    else if (other.CompareTag("Ground"))
    {
      Destroy(gameObject); // Egg disappears if missed
    }
  }
}
