using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeeRandom.Server.Services;

namespace VeeRandom.Tests;

public class RSACryptoServiceTest
{
    [Fact]
    public async Task GenerateKeysAsync_ShouldGenerateValidKeys()
    {
        // Arrange
        var cryptoService = new RSACryptoService();

        // Act
        var keys = await cryptoService.GenerateKeysAsync();

        // Assert
        Assert.NotNull(keys.publicKey);
        Assert.NotNull(keys.privateKey);
    }

    [Fact]
    public async Task EncryptAndDecrypt_ShouldProduceOriginalData()
    {
        // Arrange
        var cryptoService = new RSACryptoService();
        var originalData = Encoding.UTF8.GetBytes("Hello, RSA!");

        // Act
        var keys = await cryptoService.GenerateKeysAsync();
        var encryptedData = await cryptoService.EncryptAsync(originalData, keys.publicKey);
        var decryptedData = await cryptoService.DecryptAsync(encryptedData, keys.privateKey);
        var decryptedString = Encoding.UTF8.GetString(decryptedData);

        // Assert
        Assert.Equal(originalData, decryptedData);
        Assert.Equal("Hello, RSA!", decryptedString);
    }
}