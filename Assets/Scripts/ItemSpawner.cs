using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private Item[] itemPool; // 이 지역에 맞는 아이템 풀
    [SerializeField] private int spawnCount = 5; // 아이템을 얼마나 스폰할지

    private void OnDrawGizmos() // 테스트 용
    {
        // Transform의 로컬 스케일을 가져와서 아이템을 스폰할 영역의 크기를 정의
        Vector3 areaSize = transform.localScale;

        // Gizmos의 색상을 설정 (여기서는 경계를 구분하기 쉽게 빨간색으로 설정)
        Gizmos.color = Color.red;

        // 지정된 영역을 직육면체(3D 공간에서)로 그리기 (크기는 areaSize로, 위치는 transform.position을 기준으로)
        Gizmos.DrawWireCube(transform.position, areaSize); // 3D에서 Z축도 고려한 크기
    }

    public void Initialize(Item[] newItemPool)
    {
        itemPool = newItemPool; // 지역에 맞는 아이템 리스트 할당
    }

    public void SpawnItems()
    {
        if (itemPool == null || itemPool.Length == 0)
        {
            Debug.LogWarning($"[ItemSpawner] {gameObject.name}: ItemPool이 설정되지 않았습니다.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = GetRandomPositionInArea(); // 랜덤 위치 계산
            Item randomItem = itemPool[Random.Range(0, itemPool.Length)];

            if (randomItem != null)
            {
                // ItemData를 Instantiate로 복제하여 새로운 값 설정 (값이 바뀔 때 원본에 영향을 안주기 위해서 복사본 사용)
                ItemData itemDataClone = Instantiate(randomItem.itemData);
                itemDataClone.value = Random.Range(itemDataClone.minValue, itemDataClone.maxValue + 1); // 랜덤 값 설정
                itemDataClone.dirty = Mathf.Round(Random.Range(0f, 1f) * 10f) / 10f; // 랜덤 값 설정

                // 새로운 아이템 생성
                GameObject newItem = Instantiate(randomItem.gameObject, randomPos, Quaternion.identity);
                Item item = newItem.GetComponent<Item>();
                if (item != null)
                {
                    item.SetItemData(itemDataClone); // 복사된 ItemData 설정
                    item.AssignUniqueId(); // 고유 Id 부여
                }
            }
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 areaSize = transform.localScale;  // 스포너 크기
        Vector3 center = transform.position;     // 스포너 중심 위치

        float randomX = Random.Range(center.x - areaSize.x / 2f, center.x + areaSize.x / 2f);
        float randomZ = Random.Range(center.z - areaSize.z / 2f, center.z + areaSize.z / 2f);
        
        float randomY = center.y;

        return new Vector3(randomX, randomY, randomZ); // 3D 위치 반환
    }
}