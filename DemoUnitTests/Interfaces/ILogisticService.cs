namespace DemoUnitTests.API.Interfaces
{
    public interface ILogisticService
    {
        bool PrepareOrder(int orderId);
        bool ValideOrder(int orderId);
    }
}
