namespace DemoUnitTests.API.Models
{
    public class Order
    {
        public class OrderLine
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public int Id { get; set; }
        public List<OrderLine> Lines { get; set; } = [];
        public bool IsReadyForShipping { get; set; }


        // Des infos liées au client y serait également présent...
    }
}
