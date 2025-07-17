using UnityEngine;
using UnityEngine.AI;

public class AINavigator : MonoBehaviour
{
    public Transform[] waypoints; // 경로점
    private int currentWaypoint = 0;
    private NavMeshAgent agent;   // NavMeshAgent
    private Animator animator;    // Animator

    void Start()
    {
        // NavMeshAgent와 Animator 컴포넌트를 가져옵니다.
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 첫 번째 경로로 이동 시작
        GoToNextWaypoint();
    }

    void Update()
    {
        // 목표 지점에 도달했을 때
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }

        // 이동 중일 때 걷기 애니메이션을 재생
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void GoToNextWaypoint()
    {
        // 경로점이 없다면 종료
        if (waypoints.Length == 0) return;

        // 현재 경로점으로 이동
        agent.destination = waypoints[currentWaypoint].position;

        // 다음 경로점으로 인덱스를 증가시킴
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}
