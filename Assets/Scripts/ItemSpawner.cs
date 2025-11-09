using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
<<<<<<< HEAD
    public static ItemSpawner Instance { get; private set; }
    private Item[] itemPool; // 이 지역에 맞는 아이템 풀
    [SerializeField] private int spawnCount = 5; // 아이템을 얼마나 스폰할지

    // 희귀도 확률
    public Dictionary<ItemRarity, float> rarityProbabilities = new Dictionary<ItemRarity, float>
    {
        { ItemRarity.Common, 0.7f },
        { ItemRarity.Rare, 0.25f },
        { ItemRarity.Unique, 0.05f }
    };

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
=======
    // 이 스포너가 사용할 아이템 풀 (ItemManager에서 Initialize로 주입)
    private Item[] itemPool;
    private bool initialized;

    [Header("Spawn Settings")]
    [SerializeField] private int spawnCount = 5;        // 스폰 개수
    [SerializeField] private float raycastStartHeight = 10f;  // 위에서 Ray 쏠 높이
    [SerializeField] private float raycastMaxDistance = 50f;  // 최대 레이 거리 (startHeight 포함)
    [SerializeField] private float groundOffset = 0.01f;      // 바닥에서 아주 살짝 띄우기

    [Tooltip("바닥으로 인식할 레이어. 비워두면 모든 콜라이더 대상.")]
    [SerializeField] private LayerMask groundMask = ~0; // 기본: 모든 레이어

    // 희귀도 확률 (합계가 1.0이 되도록 유지)
    public Dictionary<ItemRarity, float> rarityProbabilities = new Dictionary<ItemRarity, float>
    {
        { ItemRarity.Common, 0.7f },
        { ItemRarity.Rare,   0.25f },
        { ItemRarity.Unique, 0.05f }
    };

    private void OnDrawGizmos() // 스폰 영역 시각화(테스트용)
    {
        Vector3 areaSize = transform.localScale;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }

    /// <summary>
    /// ItemManager가 카테고리별 아이템 배열을 주입
    /// </summary>
    public void Initialize(Item[] newItemPool)
    {
        itemPool = newItemPool;
        initialized = (itemPool != null && itemPool.Length > 0);
        Debug.Log($"[ItemSpawner:{name}] Initialize() - pool={(itemPool == null ? "null" : itemPool.Length + "ea")}");
    }

    /// <summary>
    /// 스폰 실행 (Initialize 이후 호출 전제)
    /// </summary>
    public void SpawnItems()
    {
        if (!initialized)
        {
            Debug.LogWarning($"[ItemSpawner:{name}] SpawnItems() called before Initialize or pool empty.");
            return;
        }

        if (spawnCount <= 0)
        {
            Debug.LogWarning($"[ItemSpawner:{name}] spawnCount <= 0, skip.");
            return;
        }

        if (itemPool == null || itemPool.Length == 0)
        {
            Debug.LogWarning($"[ItemSpawner:{name}] itemPool is null/empty.");
>>>>>>> 74b4bcf0 (update)
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
<<<<<<< HEAD
            Vector3 randomPos = GetRandomPositionInArea(); // 랜덤 위치 계산
            
            // 희귀도 뽑기
            ItemRarity selectedRarity = GetRandomRarity();

            // 해당 희귀도의 아이템만 추리기
            List<Item> filteredItems = new List<Item>();
            foreach (var item in itemPool)
            {
                if (item.itemData.itemRarity == selectedRarity)
                {
                    filteredItems.Add(item);
=======
            // 1) 영역 내부에서 랜덤 XY(=XZ) 추출
            Vector3 randomXZ = GetRandomXZInArea();

            // 2) 위에서 아래로 Raycast → 바닥 정확히 맞추기
            if (!TryGetGroundPoint(randomXZ, out Vector3 groundPoint))
            {
                // 바닥을 못 찾은 경우: 영역 중심 높이로 Fallback
                groundPoint = new Vector3(randomXZ.x, transform.position.y, randomXZ.z);
                Debug.LogWarning($"[ItemSpawner:{name}] Ground not found by Raycast at {randomXZ}. Using fallback Y.");
            }

            Vector3 spawnPos = groundPoint + Vector3.up * groundOffset;

            // 희귀도 선택
            ItemRarity selectedRarity = GetRandomRarity();

            // 3) 희귀도 필터링 (null 가드 포함)
            List<Item> filteredItems = new List<Item>();
            foreach (var it in itemPool)
            {
                if (it == null)
                {
                    Debug.LogWarning($"[ItemSpawner:{name}] itemPool contains a null element.");
                    continue;
                }
                if (it.itemData == null)
                {
                    Debug.LogWarning($"[ItemSpawner:{name}] {it.name} has null itemData.");
                    continue;
                }
                if (it.itemData.itemRarity == selectedRarity)
                {
                    filteredItems.Add(it);
>>>>>>> 74b4bcf0 (update)
                }
            }

            if (filteredItems.Count == 0)
            {
<<<<<<< HEAD
                Debug.LogWarning($"[ItemSpawner] {gameObject.name}: {selectedRarity} 등급 아이템이 풀에 없습니다.");
                continue;
            }

            // 추린 리스트에서 랜덤 뽑기
            Item randomItem = filteredItems[Random.Range(0, filteredItems.Count)];

            if (randomItem != null)
            {
                // ItemData를 Instantiate로 복제하여 새로운 값 설정 (값이 바뀔 때 원본에 영향을 안주기 위해서 복사본 사용)
                ItemData itemDataClone = Instantiate(randomItem.itemData);
                itemDataClone.value = Random.Range(itemDataClone.minValue, itemDataClone.maxValue + 1); // 랜덤 값 설정
                itemDataClone.price = itemDataClone.value;
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
    
    private ItemRarity GetRandomRarity()
    {
        float roll = Random.value; // 0 ~ 1 사이 값
        float cumulative = 0f;

        foreach (var kvp in ItemManager.Instance.rarityProbabilities)
=======
                Debug.LogWarning($"[ItemSpawner:{name}] No items for rarity {selectedRarity}.");
                continue;
            }

            // 4) 랜덤 선택 + ItemData 기반으로 스폰
            Item template = filteredItems[Random.Range(0, filteredItems.Count)];
            if (template == null)
            {
                Debug.LogWarning($"[ItemSpawner:{name}] Template is null after selection.");
                continue;
            }
            if (template.itemData == null)
            {
                Debug.LogWarning($"[ItemSpawner:{name}] Template {template.name} has null itemData.");
                continue;
            }

            // ScriptableObject 복제 (원본 보호)
            ItemData itemDataClone = Instantiate(template.itemData);
            if (itemDataClone == null)
            {
                Debug.LogError($"[ItemSpawner:{name}] Failed to clone ItemData from {template.name}.");
                continue;
            }

            // 값 랜덤화
            itemDataClone.value = Random.Range(itemDataClone.minValue, itemDataClone.maxValue + 1);
            itemDataClone.price = itemDataClone.value;
            itemDataClone.dirty = Mathf.Round(Random.Range(0f, 1f) * 10f) / 10f;

            // 5) 스폰 프리팹 결정: ItemData.prefab 우선, 없으면 템플릿의 gameObject
            GameObject prefabToSpawn = itemDataClone.prefab != null ? itemDataClone.prefab : template.gameObject;
            if (prefabToSpawn == null)
            {
                Debug.LogError($"[ItemSpawner:{name}] Both ItemData.prefab and template GameObject are null ({template.name}).");
                continue;
            }

            GameObject spawnedGO = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            if (spawnedGO == null)
            {
                Debug.LogError($"[ItemSpawner:{name}] Instantiate returned null.");
                continue;
            }

            // 6) Item 컴포넌트에 데이터 적용
            Item spawnedItem = spawnedGO.GetComponent<Item>();
            if (spawnedItem == null)
            {
                Debug.LogWarning($"[ItemSpawner:{name}] Spawned object has no Item component: {spawnedGO.name}");
                continue;
            }

            spawnedItem.SetItemData(itemDataClone);
            spawnedItem.AssignUniqueId();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 바닥 Raycast 관련

    /// <summary>
    /// 스포너 영역 내부의 무작위 XZ 좌표를 반환 (Y는 미포함).
    /// </summary>
    private Vector3 GetRandomXZInArea()
    {
        Vector3 areaSize = transform.localScale;   // 스포너 크기(가로/세로 범위 용도)
        Vector3 center = transform.position;

        float x = Random.Range(center.x - areaSize.x * 0.5f, center.x + areaSize.x * 0.5f);
        float z = Random.Range(center.z - areaSize.z * 0.5f, center.z + areaSize.z * 0.5f);

        return new Vector3(x, 0f, z);
    }

    /// <summary>
    /// (x,z) 위쪽에서 아래로 Raycast 하여 바닥 히트 포인트를 얻는다.
    /// </summary>
    private bool TryGetGroundPoint(Vector3 xz, out Vector3 groundPoint)
    {
        Vector3 start = new Vector3(xz.x, transform.position.y + raycastStartHeight, xz.z);

        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, raycastMaxDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            groundPoint = hit.point;
            return true;
        }

        groundPoint = default;
        return false;
    }

    // ─────────────────────────────────────────────────────────────────────────────

    private ItemRarity GetRandomRarity()
    {
        float roll = Random.value; // 0 ~ 1
        float cumulative = 0f;

        foreach (var kvp in rarityProbabilities)
>>>>>>> 74b4bcf0 (update)
        {
            cumulative += kvp.Value;
            if (roll <= cumulative)
                return kvp.Key;
        }
<<<<<<< HEAD

        return ItemRarity.Common; // fallback
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
=======
        return ItemRarity.Common; // fallback
    }
}
>>>>>>> 74b4bcf0 (update)
