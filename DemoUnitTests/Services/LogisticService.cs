using DemoUnitTests.API.Interfaces;
using DemoUnitTests.API.Models;

namespace DemoUnitTests.API.Services
{
    public class LogisticService : ILogisticService
    {
        private readonly IOrderService _orderService;
        private readonly IStockService _stockService;

        public LogisticService(IOrderService orderService, IStockService stockService)
        {
            _orderService = orderService;
            _stockService = stockService;
        }

        private Order GetOrderFromService(int orderId)
        {
            try
            {
                return _orderService.GetById(orderId);
            }
            catch (ArgumentException error)
            {
                throw new ArgumentOutOfRangeException("La commande n'a pas été trouvé !", error);
            }
        }

        public bool PrepareOrder(int orderId)
        {
            Order order = GetOrderFromService(orderId);

            try
            {
                for (int p = 0; p < order.Lines.Count(); p++)
                {
                    Order.OrderLine line = order.Lines[p];
                    int stock = _stockService.GetStock(line.ProductId);

                    if (line.Quantity > stock)
                    {
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new InvalidDataException("Un produit n'a pas été trouvé dans le stock !");
            }

            return true;
        }

        public bool ValideOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
