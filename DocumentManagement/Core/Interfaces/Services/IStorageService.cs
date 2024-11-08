using CSharpFunctionalExtensions;

namespace DocumentManagement.Core.Interfaces.Services;

public interface IStorageService
{
    Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName, bool encrypt);
    Task<Result<string>> CopyFileAsync(string sourceFilePath, string fileName);
    Task<Result<Stream>> GetFileAsync(string filePath, bool dycrypt);
    Result<bool> DeleteFileAsync(string filePath);
}
