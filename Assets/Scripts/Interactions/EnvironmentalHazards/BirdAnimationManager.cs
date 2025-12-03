using UnityEngine;

public class BirdAnimationManager : MonoBehaviour
{
    [SerializeField]
    public Animator animator;

    public void Fly(bool flyOrNot)
    {
        if (animator)
        {
            if (flyOrNot)
            {
                Debug.Log("Flying");
            }
            else
            {
                Debug.Log("Not flying");
            }

            animator.SetBool("flying", flyOrNot);
        }
    }

    public void Ruffle()
    {
        if (animator)
        {
            animator.SetTrigger("ruffle");
        }
    }

    public void Worried()
    {
        if (animator)
        {
            animator.SetTrigger("worried");
        }
    }
}
