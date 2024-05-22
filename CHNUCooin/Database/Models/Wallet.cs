namespace CHNUCooin.Database.Models
{
    [Serializable]
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Value { get; set; }
    }
}
