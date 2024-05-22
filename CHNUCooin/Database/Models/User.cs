namespace CHNUCooin.Database.Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public int IsMiner { get; set; }
        public string HashedPublicKey { get; set; }
        public string PublicKey { get; set; }
    }
}
