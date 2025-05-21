using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public Transform contentParent; // ScrollView 안 content
    public GameObject slotPrefab;
    public IntValueSO inventorySlotCount; // 업그레이드 반영된 슬롯 수
    public GameObject inventory;
    public SaveManager saveManager;
    [SerializeField] private List<InventorySlotData> slotList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        inventorySlotCount.OnValueChanged += UpdateSlots;
        UpdateSlots(inventorySlotCount.Value);
    }

    private void OnDisable()
    {
        inventorySlotCount.OnValueChanged -= UpdateSlots;
    }

    private void UpdateSlots(int newCount)
    {
        // 기존 슬롯 정리
        foreach (var slot in slotList)
        {
            Destroy(slot);
        }
        slotList.Clear();

        // 새로운 슬롯 생성
        for (int i = 0; i < newCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, contentParent);
            InventorySlotData slotData = slot.GetComponentInChildren<InventorySlotData>();
            slotList.Add(slotData);
        }
    }

    public bool AddItem(ItemData item)
    {
        foreach (var slot in slotList)
        {
            if (slot.currentItem == null) // 빈 슬롯 발견
            {
                slot.SetItem(item);
                return true; // 아이템 추가 성공
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false; // 실패
    }

    public void Upgrade()
    {

    }

    public void Open() // UpGradeUI 열기
    {
        inventory.SetActive(true);
    }

    public void Exit() // UpGradeUI 닫기
    {
        inventory.SetActive(false);
    }
}