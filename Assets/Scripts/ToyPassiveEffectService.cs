using System.Collections.Generic;
using UnityEngine;

public class ToyPassiveEffectService : MonoBehaviour
{
    [Header("장난감 식별 (ItemData.itemID)")]
    public string toyItemId = "TOY_PASSIVE";

    [Header("패시브 배율(보유 ≥1개)")]
    [Range(0.1f, 2f)] public float healthMul = 0.8f;    // 체력 20% 감소
    [Range(0.1f, 2f)] public float staminaMul = 0.75f;  // 스태미나 25% 감소

    [Header("중첩 규칙")]
    public bool stackable = false;
    public int  maxStacks = 3;

    [Header("참조 (가능하면 인스펙터에서 직접 할당)")]
    public InventoryManager inv;      // ← 인스펙터 드래그 권장
    public PlayerManager player;      // ← 인스펙터 드래그 권장

    private readonly List<ItemData> _owned = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 1) 인스펙터 참조가 비어 있으면 새 API로 탐색
        if (inv == null)
        {
            // 씬에 하나만 있다고 가정하면 이걸 권장
            inv = Object.FindFirstObjectByType<InventoryManager>();
            if (inv == null)
            {
                // 여러 개가 있어도 아무거나면 더 빠른 이걸로
                inv = Object.FindAnyObjectByType<InventoryManager>();
            }
        }

        if (player == null)
        {
            player = Object.FindFirstObjectByType<PlayerManager>();
            if (player == null)
            {
                player = Object.FindAnyObjectByType<PlayerManager>();
            }
        }

        if (inv == null || player == null)
        {
            Debug.LogWarning("[ToyPassive] 필요한 매니저(Inventory/Player)를 찾지 못했습니다. 씬에 존재하는지/인스펙터 연결 확인!");
            return;
        }

        // 2) 인벤토리 이벤트 구독
        inv.OnItemAdded   += HandleAdded;
        inv.OnItemRemoved += HandleRemoved;

        // 3) 현재 보유 스캔 → 초기 적용
        RescanInventoryAndApply();
    }

    private void OnDestroy()
    {
        if (inv != null)
        {
            inv.OnItemAdded   -= HandleAdded;
            inv.OnItemRemoved -= HandleRemoved;
        }
    }

    private void HandleAdded(ItemData data)
    {
        if (data != null && data.itemID == toyItemId)
        {
            _owned.Add(data);
            Reapply();
        }
    }

    private void HandleRemoved(ItemData data)
    {
        if (data != null && data.itemID == toyItemId)
        {
            _owned.Remove(data); // 동일 참조 제거
            Reapply();
        }
    }

    private void RescanInventoryAndApply()
    {
        _owned.Clear();
        if (inv?.slotList == null) { Reapply(); return; }

        foreach (var slot in inv.slotList)
        {
            var it = slot?.currentItem;
            if (it != null && it.itemID == toyItemId)
                _owned.Add(it);
        }
        Reapply();
    }

    private void Reapply()
    {
        if (player == null) return;

        if (_owned.Count == 0)
        {
            player.SetPassiveDrainMultipliers(1f, 1f);
            return;
        }

        if (!stackable)
        {
            player.SetPassiveDrainMultipliers(healthMul, staminaMul);
        }
        else
        {
            int stacks = Mathf.Min(_owned.Count, Mathf.Max(1, maxStacks));
            float h = Mathf.Pow(healthMul,  stacks);
            float s = Mathf.Pow(staminaMul, stacks);
            player.SetPassiveDrainMultipliers(h, s);
        }
    }
}