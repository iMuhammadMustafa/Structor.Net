using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Structor.Auth.Helpers;
public static class AuthenticationUtils
{
    private const int IterationsCount = 10000;
    private const int SaltSize = 16; // 128 bit 
    private const int KeySize = 32; // 256 bit


    public static string GenerateRandomRefresh(int length = 250, bool hasCapitalLetters = true, bool hasNumbers = true, bool hasSymbols = true)
    {
        string validChars = "abcdefghijklmnopqrstuvwxyz";
        string capitalLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string symbols = "!@#$%^&*?_-";


        if (hasCapitalLetters)
        {
            validChars += capitalLetters;
        }
        if (hasNumbers)
        {
            validChars += numbers;
        }
        if (hasSymbols)
        {
            validChars += symbols;
        }

        Random random = new Random();
        char[] chars = new char[length];



        // Set the initial character types to false
        bool hasNumber = false;
        bool hasSymbol = false;
        bool hasCapitalLetter = false;
        for (int i = 0; i < length; i++)
        {
            char c = validChars[random.Next(0, validChars.Length)];
            chars[i] = c;

            if (!hasNumber & char.IsDigit(c)) hasNumber = true;
            if (!hasSymbol & symbols.Contains(c)) hasSymbol = true;
            if (!hasCapitalLetter & char.IsUpper(c)) hasCapitalLetter = true;

        }
        if (hasNumbers && !hasNumber)
        {
            // Replace a random character with a number
            chars[random.Next(0, length)] = numbers[random.Next(0, numbers.Length)];
        }
        if (hasSymbols && !hasSymbol)
        {
            // Replace a random character with a symbol
            chars[random.Next(0, length)] = symbols[random.Next(0, symbols.Length)];
        }
        if (hasCapitalLetters && !hasCapitalLetter)
        {
            // Replace a random character with a capital letter
            chars[random.Next(0, length)] = capitalLetters[random.Next(0, capitalLetters.Length)];
        }

        return new string(chars);
    }
    public static string GenerateRandomString(int length = 250, bool hasCapitalLetters = true, bool hasNumbers = true, bool hasSymbols = true)
    {
        string validChars = "abcdefghijklmnopqrstuvwxyz";
        string capitalLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        string numbers = "0123456789";
        string symbols = "!@#$%^&*?_-";


        if (hasCapitalLetters)
        {
            validChars += capitalLetters;
        }
        if (hasNumbers)
        {
            validChars += numbers;
        }
        if (hasSymbols)
        {
            validChars += symbols;
        }

        Random random = new Random();
        char[] chars = new char[length];



        // Set the initial character types to false
        bool hasNumber = false;
        bool hasSymbol = false;
        bool hasCapitalLetter = false;
        for (int i = 0; i < length; i++)
        {
            char c = validChars[random.Next(0, validChars.Length)];
            chars[i] = c;

            if (!hasNumber & char.IsDigit(c)) hasNumber = true;
            if (!hasSymbol & symbols.Contains(c)) hasSymbol = true;
            if (!hasCapitalLetter & char.IsUpper(c)) hasCapitalLetter = true;

        }
        if (hasNumbers && !hasNumber)
        {
            // Replace a random character with a number
            chars[random.Next(0, length)] = numbers[random.Next(0, numbers.Length)];
        }
        if (hasSymbols && !hasSymbol)
        {
            // Replace a random character with a symbol
            chars[random.Next(0, length)] = symbols[random.Next(0, symbols.Length)];
        }
        if (hasCapitalLetters && !hasCapitalLetter)
        {
            // Replace a random character with a capital letter
            chars[random.Next(0, length)] = capitalLetters[random.Next(0, capitalLetters.Length)];
        }

        return new string(chars);
    }

    public static string HashString(this string password, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512, int iterCount = IterationsCount, int saltSize = SaltSize, int keySize = KeySize)
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        var hashedBytes = HashedBytes(password, rng, prf, iterCount, saltSize, keySize);

        return Convert.ToBase64String(hashedBytes);

    }
    private static byte[] HashedBytes(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int keySize)
    {
        byte[] salt = new byte[saltSize];
        rng.GetBytes(salt);
        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, keySize);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // format marker
        WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
        return outputBytes;
    }
    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }


    public static bool DoesMatchHash(this string providedPassword, string hashedPassword)
    {
        byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

        if (decodedHashedPassword.Length == 0)
        {
            return false;
        }

        try
        {
            // Read header information
            KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
            int iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
            int saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < SaltSize)
            {
                return false;
            }

            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            int subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < SaltSize)
            {
                return false;
            }
            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }
    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return (uint)buffer[offset + 0] << 24
            | (uint)buffer[offset + 1] << 16
            | (uint)buffer[offset + 2] << 8
            | buffer[offset + 3];
    }

}
