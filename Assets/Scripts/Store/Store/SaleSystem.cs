using UnityEngine;

public class SellSystem : MonoBehaviour
{
    public GameObject quantityDialogPrefab;
    public MoneyManager moneyManager;
    private GameObject quantityDialogInstance;
    private InventorySlotData selectedSlot;
    public int selectedQuantity = 1; // 기존 private → public 으로 변경


    // 슬롯 클릭 시 호출
    public void OnSlotClicked(InventorySlotData slot)
    {
        if (slot.currentItem == null) return;

        selectedSlot = slot; // 클릭된 슬롯 저장
        selectedQuantity = 1; // 수량은 항상 1부터 시작작
        ShowQuantityDialog(slot.currentItem, slot.currentItemCount); 
        // 수량 선택 다이얼로그를 화면에 띄우기 (slot.currentitem : 어떤 아이템인지, slot.currentItemCount : 최대 선택 가능한 수량량)
    }

    // 수량 선택 다이얼로그 표시
    private void ShowQuantityDialog(ItemData item, int maxQuantity)
    {
        if (quantityDialogInstance != null)
            Destroy(quantityDialogInstance); // 이미 UI가 화면에 떠 있다면 기존 다이얼로그 먼저 제거 (중복으로 여러 개 뜨는 것을 방지)

        quantityDialogInstance = Instantiate(quantityDialogPrefab); // 새로운 다이얼로그 UI 생성
        quantityDialogInstance.GetComponent<QuantityDialog>().Setup(this, item, maxQuantity); // this : SaleSystem의 인스턴스
        // 다이얼로그 UI에 아이템 정보, 수량, +/-, 판매 버튼 연결
        // 예: quantityDialogInstance.GetComponent<QuantityDialogUI>().Setup(this, item, maxQuantity);
    }

    // + 버튼
    public void IncreaseQuantity(int maxQuantity)
    {
        if (selectedQuantity < maxQuantity)
            selectedQuantity++;
        // UI 갱신
    }

    // - 버튼
    public void DecreaseQuantity()
    {
        if (selectedQuantity > 1)
            selectedQuantity--;
        // UI 갱신
    }

    // 판매 버튼
    public void OnSellButtonClicked()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null) return;

        int sellValue = selectedSlot.currentItem.value * selectedQuantity;
        moneyManager.AddMoney(sellValue);

        // 인벤토리에서 해당 수량만큼 제거
        selectedSlot.RemoveItem(selectedQuantity);

        // 다이얼로그 닫기
        Destroy(quantityDialogInstance);
        selectedSlot = null;
    }
}