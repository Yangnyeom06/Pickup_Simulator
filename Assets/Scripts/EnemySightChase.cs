using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemySightChase : MonoBehaviour
{
    public Transform player;
    public float viewAngle = 90f;
    public float viewDistance = 10f;
    public float detectionTime = 3f;
    public Image detectionGaugeUI;  // UI 연결

    private float timer = 0f;
    private bool moneyTaken = false;
    private NavMeshAgent agent;
    private PlayerManager playerManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerManager = playerController.player;
        }
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager를 찾을 수 없습니다.");
        }
        if (detectionGaugeUI != null)
            detectionGaugeUI.fillAmount = 0f;
    }

    void Update()
    {
        if (IsPlayerInSight())
        {
            timer += Time.deltaTime;
            agent.SetDestination(player.position);

            // 게이지 UI 업데이트
            if (detectionGaugeUI != null)
            {
                detectionGaugeUI.fillAmount = Mathf.Clamp01(timer / detectionTime);
            }

            if (timer >= detectionTime && !moneyTaken)
            {
                TakeMoney(0.50f);
                moneyTaken = true;
            }
        }
        else
        {
            timer = 0f;
            moneyTaken = false;
            agent.ResetPath();

            // 게이지 UI 초기화
            if (detectionGaugeUI != null)
                detectionGaugeUI.fillAmount = 0f;
        }
    }

    bool IsPlayerInSight()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < viewAngle / 2f && dirToPlayer.magnitude <= viewDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToPlayer.normalized, out hit, viewDistance))
            {
                if (hit.transform == player)
                    return true;
            }
        }
        return false;
    }

    void TakeMoney(float percentage)
    {
        int currentMoney = playerManager.money;
        int amountToTake = Mathf.FloorToInt(currentMoney * percentage);

        playerManager.money -= amountToTake;
        if (playerManager.money < 0) playerManager.money = 0;

        Debug.Log($"돈 {percentage * 100}% 차감 → {amountToTake}원. 현재 돈: {playerManager.money}");
    }
}
