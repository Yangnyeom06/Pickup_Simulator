using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class AreaPortalWithFade : MonoBehaviour
{
    [Header("Teleport Target")]
    [SerializeField] private Transform target;

    [Header("Player Transform Changes")]
    [SerializeField] private Vector3 targetScale = Vector3.one;
    [SerializeField] private bool alignToTargetRotation = false; 
    [SerializeField] private bool keepYRotationOnly = false; 

    [Header("Ground Snap (����ĳ��Ʈ�� �ٴ� ���߱�)")]
    [SerializeField] private bool snapToGround = true;
    [SerializeField] private float raycastStartHeight = 5f;
    [SerializeField] private float raycastMaxDistance = 50f;
    [SerializeField] private float groundOffset = 0.02f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("References")]
    [SerializeField] private FadeInOut fade;
    [SerializeField] private string playerTag = "Player";

    private bool busy;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (busy) return;
        if (!other.CompareTag(playerTag)) return;
        if (target == null || fade == null)
        {
            return;
        }

        busy = true;

        fade.StartFadeInAndOut(() =>
        {
            DoTeleport(other.transform);
        });

        Invoke(nameof(Unlock), fade.fadeTime + fade.delayTime + 0.2f);
    }

    private void Unlock() => busy = false;

    public void DoTeleport(Transform player)
    {
        if (player == null) return;

        var cc = player.GetComponent<CharacterController>();
        var agent = player.GetComponent<NavMeshAgent>();
        var rb = player.GetComponent<Rigidbody>();

        bool ccWasEnabled = cc ? cc.enabled : false;
        bool agentWasEnabled = agent ? agent.enabled : false;
        bool rbWasKinematic = rb ? rb.isKinematic : false;

        if (cc) cc.enabled = false;
        if (agent) agent.enabled = false;
        if (rb)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Quaternion rot = player.rotation;
        if (alignToTargetRotation)
        {
            rot = target.rotation;
            if (keepYRotationOnly)
            {
                var e = rot.eulerAngles;
                rot = Quaternion.Euler(0f, e.y, 0f);
            }
        }

        Vector3 dest = target.position;
        if (snapToGround)
        {
            Vector3 start = new Vector3(dest.x, dest.y + raycastStartHeight, dest.z);
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, raycastMaxDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                dest = hit.point + Vector3.up * groundOffset;
            }
        }

        player.position = dest;
        player.rotation = rot;
        player.localScale = targetScale;

        if (agent)
        {
            agent.enabled = true;
            agent.Warp(dest);
            agent.ResetPath();
            if (!agentWasEnabled) agent.enabled = false;
        }

        if (cc) cc.enabled = ccWasEnabled;
        if (rb) rb.isKinematic = rbWasKinematic;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.DrawWireSphere(target.position, 0.35f);
    }
}
