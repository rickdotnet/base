using System.Security.Cryptography;
using System.Text.Json;

namespace RickDotNet.Base.Utils;

public class RickUtils
{
    /// <summary>
    /// Converts a byte array to a Base32-encoded string.
    /// </summary>
    /// <param name="data">The byte array to encode.</param>
    /// <returns>A Base32-encoded string.</returns>
    public static string ToBase32(ReadOnlySpan<byte> data) 
        => Base32.ToBase32(data);

    /// <summary>
    /// Generates a random Base32-encoded string of the specified length.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the length is less than or equal to zero.</exception>
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
        
        var base32Chars = new char[length];
        Base32.ToBase32(randomBytes, base32Chars);
        
        return new string(base32Chars);
    }
    
    public static byte[] Serialize<T>(T data) 
        => JsonSerializer.SerializeToUtf8Bytes(data);

    /// <summary>
    /// Computes a Base32-encoded SHA256 hash of the given data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <typeparam name="T">T</typeparam>
    /// <returns>A Base32-encoded string representing the SHA256 hash of the data.</returns>
    /// <remarks>Data must be Json serializable</remarks>
    public static string Hash<T>(T data)
        where T : notnull
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(data);
        var hashBytes = SHA256.HashData(bytes);
        return Base32.ToBase32(hashBytes);
    }
}
