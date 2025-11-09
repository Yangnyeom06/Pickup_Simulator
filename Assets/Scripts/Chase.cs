using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCPatrolChaseRobust : MonoBehaviour
{
    [Header("Waypoints (루트의 빈 오브젝트 아래에 두는 걸 권장)")]
    public Transform[] waypoints;

    [Header("Sight / Chase")]
    public float viewRadius = 12f;
    public float viewAngle = 110f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public float timeToStartChasing = 0.6f;   // 시야 유지 시 추격 시작까지
    public float timeToStopChasing = 4f;      // 시야 상실 시 추격 종료까지
    public bool stopChasingAfterCatch = true; // 한 번 잡으면 그 뒤로 추격 안함

    [Header("Move (걷기 전용)")]
    public float walkSpeed = 2.2f;
    public float acceleration = 8f;
    public float angularSpeed = 240f;
    public float arrivalDistance = 0.3f;      // 웨이포인트 도착 허용 오차
    public float waitAtPoint = 0.5f;          // 도착 후 대기
    public float stuckTimeout = 5f;           // 멈춤 보호

    [Header("Kick Out")]
    public Transform middleSchoolExit;        // 킥아웃 목적지(중학교 밖)
    public Vector3 kickOutScale = Vector3.one;
    public FadeInOut fade;                    // 씬의 FadeInOut

    [Header("Ground Snap")]
    public bool snapToGround = true;
    public float raycastStartHeight = 5f;
    public float raycastMaxDistance = 50f;
    public float groundOffset = 0.02f;
    public LayerMask groundMask = ~0;

    [Header("Dialogue / Money")]
    public DialogueTrigger dialogueTrigger;   // 네 시스템
    public string dialogueName = "PayUp";     // DialogueLines의 name
    [Range(0f, 1f)] public float moneyLossRatio = 0.1f;

    [Header("Animator Params")]
    public string isWalkingParam = "IsWalking"; // Bool
    public string speedParam = "Speed";     // Float

    // ── 내부 상태 ──────────────────────────────────────────
    Transform player;
    NavMeshAgent agent;
    Animator anim;
    bool isChasing = false;
    bool isKickingOut = false;
    bool hasCaughtPlayer = false;

    int currentIndex = 0;
    float patrolTimer = 0f;
    float visibleTimer = 0f;
    float invisibleTimer = 0f;

    // 멈춤 감지
    float lastMoveTime;
    Vector3 lastPos;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        ApplyWalkSettings();
        EnsureOnNavMesh();

        lastPos = transform.position;
        lastMoveTime = Time.time;

        if (waypoints != null && waypoints.Length > 0)
            SetDestinationSafe(waypoints[currentIndex].position);
    }

    void Update()
    {
        if (!IsAgentReady() || isKickingOut || player == null)
        {
            UpdateAnimator(0f);
            return;
        }

        // 이동/애니메이션 반영
        UpdateAnimator(agent.velocity.magnitude);

        // 이미 한 번 잡은 뒤엔 순찰만
        if (stopChasingAfterCatch && hasCaughtPlayer)
        {
            isChasing = false;
            Patrol();
            AfterMoveWatchdog();
            return;
        }

        // 시야 판정
        bool canSee = IsPlayerVisible();
        if (canSee)
        {
            visibleTimer += Time.deltaTime;
            invisibleTimer = 0f;
            if (!isChasing && visibleTimer >= timeToStartChasing)
                isChasing = true;
        }
        else
        {
            invisibleTimer += Time.deltaTime;
            visibleTimer = 0f;
            if (isChasing && invisibleTimer >= timeToStopChasing)
                isChasing = false;
        }

        if (isChasing) Chase();
        else Patrol();

        AfterMoveWatchdog();
    }

    // ── 시야: 첫 Raycast 히트가 Player면 보임 ───────────────
    bool IsPlayerVisible()
    {
        Vector3 to = player.position - transform.position;
        float dist = to.magnitude;
        if (dist > viewRadius) return false;

        Vector3 dir = to.normalized;
        float ang = Vector3.Angle(transform.forward, dir);
        if (ang > viewAngle * 0.5f) return false;

        int mask = playerLayer | obstacleLayer;
        if (Physics.Raycast(transform.position + Vector3.up, dir, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
            return hit.collider.CompareTag("Player");

        return false;
    }

    // ── 순찰(견고) ──────────────────────────────────────────
    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            agent.ResetPath();
            return;
        }
        if (agent.pathPending) return;

        // Partial/Invalid 경로면 다음 포인트 시도
        if (agent.hasPath && agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            GoNext();
            return;
        }

        float realDist = Vector3.Distance(transform.position, waypoints[currentIndex].position);
        if (agent.hasPath &&
            (agent.remainingDistance <= Mathf.Max(arrivalDistance, agent.stoppingDistance + 0.05f) ||
             realDist <= arrivalDistance))
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= waitAtPoint)
            {
                GoNext();
                patrolTimer = 0f;
            }
        }
        else if (!agent.hasPath)
        {
            SetDestinationSafe(waypoints[currentIndex].position);
        }
    }

    // ── 추격(걷기 속도) ─────────────────────────────────────
    void Chase()
    {
        if (player && agent.isOnNavMesh)
            SetDestinationSafe(player.position);
    }

    void GoNext()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        int tries = 0;
        do
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            tries++;
        }
        while (tries < waypoints.Length && waypoints[currentIndex] == null);

        if (waypoints[currentIndex] == null)
        {
            Debug.LogError($"[{name}] 모든 waypoints가 null");
            agent.ResetPath();
            return;
        }

        if (!SetDestinationSafe(waypoints[currentIndex].position))
        {
            Debug.LogWarning($"[{name}] Waypoint {currentIndex}가 NavMesh 밖. 다음으로 건너뜀.");
            currentIndex = (currentIndex + 1) % waypoints.Length;
            SetDestinationSafe(waypoints[currentIndex].position);
        }
    }

    bool SetDestinationSafe(Vector3 targetPos)
    {
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            return agent.SetDestination(hit.position);
        return false;
    }

    // ── 충돌: Idle → “돈 내놔” → 돈 차감 →(옵션)추격종료 → 킥아웃 ─
    void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Player")) return;
        if (isKickingOut) return;

        ForceIdle();

        if (dialogueTrigger && !string.IsNullOrEmpty(dialogueName))
            dialogueTrigger.TriggerDialouge(dialogueName);

        if (PlayerManager.Instance != null)
        {
            int current = PlayerManager.Instance.money;
            int loss = Mathf.FloorToInt(current * moneyLossRatio);
            PlayerManager.Instance.money = Mathf.Max(0, current - loss);
        }

        if (stopChasingAfterCatch)
        {
            hasCaughtPlayer = true;
            isChasing = false;
        }

        StartCoroutine(KickOutRoutine(c.transform));
    }

    IEnumerator KickOutRoutine(Transform playerTf)
    {
        isKickingOut = true;
        agent.ResetPath();

        if (fade)
        {
            fade.StartFadeInAndOut(() => { DoTeleport(playerTf); });
            yield return new WaitForSeconds(fade.fadeTime + fade.delayTime + 0.1f);
        }
        else
        {
            DoTeleport(playerTf);
            yield return null;
        }

        GoNext();
        isKickingOut = false;
    }

    void DoTeleport(Transform t)
    {
        if (!t || !middleSchoolExit) return;

        var cc = t.GetComponent<CharacterController>();
        var agentP = t.GetComponent<NavMeshAgent>();
        var rb = t.GetComponent<Rigidbody>();

        bool ccOn = cc ? cc.enabled : false;
        bool agOn = agentP ? agentP.enabled : false;
        bool rbKin = rb ? rb.isKinematic : false;

        if (cc) cc.enabled = false;
        if (agentP) agentP.enabled = false;
        if (rb) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        Vector3 dest = middleSchoolExit.position;
        if (snapToGround)
        {
            Vector3 start = new Vector3(dest.x, dest.y + raycastStartHeight, dest.z);
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, raycastMaxDistance, groundMask, QueryTriggerInteraction.Ignore))
                dest = hit.point + Vector3.up * groundOffset;
        }

        t.position = dest;
        t.rotation = middleSchoolExit.rotation;
        t.localScale = kickOutScale;

        if (agentP)
        {
            agentP.enabled = true;
            agentP.Warp(dest);
            agentP.ResetPath();
            if (!agOn) agentP.enabled = false;
        }
        if (cc) cc.enabled = ccOn;
        if (rb) rb.isKinematic = rbKin;
    }

    // ── 애니메이터 & 이동 가드 ─────────────────────────────
    void UpdateAnimator(float speed)
    {
        if (!string.IsNullOrEmpty(speedParam)) anim.SetFloat(speedParam, speed);
        if (!string.IsNullOrEmpty(isWalkingParam)) anim.SetBool(isWalkingParam, speed > 0.1f);
    }

    void ForceIdle()
    {
        if (agent && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
        UpdateAnimator(0f);
    }

    void AfterMoveWatchdog()
    {
        float moved = (transform.position - lastPos).sqrMagnitude;
        if (moved > 0.0001f) { lastMoveTime = Time.time; lastPos = transform.position; }
        else if (Time.time - lastMoveTime > stuckTimeout) { Debug.LogWarning($"[{name}] Stuck. Forcing next."); GoNext(); lastMoveTime = Time.time; lastPos = transform.position; }
    }

    bool IsAgentReady() => agent && agent.isActiveAndEnabled && agent.isOnNavMesh;

    void ApplyWalkSettings()
    {
        agent.speed = walkSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.autoBraking = false; // 포인트 간 부드럽게
    }

    void EnsureOnNavMesh()
    {
        if (!agent || !agent.isActiveAndEnabled) return;
        if (!agent.isOnNavMesh)
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                agent.Warp(hit.position);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (waypoints == null) return;
        for (int i = 0; i < waypoints.Length; i++)
        {
            var t = waypoints[i];
            if (!t) { Debug.LogWarning($"[{name}] waypoints[{i}] = NULL"); continue; }
            if (t.IsChildOf(transform))
                Debug.LogWarning($"[{name}] waypoints[{i}] '{t.name}' 이 NPC의 자식입니다. 루트의 Waypoints 아래로 빼세요.");
            for (int j = i + 1; j < waypoints.Length; j++)
                if (t == waypoints[j]) Debug.LogWarning($"[{name}] waypoints[{i}]와 [{j}]가 같은 Transform('{t.name}')을 공유.");
        }
    }
#endif

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;
        for (int i = 0; i < waypoints.Length; i++)
        {
            var t = waypoints[i]; if (!t) continue;
            Gizmos.color = (i == currentIndex) ? Color.yellow : Color.green;
            Gizmos.DrawWireSphere(t.position, 0.25f);
            var next = waypoints[(i + 1) % waypoints.Length];
            if (next) { Gizmos.color = Color.cyan; Gizmos.DrawLine(t.position, next.position); }
        }
        if (waypoints.Length > 0 && waypoints[currentIndex])
        { Gizmos.color = Color.yellow; Gizmos.DrawLine(transform.position, waypoints[currentIndex].position); }
    }

    void OnDrawGizmos()
    {
        // 기본 원: 시야 반경
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // 시야 각도 시각화
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = new Color(1, 0.5f, 0f, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewRadius);

#if UNITY_EDITOR
        // 반투명 부채꼴 시야 영역 (씬 뷰에서 직관적으로 보이게)
        UnityEditor.Handles.color = new Color(1, 0.3f, 0f, 0.15f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, leftDir, viewAngle, viewRadius);
#endif
    }

}
