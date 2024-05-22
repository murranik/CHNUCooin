using CHNUCooin.Database;
using CHNUCooin.Database.Models;
using CHNUCooin.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CHNUCooin.Services
{
    public class TransactionService(ApplicationContext applicationContext)
    {
        private readonly ApplicationContext _applicationContext = applicationContext;

        public async Task CreateTransaction(CreateTransactionRequestDto createTransactionRequest, CancellationToken cancellationToken)
        {
            var privateKey = createTransactionRequest.PrivateKey;
            var userPrivate = await _applicationContext.UserPrivates.FirstOrDefaultAsync(u => u.PrivateKey == privateKey, cancellationToken);

            if (userPrivate == null)
            {
                throw new Exception("User Not Found");
            }

            var user = await _applicationContext.Users.FirstOrDefaultAsync(u => u.Id == userPrivate.UserId, cancellationToken);

            if (user == null)
            {
                throw new Exception("User Not Found");
            }

             var lastTransaction = await _applicationContext.Transactions.OrderByDescending(t => t.Time).FirstOrDefaultAsync(cancellationToken);

            var transaction = new Transaction()
            {
                Approved = 0,
                Processed = 0,
                FromAddress = user.HashedPublicKey,
                ToAddress = createTransactionRequest.Address,
                Time = DateTime.Now.ToString(),
                Sum = createTransactionRequest.Value,
            };

            string transactionHash = HashSHA256(transaction.ToString());
            if (lastTransaction != null)
            {
                transactionHash = HashSHA256(lastTransaction.Hash + transactionHash);
            }
            transaction.Hash = transactionHash;

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(transactionHash), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                transaction.SenderSign = Convert.ToBase64String(signature);
            }

            await _applicationContext.Transactions.AddAsync(transaction, cancellationToken);
            await _applicationContext.SaveChangesAsync(cancellationToken);

            var list = await _applicationContext.Transactions.ToListAsync(cancellationToken);
        }

        private string HashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
