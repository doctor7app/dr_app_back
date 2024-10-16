﻿namespace DocumentManagement.Core.Interfaces;

public interface IStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, bool encrypt);
    Task<Stream> GetFileAsync(string filePath, bool dycrypt);
    Task DeleteFileAsync(string filePath);
}