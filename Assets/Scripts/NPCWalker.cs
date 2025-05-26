using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class NPCWalker : MonoBehaviour
{
    public Transform[] walkPoints;
    private NavMeshAgent agent;
    private Animator animator;
    private int currentIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        MoveToNextPoint();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        if (walkPoints.Length == 0) return;

        agent.destination = walkPoints[currentIndex].position;
        currentIndex = (currentIndex + 1) % walkPoints.Length;
    }
}
