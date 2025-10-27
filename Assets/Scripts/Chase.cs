using UnityEngine;
using UnityEngine.AI;

public class NPCVisionPatrol : MonoBehaviour
{
    [Header("Sight Settings")]
    public float viewRadius = 10f;
    public float viewAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Chase Settings")]
    public float timeToStartChasing = 2f;
    public float timeToStopChasing = 10f;
    [SerializeField] private float moneyLossRatio = 0.1f; // �÷��̾� �� ���� ����

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;

    private Transform player;
    private NavMeshAgent agent;

    private float visibleTimer = 0f;
    private float invisibleTimer = 0f;
    private bool isChasing = false;

    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        bool canSeePlayer = IsPlayerVisible();

        if (canSeePlayer)
        {
            visibleTimer += Time.deltaTime;
            invisibleTimer = 0f;

            if (!isChasing && visibleTimer >= timeToStartChasing)
            {
                StartChasing();
            }
        }
        else
        {
            invisibleTimer += Time.deltaTime;
            visibleTimer = 0f;

            if (isChasing && invisibleTimer >= timeToStopChasing)
            {
                StopChasing();
            }
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool IsPlayerVisible()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= viewRadius)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle <= viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distToPlayer, obstacleLayer))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void StartChasing()
    {
        isChasing = true;
        Debug.Log("NPC: �÷��̾� �߰� ����!");
    }

    void StopChasing()
    {
        isChasing = false;
        Debug.Log("NPC: �÷��̾ ���ļ� ������ ����.");
        GoToNextPatrolPoint();
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                GoToNextPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    // �÷��̾�� �浹 �� �� ����
    private void OnCollisionEnter(Collision collision)
    {
        if (isChasing && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("NPC�� �÷��̾ ��Ҵ�!");

            int currentMoney = PlayerManager.Instance.money;
            int lostAmount = Mathf.FloorToInt(currentMoney * moneyLossRatio);
            PlayerManager.Instance.money = Mathf.Max(0, currentMoney - lostAmount);

            Debug.Log($"�÷��̾� �� ����: -{lostAmount} (���� ��: {PlayerManager.Instance.money})");

            StopChasing();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 fovLine1 = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRadius;
        Vector3 fovLine2 = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
