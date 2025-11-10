using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    [SerializeField] public ItemListData itemListData; // 아이템 리스트
    [SerializeField] private List<ItemSpawner> roadSpawners; // 길 스포너 리스트
<<<<<<< HEAD
    [SerializeField] private List<ItemSpawner> slumSpawners; // 빈민가 스포너 리스트
=======
    [SerializeField] private List<ItemSpawner> mountainSpawners; // 산 스포너 리스트
>>>>>>> origin/dev/Junk
    [SerializeField] private List<ItemSpawner> elementSchoolSpawners; // 초등학교 스포너 리스트
    [SerializeField] private List<ItemSpawner> middleSchoolSpawners; // 중학교 스포너 리스트
    [SerializeField] private List<ItemSpawner> citySpawners; //도시 스포너 리스트

    public Dictionary<ItemRarity, float> rarityProbabilities = new Dictionary<ItemRarity, float>
    {
        { ItemRarity.Common, 0.7f },
        { ItemRarity.Rare, 0.25f },
        { ItemRarity.Unique, 0.05f }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void Start()
    {
        // 각 지역별 스포너 초기화
        InitializeSpawners();
        
        // 아이템 스폰 시작
        SpawnAllItems();
    }

    private void InitializeSpawners()
    {
        InitializeSpawnerList(roadSpawners, itemListData.roadItems);
<<<<<<< HEAD
        InitializeSpawnerList(slumSpawners, itemListData.slumItems);
=======
        InitializeSpawnerList(mountainSpawners, itemListData.mountainItems);
>>>>>>> origin/dev/Junk
        InitializeSpawnerList(elementSchoolSpawners, itemListData.elementSchoolItems);
        InitializeSpawnerList(middleSchoolSpawners, itemListData.middleSchoolItems);
        InitializeSpawnerList(citySpawners, itemListData.cityItems);
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
        SpawnItemsInList(slumSpawners);
        SpawnItemsInList(elementSchoolSpawners);
        SpawnItemsInList(middleSchoolSpawners);
        SpawnItemsInList(citySpawners);
    }

    private void SpawnItemsInList(List<ItemSpawner> spawnerList)
    {
        foreach (ItemSpawner spawner in spawnerList)
        {
            spawner.SpawnItems();
        }
    }
}