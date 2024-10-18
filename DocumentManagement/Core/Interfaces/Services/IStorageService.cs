namespace DocumentManagement.Core.Interfaces.Services;

public interface IStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, bool encrypt);
    Task<string> CopyFileAsync(string sourceFilePath, string fileName);
    Task<Stream> GetFileAsync(string filePath, bool dycrypt);
    Task DeleteFileAsync(string filePath);
}
