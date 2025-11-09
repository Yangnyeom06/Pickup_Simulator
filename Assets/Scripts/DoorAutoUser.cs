using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DoorAutoUser : MonoBehaviour
{
    public float detectDistance = 2f;
    public float detectRadius = 0.4f;
    public LayerMask doorMask = ~0;
    public float rayOriginHeight = 1f;
    public float cooldown = 0.8f;

    private NavMeshAgent agent;
    private float lastUseTime = -10f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent.isOnNavMesh || agent.velocity.sqrMagnitude < 0.01f)
            return;
        if (Time.time < lastUseTime + cooldown)
            return;

        Vector3 origin = transform.position + Vector3.up * rayOriginHeight;
        Vector3 dir = agent.velocity.normalized;
        if (Physics.SphereCast(origin, detectRadius, dir, out RaycastHit hit, detectDistance, doorMask))
        {
            // DoorInteract 또는 SingleDoorInteract 둘 다 지원
            var door = hit.collider.GetComponentInParent<DoorInteract>();
            if (door != null)
            {
                door.Interact();
                lastUseTime = Time.time;
                return;
            }

            var single = hit.collider.GetComponentInParent<SingleDoorInteract>();
            if (single != null)
            {
                single.Interact();
                lastUseTime = Time.time;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 origin = transform.position + Vector3.up * rayOriginHeight;
        Vector3 dir = Application.isPlaying && GetComponent<NavMeshAgent>() && GetComponent<NavMeshAgent>().velocity.sqrMagnitude > 0.001f
            ? GetComponent<NavMeshAgent>().velocity.normalized
            : transform.forward;

        Gizmos.DrawWireSphere(origin, detectRadius);
        Gizmos.DrawLine(origin, origin + dir * detectDistance);
        Gizmos.DrawWireSphere(origin + dir * detectDistance, detectRadius);
    }
}
