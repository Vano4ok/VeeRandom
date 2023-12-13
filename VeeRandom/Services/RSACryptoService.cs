using System.Security.Cryptography;

namespace VeeRandom.Server.Services;

public sealed class RSACryptoService
{
    private const int ChunkSize = 117;

    public async Task<(string publicKey, string privateKey)> GenerateKeysAsync()
    {
        try
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
                string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                return (publicKey, privateKey);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating keys: {ex.Message}");
            throw;
        }
    }

    public async Task<byte[]> EncryptAsync(byte[] data, string publicKey)
    {
        try
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

                List<byte> encryptedData = new List<byte>();

                for (int i = 0; i < data.Length; i += ChunkSize)
                {
                    int chunkSize = Math.Min(ChunkSize, data.Length - i);
                    byte[] chunk = data.Skip(i).Take(chunkSize).ToArray();
                    encryptedData.AddRange(rsa.Encrypt(chunk, RSAEncryptionPadding.Pkcs1));
                }

                return encryptedData.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error encrypting data: {ex.Message}");
            throw;
        }
    }

    public async Task<byte[]> DecryptAsync(byte[] data, string privateKey)
    {
        try
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                List<byte> decryptedData = new List<byte>();

                for (int i = 0; i < data.Length; i += rsa.KeySize / 8)
                {
                    int chunkSize = Math.Min(rsa.KeySize / 8, data.Length - i);
                    byte[] chunk = data.Skip(i).Take(chunkSize).ToArray();
                    decryptedData.AddRange(rsa.Decrypt(chunk, RSAEncryptionPadding.Pkcs1));
                }

                return decryptedData.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error decrypting data: {ex.Message}");
            throw;
        }
    }
}
