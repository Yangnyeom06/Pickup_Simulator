public interface ISaleSystem
{
    int selectedQuantity { get; set; }
    void IncreaseQuantity(int maxQty);
    void DecreaseQuantity();
    void ConfirmSell();
}