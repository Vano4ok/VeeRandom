using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace VeeRandom.Server.Services;

public class DigitalSignatureService
{
    private const int DefaultOffset = 0;

    private readonly DSA _dsa = DSA.Create();

    public byte[] SignData(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        int offset = DefaultOffset;
        int count = data.Length - offset;
        var hashAlgorithm = HashAlgorithmName.SHA256;

        return _dsa.SignData(data, offset, count, hashAlgorithm);
    }

    public bool VerifyData(byte[] data, byte[] signature)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(signature);

        int offset = DefaultOffset;
        int count = data.Length - offset;
        var hashAlgorithm = HashAlgorithmName.SHA256;

        return _dsa.VerifyData(data, offset, count, signature, hashAlgorithm);
    }
}
