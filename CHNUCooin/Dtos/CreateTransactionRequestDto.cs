namespace CHNUCooin.Dtos
{
    [Serializable]
    public class CreateTransactionRequestDto
    {
        public string PrivateKey { get; set; }
        public string Address { get; set; }
        public double Value { get; set; }
    }
}
