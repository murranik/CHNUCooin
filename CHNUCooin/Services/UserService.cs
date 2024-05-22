using CHNUCooin.Database;
using CHNUCooin.Database.Models;
using CHNUCooin.Dtos;
using System.Security.Cryptography;
using System.Text;

namespace CHNUCooin.Services
{
    public class UserService(ApplicationContext applicationContext)
    {
        private readonly ApplicationContext _applicationContext = applicationContext;

        public async Task<CreateUserResponseDto> CreateUser()
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.PersistKeyInCsp = false;
            var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

            var hashedPublicKey = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(publicKey)));

            var user = new User
            {
                IsMiner = 0,
                PublicKey = publicKey,
                HashedPublicKey = hashedPublicKey
            };

            _applicationContext.Users.Add(user);
            await _applicationContext.SaveChangesAsync();

            var userPrivate = new UserPrivate
            {
                UserId = user.Id,
                PrivateKey = privateKey
            };

            _applicationContext.UserPrivates.Add(userPrivate);

            var wallet = new Wallet
            {
                UserId = user.Id,
                Value = 10.0
            };

            _applicationContext.Wallets.Add(wallet);

            await _applicationContext.SaveChangesAsync();

            return new CreateUserResponseDto
            {
                Address = user.HashedPublicKey,
                PrivateKey = privateKey,
                PublicKey = publicKey
            };
        }

        public List<string> GetListOfAddresses()
        {
            return [.. _applicationContext.Users.Select(user => user.HashedPublicKey)];
        }
    }
}
