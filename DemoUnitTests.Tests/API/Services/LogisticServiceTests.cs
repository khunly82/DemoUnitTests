using DemoUnitTests.API.Interfaces;
using DemoUnitTests.API.Models;
using DemoUnitTests.API.Services;
using Moq;

namespace DemoUnitTests.Tests.API.Services
{
    #region Fake
    internal class FakeOrderService : IOrderService
    {
        public Order CreateNewOrder()
        {
            throw new NotImplementedException();
        }

        public Order GetById(int orderId)
        {
            if (orderId != 42)
                throw new ArgumentException("Bad Order !");

            return new Order()
            {
                Id = 42,
                IsReadyForShipping = false,
                Lines = [
                    new Order.OrderLine() { ProductId = 1, Quantity = 10 },
                    new Order.OrderLine() { ProductId = 3, Quantity = 2 },
                    new Order.OrderLine() { ProductId = 4, Quantity = 1 },
                ]
            };
        }
    }

    internal class FakeStockService_With10UnitByProduct : IStockService
    {
        public int GetStock(int productId)
        {
            return 10;
        }

        public void RemoveStock(int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeStockService_OnlyOneProductInStock : IStockService
    {
        public int GetStock(int productId)
        {
            return (productId == 1) ? 100 : 0;
        }

        public void RemoveStock(int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    public class LogisticServiceTests
    {
        // Exemple avec des dépendences simulée
        [Fact]
        public void PrepareOrder_ProductOrderedInStock_ReturnTrue()
        {
            // Simulation des dépendences
            IOrderService stubOrderService = new FakeOrderService();
            IStockService stubStockService = new FakeStockService_With10UnitByProduct();

            // Arrange
            ILogisticService logisticService = new LogisticService(stubOrderService, stubStockService);
            int orderId = 42;

            // Act
            bool result = logisticService.PrepareOrder(orderId);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void PrepareOrder_ProductOrderedNotInStock_ReturnFalse()
        {
            // Simulation des dépendences
            IOrderService stubOrderService = new FakeOrderService();
            IStockService stubStockService = new FakeStockService_OnlyOneProductInStock();

            // Arrange
            ILogisticService logisticService = new LogisticService(stubOrderService, stubStockService);
            int orderId = 42;

            // Act
            bool result = logisticService.PrepareOrder(orderId);

            //Assert
            Assert.False(result);
        }


        // Exemple avec des dépendences simulée via le package « Moq »
        [Fact]
        public void PrepareOrder_ProductOrderedInStock_AllProductStockIsChecked()
        {
            // Mocking
            IOrderService stubOrderService = new FakeOrderService();
            Mock<IStockService> mockStockService = new Mock<IStockService>();
            mockStockService.Setup(service => service.GetStock(It.IsAny<int>())).Returns(1_000);
            // → It.IsAny<int>() : N'import quelle entier

            // Arrange
            ILogisticService logisticService = new LogisticService(stubOrderService, mockStockService.Object);

            // Act
            logisticService.PrepareOrder(42);

            // Assert
            mockStockService.Verify(service => service.GetStock(It.IsAny<int>()), Times.Exactly(3));
            // → L'order 42 obtenir via le stubOrderService, contient 3 produits !
        }

        // Exemple de tests avec exceptions
        [Fact]
        public void PrepareOrder_OneProductIsNotFound_ThrowsInvalidDataException()
        {
            int orderId = 42;
            int productIdNotFound = 3;
            int productQuantity = 1_000;
            string expected_error_msg = "Un produit n'a pas été trouvé dans le stock !";

            // Mocking
            IOrderService stubOrderService = new FakeOrderService();

            Mock<IStockService> stubStockService = new Mock<IStockService>();
            stubStockService.Setup(service => service.GetStock(It.IsNotIn<int>(productIdNotFound))).Returns(productQuantity);
            stubStockService.Setup(service => service.GetStock(productIdNotFound)).Throws<ArgumentException>();
            // → Le produit 3 n'existe pas dans la "db" du stock

            // Arrange
            ILogisticService logisticService = new LogisticService(stubOrderService, stubStockService.Object);

            // Assert + Act
            var error = Assert.Throws<InvalidDataException>(() => {
                logisticService.PrepareOrder(orderId);
            });
            Assert.Equal(expected_error_msg, error.Message);
        }

        [Fact]
        public void PrepareOrder_OrderIsNotFound_ThrowsArgumentOutOfRangeException()
        {
            int orderId = 5;
            string expected_error_msg = "La commande n'a pas été trouvé !";

            // Mocking
            // -> Fake de "IOrderService" avec un order not found en exception
            Mock<IOrderService> stubOrderService = new Mock<IOrderService>();
            stubOrderService.Setup(service => service.GetById(orderId)).Throws<ArgumentException>();
            // -> Fake de "IStockService" sans config (en a juste besoin pour le ctor)
            Mock<IStockService> stubStockService = new Mock<IStockService>();

            // Arrange
            ILogisticService logisticService = new LogisticService(stubOrderService.Object, stubStockService.Object);

            //Act and Assert
            var error = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                logisticService.PrepareOrder(orderId);
            });
            Assert.Equal(expected_error_msg, error.Message);
        }
    }
}
