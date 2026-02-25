using System.Collections;

public static class BitArrayUtility
{
    public static BitArray GenerateRandomBitArray(int length)
    {
        if (length <= 0) return new BitArray(0);

        int byteCount = (length + 7) / 8;
        byte[] buffer = new byte[byteCount];

        Random.Shared.NextBytes(buffer);

        BitArray bitArray = new BitArray(buffer);
        bitArray.Length = length;

        return bitArray;
    }
}
