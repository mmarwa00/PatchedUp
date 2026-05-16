public interface IPlaceable {
    bool CanPlace(PickableItem item);
    void Place(PickableItem item);
}