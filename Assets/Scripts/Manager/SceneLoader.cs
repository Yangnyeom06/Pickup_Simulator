using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string targetSceneName = "Slum";
    public string targetSpawnPoint = "SlumSpawn";
    public int slotId = 0; // 현재 저장 중인 슬롯 ID

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveManager.Instance.SaveGame(slotId);
            SceneChangeManager.Instance.LoadPlayScene(targetSceneName, slotId, targetSpawnPoint);
        }
    }
}
