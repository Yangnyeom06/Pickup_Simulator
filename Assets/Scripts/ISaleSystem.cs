public interface ISaleSystem
{
    
    void OnSlotClicked(InventorySlotData slot);
    void ShowSellUI();
    void CancelSell();

    bool CanSell(ItemData item);
}