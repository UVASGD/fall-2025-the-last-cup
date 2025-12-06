using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RatBehavior : MonoBehaviour
{
    // Animator helpers
    private Animator animator;

    private void Run(bool runOrNot)
    {
        if (animator)
        {
            animator.SetTrigger("run");
        }
    }
    private void Jump()
    {
        animator.SetTrigger("jump");
    }

    private void Shake()
    {
        animator.SetTrigger("shake");
    }

    private static bool foundPipes = false;
    private static PipeNodes[] pipes;

    // If no starting pipe is established it defaults to the nearest pipe
    [SerializeField]
    PipeNodes currentPipe;

    [SerializeField]
    float traverseSpeed;

    [SerializeField]
    float snapNodeThreshold;

    [SerializeField]
    float stunTime;

    private int currentPipeNode;
    private bool goingForward;
    
    [SerializeField]
    private float currentStunTime = 0;

    AudioSource mouseLoopingSource;

    struct PipeLoc
    {
        public PipeNodes pipe;
        public int nodeIdx;
    };

    void OnTriggerEnter(Collider collision)
    {
        print("WOW " + collision.gameObject.tag);
        if (collision.gameObject.tag == "WaterProjectile")
        {
            if (mouseLoopingSource != null)
            {
                AudioManager.audioManagerInstance.StopLoopingSFX(mouseLoopingSource);
                mouseLoopingSource = null;
            }

            AudioManager.audioManagerInstance.PlaySFX(AudioManager.audioManagerInstance.mouseAttacked);
            
            Shake();
            currentStunTime = stunTime;
        }
    }

    PipeLoc findNearest(PipeNodes[] checkPipes, PipeNodes excludePipe)
    {
        bool foundSingle = false;
        PipeLoc ret = new PipeLoc();
        float minDistance = float.MaxValue;
        foreach(PipeNodes pipe in checkPipes) {
            if (excludePipe != pipe)
            {
                foundSingle = true;
                for (int i = 0 ; i < pipe.pipeNodes.Length ; i++)
                {
                    if ((pipe.pipeNodes[i].position - gameObject.transform.position).magnitude < minDistance)
                    {
                        ret.pipe = pipe;
                        ret.nodeIdx = i;
                    }
                } 
            }
        }
        if (!foundSingle)
        {
            print("ERROR: No other pipe nodes found");
        }

        return ret;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            print("ERROR: animator not found");
            Destroy(this);
            return;
        }

        mouseLoopingSource = AudioManager.audioManagerInstance.PlayLoopingSFX(AudioManager.audioManagerInstance.mouse);

        if (!foundPipes)
        {
            pipes = FindObjectsByType<PipeNodes>(FindObjectsSortMode.None);
        }

        PipeLoc loc;
        // Finds nearest pipe node to attach to 
        if (currentPipe == null)
        {
            loc = findNearest(pipes,null);
        }
        // Finds nearest singular part of pipe to attach to
        else
        {
            PipeNodes[] singleList = new PipeNodes[1];
            singleList[0] = currentPipe;
            loc = findNearest(singleList, null);
        }

        currentPipe = loc.pipe;
        currentPipeNode = loc.nodeIdx;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStunTime <= 0)
        {
            Run(true);
            Vector3 currentToNodeDiff = currentPipe.pipeNodes[currentPipeNode].position - gameObject.transform.position;
            // Reaches the next node then proceeds to set a new targeted node
            // Right now it just reverse direction on the pipe ending
            if (currentToNodeDiff.magnitude < snapNodeThreshold)
            {
                gameObject.transform.position = currentPipe.pipeNodes[currentPipeNode].position;
                if (goingForward)
                {
                    // Reached end, reverses direction
                    if (currentPipeNode == currentPipe.pipeNodes.Length - 1)
                    {
                        goingForward = false;
                        currentPipeNode = currentPipe.pipeNodes.Length - 2;
                    }
                    else
                    {
                        currentPipeNode++;
                    }
                }
                else
                {
                    if (currentPipeNode == 0)
                    {
                        goingForward = true;
                        currentPipeNode = 1;
                    }
                    else
                    {
                        currentPipeNode--;
                    }
                }
            }
            else
            {
                Vector3 currentToNodeDir = Vector3.Normalize(currentPipe.pipeNodes[currentPipeNode].position - gameObject.transform.position);
                gameObject.transform.position += (currentToNodeDir * traverseSpeed * Time.deltaTime);
                gameObject.transform.rotation = Quaternion.Euler(0,Mathf.Rad2Deg * Mathf.Atan2(currentToNodeDir.x,currentToNodeDir.z),0);
            }
        }
        else
        {
            Shake();
            currentStunTime -= Time.deltaTime;
        }

    }
}
