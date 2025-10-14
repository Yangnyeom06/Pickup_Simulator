using UnityEngine;
using System.Collections;

public class SingleDoorInteract : MonoBehaviour
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
        animator.SetTrigger("OneOpen");
        isOpened = true;

        if (doorCollider != null) doorCollider.enabled = false;

        if (closeRoutine != null) StopCoroutine(closeRoutine);
        closeRoutine = StartCoroutine(AutoCloseCoroutine());
    }

    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(3f);

        animator.SetTrigger("OneClose");
        if (doorCollider != null) doorCollider.enabled = true;
        isOpened = false;
    }
}
