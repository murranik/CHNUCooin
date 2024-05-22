namespace CHNUCooin.Dtos
{
    [Serializable]
    public class CreateUserResponseDto
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Address { get; set; }
    }
}
