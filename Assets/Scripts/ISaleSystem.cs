public interface ISaleSystem
{
    // int selectedQuantity { get; set; }
// 
    // void IncreaseQuantity(int maxQty);
    // void DecreaseQuantity();
    void ConfirmSell();
    
    void OnSlotClicked(InventorySlotData slot);
    // void RefreshSellSlots();
    void ShowSellUI();
    void CancelSell();

    bool CanSell(ItemData item);
}