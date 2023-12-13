using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeeRandom.MD5;

namespace VeeRandom.Tests;

public class VeeMD5Test
{
    [Fact]
    public void TestHashBytes()
    {
        // Arrange
        string input = "Hello, World!";
        byte[] inputData = Encoding.UTF8.GetBytes(input);

        // Act
        byte[] hashBytes = VeeMD5.HashBytes(inputData);

        // Assert
        Assert.NotNull(hashBytes);
        Assert.Equal(16, hashBytes.Length);
    }

    [Fact]
    public void TestHashBytesWithEmptyData()
    {
        // Arrange
        byte[] inputData = new byte[0];

        // Act
        byte[] hashBytes = VeeMD5.HashBytes(inputData);

        // Assert
        Assert.NotNull(hashBytes);
        Assert.Equal(16, hashBytes.Length);
    }

    [Fact]
    public void TestHashBytesWithNullData()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => VeeMD5.HashBytes(null));
    }

    [Fact]
    public void TestHashBytesConsistency()
    {
        // Arrange
        string input = "Hello, World!";
        byte[] inputData = Encoding.UTF8.GetBytes(input);

        // Act
        byte[] hashBytes1 = VeeMD5.HashBytes(inputData);
        byte[] hashBytes2 = VeeMD5.HashBytes(inputData);

        // Assert
        Assert.NotNull(hashBytes1);
        Assert.NotNull(hashBytes2);
        Assert.Equal(hashBytes1, hashBytes2);
    }
}
