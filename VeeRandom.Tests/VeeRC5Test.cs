using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeeRandom.RC5;
using VeeRandomGenerator;

namespace VeeRandom.Tests;

public class VeeRC5Test
{
    [Fact]
    public void Constructor_InvalidKeySize_ThrowsArgumentException()
    {
        // Arrange
        byte[] keyWithInvalidSize = new byte[15];
        Mock<IGenerator> generatorMock = new Mock<IGenerator>();

        // Act and Assert
        Assert.Throws<ArgumentException>(() => new VeeRC5(keyWithInvalidSize, generatorMock.Object));
    }

    [Fact]
    public void Decrypt_InvalidDataSize_ThrowsArgumentException()
    {
        // Arrange
        byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        Mock<IGenerator> generatorMock = new Mock<IGenerator>();
        VeeRC5 veeRC5 = new VeeRC5(key, generatorMock.Object);

        // Act and Assert
        byte[] dataWithInvalidSize = new byte[7];
        Assert.Throws<ArgumentException>(() => veeRC5.Decrypt(dataWithInvalidSize));
    }

    [Fact]
    public void EncryptDecrypt_EmptyData_Success()
    {
        // Arrange
        byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        Mock<IGenerator> generatorMock = new Mock<IGenerator>();
        VeeRC5 veeRC5 = new VeeRC5(key, generatorMock.Object);

        // Act
        byte[] emptyData = new byte[0];
        byte[] encryptedData = veeRC5.Encrypt(emptyData);
        byte[] decryptedData = veeRC5.Decrypt(encryptedData);

        // Assert
        Assert.Equal(emptyData, decryptedData);
    }

    [Fact]
    public void EncryptDecrypt_BlockSizeData_Success()
    {
        // Arrange
        byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
        Mock<IGenerator> generatorMock = new Mock<IGenerator>();
        VeeRC5 veeRC5 = new VeeRC5(key, generatorMock.Object);

        // Act
        byte[] blockData = new byte[8];
        byte[] encryptedData = veeRC5.Encrypt(blockData);
        byte[] decryptedData = veeRC5.Decrypt(encryptedData);

        // Assert
        Assert.Equal(blockData, decryptedData);
    }
}