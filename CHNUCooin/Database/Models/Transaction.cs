namespace CHNUCooin.Database.Models
{
    [Serializable]
    public class Transaction
    {
        public int Id { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Time { get; set; }
        public double Sum { get; set; }
        public string Hash { get; set; }
        public long Nonce { get; set; }
        public int Approved { get; set; }
        public string SenderSign { get; set; }
        public string? MinerSign { get; set; }
        public string? MinedTime { get; set; }
        public int Processed { get; set; }

        public override string ToString()
        {
            return "Transaction{" +
                   "Id='" + Id + '\'' +
                   ", FromAddress='" + FromAddress + '\'' +
                   ", ToAddress='" + ToAddress + '\'' +
                   ", Time=" + Time +
                   ", Sum=" + Sum +
                   ", Hash='" + Hash + '\'' +
                   ", Nonce=" + Nonce +
                   ", Approved=" + Approved +
                   ", SenderSign='" + SenderSign + '\'' +
                   ", MinerSign='" + MinerSign + '\'' +
                   ", MinedTime=" + MinedTime +
                   ", Processed=" + Processed +
                   '}';
        }
    }
}
