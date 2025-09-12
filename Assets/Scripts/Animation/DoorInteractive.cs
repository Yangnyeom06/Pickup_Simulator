using UnityEngine;
using System.Collections;

public class DoorInteract : MonoBehaviour
{
    private Animator animator;
    private bool isOpened = false;
    private Coroutine closeRoutine;
    private Collider doorCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();
    }

    public void Interact()
    {
        if (!isOpened)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        animator.SetTrigger("OPEN");
        isOpened = true;

        if(doorCollider != null) doorCollider.enabled = false;

        // 닫는 코루틴 시작
        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(AutoCloseCoroutine());
    }

    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(3f); // 3초 대기

        animator.SetTrigger("CLOSE");
        if (doorCollider != null) doorCollider.enabled = true;
        isOpened = false;
    }
}
