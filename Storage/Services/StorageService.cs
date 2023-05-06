using Storage.Models;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace Storage.Services;
public class StorageService
{
    public static Dictionary<Guid, FileModel> FileStorage { get; set; } = new();
    public FileModel AddFile(IFormFile file, int expiresIn)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Padding = PaddingMode.PKCS7;

            using (MemoryStream inputMemoryStream = new MemoryStream())
            using (MemoryStream outputMemoryStream = new MemoryStream())
            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
            using (CryptoStream csEncrypt = new CryptoStream(outputMemoryStream, encryptor, CryptoStreamMode.Write))
            {
                // Copy the contents of the input file to the input memory stream
                file.CopyTo(inputMemoryStream);
                inputMemoryStream.Position = 0;

                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = inputMemoryStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    csEncrypt.Write(buffer, 0, bytesRead);
                }
                csEncrypt.FlushFinalBlock();

                var fileModel = new FileModel(outputMemoryStream.ToArray(), expiresIn, file.FileName, aesAlg.Key, aesAlg.IV);
                FileStorage[fileModel.FileId] = fileModel;

                return fileModel;
            }
        }
    }
    public FileResponse DecryptFile(Guid fileId)
    {
        var fileModel = FileStorage[fileId];
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = fileModel.Key;
            aesAlg.IV = fileModel.IV;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (MemoryStream inputStream =  new MemoryStream(fileModel.FileContent))
            using (MemoryStream outputMemoryStream = new MemoryStream())
            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
            using (CryptoStream csDecrypt = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputMemoryStream.Write(buffer, 0, bytesRead);
                }

                return new FileResponse(outputMemoryStream.ToArray(), fileModel.Name);
            }
        }
    }
    public FileResponse GetFile(Guid fileId)
    {
        if (!FileStorage.ContainsKey(fileId))
        {
            return null;
        }
        return DecryptFile(fileId);
    }
    public bool DeleteFile(Guid fileId)
    {
        return FileStorage.Remove(fileId);
    }
}

