using VeeRandom.Generator;

namespace VeeRandom.RC5;

public class VeeRC5
{
    private const int WordSize = 32;
    private const int Rounds = 8;
    private const int KeySize = 16;
    private const int BlockSize = 8; // in bytes

    private readonly IGenerator _generator;
    private uint[] S;

    public VeeRC5(byte[] key, IGenerator generator)
    {
        if (key.Length != KeySize)
        {
            throw new ArgumentException("Key size must be 16 bytes.");
        }

        _generator = generator;

        Initialize(key);
    }

    private void Initialize(byte[] key)
    {
        int c = key.Length / (WordSize / 8);
        int t = 2 * (Rounds + 1);

        S = new uint[t];

        uint Pw = 0xb7e15163;
        uint Qw = 0x9e3779b9;

        S[0] = Pw;
        for (int kk = 1; kk < t; kk++)
        {
            S[kk] = S[kk - 1] + Qw;
        }

        int iA = 0;
        int iB = 0;
        uint[] L = new uint[c * 2];
        for (int k = 0; k < c * 2; k++)
        {
            uint A = BitConverter.ToUInt32(key, iA);
            uint B = BitConverter.ToUInt32(key, iB);

            L[k] = A + B;
            iA = (iA + 4) % key.Length;
            iB = (iB + 4) % key.Length;
        }

        int i = 0, j = 0;
        for (int k = 0; k < 2 * Math.Max(t, c); k++)
        {
            S[i] = RotateLeft((S[i] + L[j]), 3);
            i = (i + 1) % t;
            j = (j + 1) % c;
        }
    }

    private static uint RotateLeft(uint value, int shift)
    {
        return (value << shift) | (value >> (WordSize - shift));
    }

    private static uint RotateRight(uint value, int shift)
    {
        return (value >> shift) | (value << (WordSize - shift));
    }

    private byte[] GenerateIV(int length)
    {
        byte[] iv = new byte[length];

        for (int i = 0; i < length; i++)
        {
            long randomNumber = _generator.GenerateRandomNumber();

            byte[] randomNumberBytes = BitConverter.GetBytes(randomNumber);
            iv[i] = randomNumberBytes[0]; 
        }

        return iv;
    }

    public byte[] Encrypt(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentException("Input data must be non-null.");
        }

        byte[] iv = GenerateIV(BlockSize);

        int paddedLength = (data.Length % BlockSize == 0) ? data.Length : data.Length + (BlockSize - (data.Length % BlockSize));
        byte[] paddedData = new byte[paddedLength];
        Array.Copy(data, paddedData, data.Length);

        byte[] encryptedData = new byte[paddedLength];
        byte[] previousBlock = new byte[BlockSize];
        iv.CopyTo(previousBlock, 0);

        for (int i = 0; i < paddedLength; i += BlockSize)
        {
            byte[] block = new byte[BlockSize];
            Array.Copy(paddedData, i, block, 0, BlockSize);

            for (int j = 0; j < BlockSize; j++)
            {
                block[j] ^= previousBlock[j];
            }

            byte[] encryptedBlock = EncryptBlock(block);

            encryptedBlock.CopyTo(previousBlock, 0);

            encryptedBlock.CopyTo(encryptedData, i);
        }

        byte[] result = new byte[iv.Length + encryptedData.Length];
        iv.CopyTo(result, 0);
        encryptedData.CopyTo(result, iv.Length);

        return result;
    }

    public byte[] Decrypt(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentException("Input data must be non-null.");
        }

        if (data.Length < BlockSize)
        {
            throw new ArgumentException("Input data is too short to contain an IV.");
        }

        byte[] iv = new byte[BlockSize];
        Array.Copy(data, iv, BlockSize);

        byte[] encryptedData = new byte[data.Length - BlockSize];
        Array.Copy(data, BlockSize, encryptedData, 0, encryptedData.Length);

        byte[] decryptedData = new byte[encryptedData.Length];
        byte[] previousBlock = new byte[BlockSize];
        iv.CopyTo(previousBlock, 0);

        for (int i = 0; i < encryptedData.Length; i += BlockSize)
        {
            byte[] block = new byte[BlockSize];
            Array.Copy(encryptedData, i, block, 0, BlockSize);

            byte[] decryptedBlock = DecryptBlock(block);

            for (int j = 0; j < BlockSize; j++)
            {
                decryptedBlock[j] ^= previousBlock[j];
            }

            block.CopyTo(previousBlock, 0);

            decryptedBlock.CopyTo(decryptedData, i);
        }

        return decryptedData;
    }

    private byte[] EncryptBlock(byte[] block)
    {
        uint A = BitConverter.ToUInt32(block, 0);
        uint B = BitConverter.ToUInt32(block, 4);

        A += S[0];
        B += S[1];

        for (int i = 1; i <= Rounds; i++)
        {
            A = RotateLeft(A ^ B, (int)B) + S[2 * i];
            B = RotateLeft(B ^ A, (int)A) + S[2 * i + 1];
        }

        byte[] encryptedBlock = new byte[BlockSize];
        BitConverter.GetBytes(A).CopyTo(encryptedBlock, 0);
        BitConverter.GetBytes(B).CopyTo(encryptedBlock, 4);

        return encryptedBlock;
    }

    private byte[] DecryptBlock(byte[] block)
    {
        uint A = BitConverter.ToUInt32(block, 0);
        uint B = BitConverter.ToUInt32(block, 4);

        for (int i = Rounds; i > 0; i--)
        {
            B = RotateRight(B - S[2 * i + 1], (int)A) ^ A;
            A = RotateRight(A - S[2 * i], (int)B) ^ B;
        }

        B -= S[1];
        A -= S[0];

        byte[] decryptedBlock = new byte[BlockSize];
        BitConverter.GetBytes(A).CopyTo(decryptedBlock, 0);
        BitConverter.GetBytes(B).CopyTo(decryptedBlock, 4);

        return decryptedBlock;
    }
}
