using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemList itemList; // 아이템 리스트
    [SerializeField] private List<ItemSpawner> roadSpawners; // 길 스포너 리스트
    [SerializeField] private List<ItemSpawner> mountainSpawners; // 산 스포너 리스트
    [SerializeField] private List<ItemSpawner> elementSchoolSpawners; // 초등학교 스포너 리스트
    [SerializeField] private List<ItemSpawner> middleSchoolSpawners; // 중학교 스포너 리스트

    private void Start()
    {
        // 각 지역별 스포너 초기화
        InitializeSpawners();
        
        // 아이템 스폰 시작
        SpawnAllItems();
    }

    private void InitializeSpawners()
    {
        InitializeSpawnerList(roadSpawners, itemList.roadItems);
        InitializeSpawnerList(mountainSpawners, itemList.mountainItems);
        InitializeSpawnerList(elementSchoolSpawners, itemList.elementSchoolItems);
        InitializeSpawnerList(middleSchoolSpawners, itemList.middleSchoolItems);
    }

    private void InitializeSpawnerList(List<ItemSpawner> spawnerList, Item[] itemArray)
    {
        foreach (ItemSpawner spawner in spawnerList)
        {
            spawner.Initialize(itemArray); // 스포너에 해당 아이템 풀 설정
        }
    }

    private void SpawnAllItems()
    {
        SpawnItemsInList(roadSpawners);
        SpawnItemsInList(mountainSpawners);
        SpawnItemsInList(elementSchoolSpawners);
        SpawnItemsInList(middleSchoolSpawners);
    }

    private void SpawnItemsInList(List<ItemSpawner> spawnerList)
    {
        foreach (ItemSpawner spawner in spawnerList)
        {
            spawner.SpawnItems();
        }
    }
}