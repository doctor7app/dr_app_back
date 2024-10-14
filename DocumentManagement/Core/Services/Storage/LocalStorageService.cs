using DocumentManagement.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace DocumentManagement.Core.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly bool _encryptFiles;

        public LocalStorageService(IConfiguration configuration)
        {
            _basePath = configuration["StorageSettings:BasePath"];
            _encryptFiles = bool.Parse(configuration["StorageSettings:EncryptFiles"]);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, bool encrypt)
        {
            var filePath = Path.Combine(_basePath, Guid.NewGuid().ToString() + Path.GetExtension(fileName));

            // Optionally encrypt the file
            using (var file = new FileStream(filePath, FileMode.Create))
            {
                if (encrypt || _encryptFiles)
                {
                    using (var cryptoStream = new CryptoStream(file, CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        await fileStream.CopyToAsync(cryptoStream);
                    }
                }
                else
                {
                    await fileStream.CopyToAsync(file);
                }
            }

            return filePath;
        }

        public async Task<Stream> GetFileAsync(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return fileStream; // Decryption can be added here as needed.
        }

        public Task DeleteFileAsync(string filePath)
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        private ICryptoTransform CreateEncryptor()
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("YourSecretEncryp--112233"); // Simplified; manage key securely
                aes.IV = new byte[16]; // Use a secure IV management
                return aes.CreateEncryptor();
            }
        }
    }
}
