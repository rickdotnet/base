using System.Security.Cryptography;

namespace RickDotNet.Base.Utils;

public class TextUtils
{
    public static string RandomBase32(int length)
    {
        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");
        
        // length param is for encoded length
        // base32 is 5bits per char instead
        // of 8bits per byte
        var dataLength = length * 5 / 8;
        byte[] randomBytes = new byte[dataLength];
        RandomNumberGenerator.Fill(randomBytes);
        
        //var encodedLength = GetEncodedLength(dataLength);
        var base32Chars = new char[length];
        Base32.ToBase32(randomBytes, base32Chars);
        
        return new string(base32Chars);
    }
}
