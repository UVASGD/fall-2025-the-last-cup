using UnityEngine;

public class MouseAnimationManager : MonoBehaviour
{

    [SerializeField]
    public Animator animator;

    public void Run(bool runOrNot)
    {
        if (animator)
        {
            if (runOrNot)
            {
                Debug.Log("Running");
            }
            else
            {
                Debug.Log("Not running");
            }

            animator.SetBool("run", runOrNot);
        }
    }
    public void Jump()
    {
        animator.SetTrigger("jump");
    }

    public void Shake()
    {
        animator.SetTrigger("shake");
    }
}
