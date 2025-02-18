using UnityEngine;

public class Axe : MonoBehaviour
{
  private Rigidbody2D rb;
  private Animator animator;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Wood"))
    {
      WoodGameManager.Instance.AddScore();
      if (animator != null)
      {
        animator.SetTrigger("Chop");
      }
      Destroy(other.gameObject);
    }
  }
}
