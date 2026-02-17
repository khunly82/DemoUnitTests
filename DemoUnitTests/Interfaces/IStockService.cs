namespace DemoUnitTests.API.Interfaces
{
    public interface IStockService
    {
        int GetStock(int productId);
        void RemoveStock(int productId, int quantity);
    }
}
