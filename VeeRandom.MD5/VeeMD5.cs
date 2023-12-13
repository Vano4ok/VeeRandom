using System.Text;

namespace VeeRandom.MD5;

public static class VeeMD5
{
    private static readonly uint[] S =
    [
        7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
        5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
        4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
        6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21,
    ];

    private static readonly uint[] K =
    [
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391,
    ];

    private static readonly uint[] Padding =
    [
        0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    ];

    public static byte[] HashBytes(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        uint a = 0x67452301;
        uint b = 0xefcdab89;
        uint c = 0x98badcfe;
        uint d = 0x10325476;

        // Calculate the data size in bits
        ulong dataSizeBits = (ulong)data.Length * 8;

        // Append padding
        int originalLength = data.Length;
        int paddingLength = (originalLength % 64 < 56) ? (56 - originalLength % 64) : (120 - originalLength % 64);
        byte[] paddedData = new byte[originalLength + paddingLength + 8];
        Array.Copy(data, paddedData, originalLength);

        for (int i = 0; i < paddingLength; i++)
        {
            paddedData[originalLength + i] = (byte)(Padding[i] & 0xFF);
        }

        Array.Copy(BitConverter.GetBytes(dataSizeBits), 0, paddedData, originalLength + paddingLength, 8);

        // Process each 512-bit block
        for (int i = 0; i < paddedData.Length; i += 64)
        {
            uint[] words = new uint[16];
            for (int j = 0; j < 16; j++)
            {
                words[j] = BitConverter.ToUInt32(paddedData, i + j * 4);
            }

            uint AA = a;
            uint BB = b;
            uint CC = c;
            uint DD = d;

            // Main MD5 algorithm
            for (int j = 0; j < 64; j++)
            {
                uint F_result;
                int g;

                if (j < 16)
                {
                    F_result = F(BB, CC, DD);
                    g = j;
                }
                else if (j < 32)
                {
                    F_result = G(BB, CC, DD);
                    g = (5 * j + 1) % 16;
                }
                else if (j < 48)
                {
                    F_result = H(BB, CC, DD);
                    g = (3 * j + 5) % 16;
                }
                else
                {
                    F_result = I(BB, CC, DD);
                    g = (7 * j) % 16;
                }

                F_result += AA + K[j] + words[g];
                AA = DD;
                DD = CC;
                CC = BB;
                BB += RotateLeft(F_result, (int)S[j]);
            }

            a += AA;
            b += BB;
            c += CC;
            d += DD;
        }

        // Combine the four 32-bit hash values
        byte[] hashBytes = new byte[16];
        Array.Copy(BitConverter.GetBytes(a), 0, hashBytes, 0, 4);
        Array.Copy(BitConverter.GetBytes(b), 0, hashBytes, 4, 4);
        Array.Copy(BitConverter.GetBytes(c), 0, hashBytes, 8, 4);
        Array.Copy(BitConverter.GetBytes(d), 0, hashBytes, 12, 4);

        return hashBytes;
    }

    private static uint F(uint x, uint y, uint z)
    {
        return (x & y) | (~x & z);
    }

    private static uint G(uint x, uint y, uint z)
    {
        return (x & z) | (y & ~z);
    }

    private static uint H(uint x, uint y, uint z)
    {
        return x ^ y ^ z;
    }

    private static uint I(uint x, uint y, uint z)
    {
        return y ^ (x | ~z);
    }

    private static uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
    }
}