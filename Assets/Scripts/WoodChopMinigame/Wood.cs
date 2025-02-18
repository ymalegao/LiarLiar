using UnityEngine;

public class Wood : MonoBehaviour
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
    if (other.CompareTag("Axe"))
    {
      WoodGameManager.Instance.AddScore();
      if (animator != null)
      {
        animator.SetTrigger("Chop");
      }
      Destroy(gameObject, 0.3f); // Add delay to allow chop animation
    }
    else if (other.CompareTag("Ground"))
    {
      Destroy(gameObject); // Wood disappears if missed
    }
  }
}
