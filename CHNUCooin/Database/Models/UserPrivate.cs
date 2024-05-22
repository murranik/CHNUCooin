using System.ComponentModel.DataAnnotations.Schema;

namespace CHNUCooin.Database.Models
{
    [Serializable]
    public class UserPrivate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PrivateKey { get; set; }
    }
}
