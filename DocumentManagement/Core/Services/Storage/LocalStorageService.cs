using DocumentManagement.Core.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace DocumentManagement.Core.Services.Storage;

public class LocalStorageService(IConfiguration configuration) : IStorageService
{
    private readonly string _basePath = configuration["StorageSettings:BasePath"];

    private readonly string _key = "YourSecretEncryp--112233";

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, bool encrypt)
    {
        // Generate the file path
        string filePath = GenerateFilePath() + Path.GetExtension(fileName);

        // Ensure that the directory structure exists
        CreateDirectoryIfNotExists(filePath);

        // Optionally encrypt the file
        using (var file = new FileStream(filePath, FileMode.Create))
        {
            if (encrypt)
            {
                using var cryptoStream = new CryptoStream(file, CreateEncryptor(), CryptoStreamMode.Write);
                await fileStream.CopyToAsync(cryptoStream);
            }
            else
            {
                await fileStream.CopyToAsync(file);
            }
        }

        return filePath;
    }

    public async Task<string> CopyFileAsync(string sourceFilePath, string fileName)
    {
        // Generate the file path
        string destinationFilePath = GenerateFilePath() + Path.GetExtension(fileName);

        // Ensure that the directory structure exists
        CreateDirectoryIfNotExists(destinationFilePath);

        using Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using Stream destination = File.Create(destinationFilePath);
        await source.CopyToAsync(destination);

        return destinationFilePath;
    }

    public async Task<Stream> GetFileAsync(string filePath, bool dycrypt)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        Console.WriteLine(dycrypt);
        if (dycrypt)
        {
            var resultStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(fileStream, CreateDecryptor(), CryptoStreamMode.Read))
            {
                await cryptoStream.CopyToAsync(resultStream);
            }
            resultStream.Position = 0;
            return resultStream;

        }
        return fileStream;
    }

    public Task DeleteFileAsync(string filePath)
    {
        File.Delete(filePath);
        return Task.CompletedTask;
    }

    private ICryptoTransform CreateEncryptor()
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_key);
        aes.IV = new byte[16];
        return aes.CreateEncryptor();
    }

    private ICryptoTransform CreateDecryptor()
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_key);
        aes.IV = new byte[16];
        return aes.CreateDecryptor();
    }

    private string GenerateFilePath()
    {
        string fileId = Guid.NewGuid().ToString();
        List<string> dirs = [.. fileId.Split('-')];
        dirs.RemoveAt(dirs.Count - 1);

        // Combine to form the file path structure: /12d84ce5/ab05/4c41//4c41//9b6c/12d84ce5-ab05-4c41-9b6c-5af331e9ecc4.ext
        return Path.Combine(_basePath, string.Join(Path.DirectorySeparatorChar, [.. dirs]), $"{fileId}");
    }

    private static void CreateDirectoryIfNotExists(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
