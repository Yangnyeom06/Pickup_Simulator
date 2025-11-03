using UnityEngine;
using System.Collections;

public class EndingMapTrigger : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger; // Inspector에서 연결
    public string dialogueName = "NoEntry";
    public GameObject invisibleWall; // 지나가지 못하게 하는 벽
    public float wallDuration = 2f; // 벽이 유지되는 시간

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 트리거에 진입함");

            hasTriggered = true;

            dialogueTrigger.TriggerDialouge(dialogueName);
            Debug.Log("대사 시작됨");

            StartCoroutine(HandleWallRoutine());
        }
    }


    IEnumerator HandleWallRoutine()
    {
        // 3. 벽 ON
        if (invisibleWall != null)
            invisibleWall.SetActive(true);

        // 4. 대사 시간과 벽 지속 시간 대기
        yield return new WaitForSeconds(wallDuration);

        // 5. 벽 OFF
        if (invisibleWall != null)
            invisibleWall.SetActive(false);

        // 6. 트리거 재사용 가능하게 (필요시 제거)
        hasTriggered = false;
    }
}
