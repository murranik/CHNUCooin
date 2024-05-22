using CHNUCooin.Database;
using System.Security.Cryptography;
using System.Text;

namespace CHNUCooin.Services
{
    public class TransactionProcessingService(ApplicationContext applicationContext)
    {
        private readonly ApplicationContext _applicationContext = applicationContext;

        public async void ProcessTransactions()
        {
            var transaction = _applicationContext.Transactions.FirstOrDefault(t => t.Approved == 0);
            var notProcessedTransaction = _applicationContext.Transactions.FirstOrDefault(t => t.Processed == 0 && t.Approved == 1);

            if (notProcessedTransaction != null)
            {
                var wallets = _applicationContext.Wallets.ToList();
                var minerWallet = wallets.FirstOrDefault(w => w.UserId == 1); // Assuming miner's wallet ID is known
                if (minerWallet != null)
                {
                    minerWallet.Value += 0.1;
                    _applicationContext.Wallets.Update(minerWallet);
                }

                await _applicationContext.SaveChangesAsync();

                var users = _applicationContext.Users.ToList();
                var addressToUser = users.ToDictionary(u => u.HashedPublicKey, u => u);

                var userIdToWallet = wallets.ToDictionary(w => w.UserId, w => w);

                var senderWallet = userIdToWallet[addressToUser[notProcessedTransaction.FromAddress].Id];
                senderWallet.Value -= notProcessedTransaction.Sum;
                _applicationContext.Wallets.Update(senderWallet);

                var receiverWallet = userIdToWallet[addressToUser[notProcessedTransaction.ToAddress].Id];
                receiverWallet.Value += notProcessedTransaction.Sum;
                _applicationContext.Wallets.Update(receiverWallet);

                notProcessedTransaction.Processed = 1;
                _applicationContext.Transactions.Update(notProcessedTransaction);
                await _applicationContext.SaveChangesAsync();
            }

            if (transaction != null)
            {
                long nonce = 0;
                while (true)
                {
                    string hash = HashSHA256(transaction.Hash + nonce.ToString());
                    if (hash.StartsWith("0"))
                    {
                        transaction.Nonce = nonce;
                        transaction.Hash = hash;

                        byte[] keyBytes = Convert.FromHexString("308204bf020100300d06092a864886f70d0101010500048204a9308204a50201000282010100a0e9a895f72bb569ba6b22fa95fd5f98b4432a47ceb869b1fd21cfa7cf39843cc2d1b27cd861559176ba69ba20bb23f0c3b86fc8577d56c9ec4dc863ce5c65a74fb4bd77f0c15ea722c2dc2354aa2ae8193cd16841d9564112d6737e0158b4592e0698fc736ec6468ce2788400b79aeb26e9b9cd0a08030518593664c57ea8fbe63a9dcf74d7aabce2225f85b92514118f2d2dc9ad96ee6a98d4611e05b4653435695bce5c1ba74e5d975d6c00f583c87225d41c5cffd335a64472875b998167124d7bd343ebbf5ee611545423990bb82fd82bb4c71a73a3a21f1d5eab7bc5fc89dedd2a1c0dd96c237c253bd6290fcb85341bfff7b3330ec510895417266863020301000102820100692362d0e1d840479dd384de608456230b9d5b11e22322847695d1fd9e6bc158861ec7d5c4f80c15e892b2a437e0af0210900111d1008765962dd88246029f34ebe2776dd7f430e959d022503f70946a649b15645eaa282e8aa56e7ee558553673d941baa99db695c055f552a2b954d2d2f12242b38c3a94e8acd9da8c244f3a80da138ae81c8c985e364c9a687cf154aeefcbe5a6b562669af61ba7af6e88a57ecd0386aad35b9c047d715ff0eb7d08967bac9b238d285c814b4e8685c6209cf010b9bf3feecf974c6c5bf90109621f10a719ac3098d275708cab42924aa7915fcbc18d4a99d47419becdfc8f510158fd90403556a5371be657811f0e953f1102818100f9f503815e4a3df77b6fef30a83e6a85f16c8b8a324d2c507417df6a94c9c9bebef92be1852f8af94e295ee405b723e8cba27dd0714b8afe4845d61f5662a838e1bfcf417fc9bd1b80876946553078f27763a4f2b9af5bdc245f2a66b772f897e4ba5ec25030076f4302c2b763a98028e6b93f7229b85244c9fcec77505260cb02818100a4cd8c7407680fd1df8a37a72f371bbf1cc29b8e350ac05fdaec4e59124a166149f7d10fecad6e48162ba72dc8ae9539650c5add7b338e66002424ef7d12207a6766aa9428ceee2228230bc0e371aef52c1809fb4b32e3cd3c835c01d693dc338f79e48d3b65f8b135162ff029ef745067acba80e8fe32928d7467e589691bc902818100a2385e77065fbe89b045bc01631f66f8d8ef6e3c595af6889c5fa5f2c0ef990c3f5aed1ca6d86d245192cdefc4b9d00754a97767e12f7adc7ec13c2f085c6cfac5a03b7d31d716f113604e7583f23a7c79f96462d719b4cf5197a5a81624faab719976c83b4c8076b811c1974d8a031008756f74c318c9e3b5077ddc0d77f9330281810091f922fc2f074463f02d9c27f0914e7953ff280ba8fdd1f1d596d3badf91520b709a19ca9227f7f3cff7708804266734c4455242b57ab019b6ec4ceef808534e38425ebb06fe6b3d43f1eeda365b9ecdb8f5bdf9a0dcfd9bef8aa6b75619f7c48fc436e5c6040768e70db4fe74168e1f1e0587d8e1072f42521b165fe0fb81c902818100d54eddec066fdbfd1dcc6792a6c16bb8a85b7fca2605be34a50e17380d7bf6f1a77e91e28db76f504b6a69d646c4071370e5df54007858f235755ae98031496cc9bdc98c0fcf9ccc7b3b1e5785a1d55cb2ce904a3ce7318d241dacd544b4aed523fc8206f9f8bae48ef7679566fefb09d532042a7dbef61e8d10806536e6e014");

                        // Create RSA instance
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                        // Import the key
                        rsa.ImportPkcs8PrivateKey(keyBytes, out _);

                        byte[] signatureBytes = rsa.SignData(Encoding.UTF8.GetBytes(hash), new SHA256CryptoServiceProvider());

                        transaction.Approved = 1;
                        transaction.MinerSign = BitConverter.ToString(signatureBytes).Replace("-", "");
                        transaction.MinedTime = DateTime.Now.ToString();
                        _applicationContext.Transactions.Update(transaction);
                        await _applicationContext.SaveChangesAsync();

                        Console.WriteLine(nonce);
                        rsa.Dispose();
                        break;
                    }
                    else
                    {
                        nonce++;
                        Console.WriteLine(nonce);
                    }
                }
            }
        }

        private string HashSHA256(string input)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
