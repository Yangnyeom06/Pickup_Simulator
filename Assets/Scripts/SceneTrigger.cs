using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Header("전환할 씬 이름")]
    public string targetSceneName = "Slum";

    [Header("페이드 처리")]
    public FadeInOut fadeController;

    [Header("스폰 포인트 이름 (선택)")]
    public string nextSpawnPoint = "SpawnPoint";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fadeController.StartFadeInAndOut(() =>
            {
                SceneChangeManager.Instance.LoadPlayScene(targetSceneName, SceneChangeManager.Instance.selectSlotId, nextSpawnPoint);
            });
        }
    }
}
