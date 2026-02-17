using DemoUnitTests.API.Models;

namespace DemoUnitTests.API.Interfaces
{
    public interface IOrderService
    {
        Order GetById(int orderId);
        Order CreateNewOrder();
    }
}
