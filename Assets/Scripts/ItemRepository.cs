using System.Collections.Generic;
using UnityEngine;

public class ItemRepository : MonoBehaviour
{
    public static ItemRepository Instance { get; private set; }

    // 아이템 원본 오브젝트 (게임 중)
    public List<GameObject> collectedObjects = new();

    // 세이브용 직렬화 데이터
    public List<ItemInstanceData> collectedItemDatas = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
        {
            if (!collectedObjects.Contains(other.gameObject))
            {
                collectedObjects.Add(other.gameObject);

                var itemData = item.itemData; // ItemData 구조를 가져옴
                collectedItemDatas.Add(new ItemInstanceData(
                    item.uniqueID,
                    itemData.itemID,
                    itemData.itemName,
                    itemData.icon,
                    itemData.description,
                    itemData.itemType,
                    itemData.dirty,
                    itemData.value,
                    collectedObjects.Count - 1,
                    other.transform.position,
                    other.transform.rotation

                ));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
        {
            if (collectedObjects.Contains(other.gameObject))
            {
                int index = collectedObjects.IndexOf(other.gameObject);

                collectedObjects.RemoveAt(index);
                collectedItemDatas.RemoveAt(index);
            }
        }
    }

    public void SpawnLoadedItems()
    {
        // 기존에 남아있는 오브젝트 정리
        foreach (var obj in collectedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        collectedObjects.Clear();

        // 저장된 데이터대로 복원
        foreach (var data in collectedItemDatas)
        {
            // 1) ItemData 찾기
            ItemData itemData = FindItemDataInList(data.itemID);
            if (itemData == null || itemData.prefab == null)
            {
                Debug.LogWarning($"ItemData (ID:{data.itemID})을 ItemListData에서 찾을 수 없음!");
                continue;
            }

            // 2) 프리팹 스폰
            GameObject obj = Instantiate(itemData.prefab, data.position, data.rotation);
            collectedObjects.Add(obj);

            // 3) Item 컴포넌트에 데이터 적용
            var itemComp = obj.GetComponent<Item>();
            if (itemComp != null)
            {
                itemComp.SetItemData(itemData);
                itemComp.uniqueID = data.uniqueID;
            }
        }
    }

    private ItemData FindItemDataInList(string id)
    {
        foreach (var item in ItemManager.Instance.itemListData.roadItems)
            if (item != null && item.itemData.itemID == id) return item.itemData;

        foreach (var item in ItemManager.Instance.itemListData.slumItems)
            if (item != null && item.itemData.itemID == id) return item.itemData;

        foreach (var item in ItemManager.Instance.itemListData.elementSchoolItems)
            if (item != null && item.itemData.itemID == id) return item.itemData;

        foreach (var item in ItemManager.Instance.itemListData.middleSchoolItems)
            if (item != null && item.itemData.itemID == id) return item.itemData;

        return null;
    }

    public void ResetCollectedItems()
    {
        collectedItemDatas.Clear();
        collectedObjects.Clear();
    }

    private void OnDrawGizmos() // 테스트 용
    {
        // Transform의 로컬 스케일을 가져와서 아이템을 스폰할 영역의 크기를 정의
        Vector3 areaSize = transform.localScale;

        // Gizmos의 색상을 설정 (여기서는 경계를 구분하기 쉽게 빨간색으로 설정)
        Gizmos.color = Color.blue;

        // 지정된 영역을 직육면체(3D 공간에서)로 그리기 (크기는 areaSize로, 위치는 transform.position을 기준으로)
        Gizmos.DrawWireCube(transform.position, areaSize); // 3D에서 Z축도 고려한 크기
    }
}