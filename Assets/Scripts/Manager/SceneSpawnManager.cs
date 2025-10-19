using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    void Start()
    {
        string spawnName = SceneChangeManager.Instance.nextSpawnPointName;
        GameObject spawnPoint = GameObject.Find(spawnName);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("스폰 위치 또는 플레이어를 찾을 수 없습니다.");
        }
    }
}
