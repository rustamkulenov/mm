namespace mm.Execution
{
    internal sealed class Order
    {
        public string OrderId { get; }
        public string ClOrderId { get; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
    }
}