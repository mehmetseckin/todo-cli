using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Todo.CLI.Auth;

static class TokenCacheHelper
{
    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".todo-cli"
    );
    private static readonly string KeyFilePath = Path.Combine(ConfigDir, "key.bin");
    private static readonly string CacheFilePath = Path.Combine(ConfigDir, "token.cache");
    private static readonly object FileLock = new object();

    private static byte[] GetOrCreateKey()
    {
        if (!Directory.Exists(ConfigDir))
        {
            Directory.CreateDirectory(ConfigDir);
        }

        if (File.Exists(KeyFilePath))
        {
            return File.ReadAllBytes(KeyFilePath);
        }

        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        File.WriteAllBytes(KeyFilePath, key);
        return key;
    }

    public static void EnableSerialization(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }

    private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
    {
        lock (FileLock)
        {
            if (File.Exists(CacheFilePath))
            {
                var encryptedData = File.ReadAllBytes(CacheFilePath);
                var nonce = new byte[12];
                var tag = new byte[16];
                var ciphertext = new byte[encryptedData.Length - 28];
                Array.Copy(encryptedData, nonce, 12);
                Array.Copy(encryptedData, 12, tag, 0, 16);
                Array.Copy(encryptedData, 28, ciphertext, 0, ciphertext.Length);

                using var aes = new AesGcm(GetOrCreateKey(), 16);
                var plaintext = new byte[ciphertext.Length];
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
                args.TokenCache.DeserializeMsalV3(plaintext);
            }
        }
    }

    private static void AfterAccessNotification(TokenCacheNotificationArgs args)
    {
        if (args.HasStateChanged)
        {
            lock (FileLock)
            {
                if (!Directory.Exists(ConfigDir))
                {
                    Directory.CreateDirectory(ConfigDir);
                }

                var plaintext = args.TokenCache.SerializeMsalV3();
                using var aes = new AesGcm(GetOrCreateKey(), 16);
                var nonce = new byte[12];
                RandomNumberGenerator.Fill(nonce);
                var ciphertext = new byte[plaintext.Length];
                var tag = new byte[16];
                aes.Encrypt(nonce, plaintext, ciphertext, tag);

                var encryptedData = new byte[nonce.Length + tag.Length + ciphertext.Length];
                Array.Copy(nonce, encryptedData, nonce.Length);
                Array.Copy(tag, 0, encryptedData, nonce.Length, tag.Length);
                Array.Copy(ciphertext, 0, encryptedData, nonce.Length + tag.Length, ciphertext.Length);
                File.WriteAllBytes(CacheFilePath, encryptedData);
            }
        }
    }
}