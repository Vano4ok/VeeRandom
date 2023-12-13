using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeeRandom.Server.Services;

namespace VeeRandom.Tests;

public class DigitalSignatureServiceTests
{
    [Fact]
    public void SignDataAndVerifyData_ShouldSucceed()
    {
        // Arrange
        var digitalSignatureService = new DigitalSignatureService();
        var originalData = Encoding.UTF8.GetBytes("Hello, Digital Signature!");

        // Act
        var signature = digitalSignatureService.SignData(originalData);
        var isVerified = digitalSignatureService.VerifyData(originalData, signature);

        // Assert
        Assert.True(isVerified);
    }

    [Fact]
    public void VerifyData_WithModifiedData_ShouldFail()
    {
        // Arrange
        var digitalSignatureService = new DigitalSignatureService();
        var originalData = Encoding.UTF8.GetBytes("Hello, Digital Signature!");

        // Act
        var signature = digitalSignatureService.SignData(originalData);

        // Modify the data
        originalData[0] = (byte)('H' + 1);

        var isVerified = digitalSignatureService.VerifyData(originalData, signature);

        // Assert
        Assert.False(isVerified);
    }

    [Fact]
    public void VerifyData_WithInvalidSignature_ShouldFail()
    {
        // Arrange
        var digitalSignatureService = new DigitalSignatureService();
        var originalData = Encoding.UTF8.GetBytes("Hello, Digital Signature!");
        var invalidSignature = new byte[] { /* invalid signature data */ };

        // Act
        var isVerified = digitalSignatureService.VerifyData(originalData, invalidSignature);

        // Assert
        Assert.False(isVerified);
    }

    [Fact]
    public void SignDataAndVerifyData_WithEmptyData_ShouldSucceed()
    {
        // Arrange
        var digitalSignatureService = new DigitalSignatureService();
        var originalData = new byte[0];

        // Act
        var signature = digitalSignatureService.SignData(originalData);
        var isVerified = digitalSignatureService.VerifyData(originalData, signature);

        // Assert
        Assert.True(isVerified);
    }

    [Fact]
    public void SignDataAndVerifyData_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        var digitalSignatureService = new DigitalSignatureService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            digitalSignatureService.SignData(null);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            digitalSignatureService.VerifyData(null, new byte[0]);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            digitalSignatureService.VerifyData(new byte[0], null);
        });
    }
}
