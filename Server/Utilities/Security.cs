public class Security
{
    public static List<string> Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
        return new List<string> { hashed, Convert.ToBase64String(salt) };
    }

    public static string GetHashPasswordV3(string password){
        return Convert.ToBase64String(HashPasswordV3(password));
    }
    public static byte[] HashPasswordV3(string password, 
        KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512, 
        int iterCount = 1000, 
        int saltSize = 128 / 8, 
        int numBytesRequested = 256 / 8)
    {
        // Produce a version 3 (see comment above) text hash.
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // format marker
        WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
        return outputBytes;
    }

    public static bool VerifyHashedPasswordV3(
        byte[] hashedPassword, 
        string password, 
        int iterCount = 1000, 
        KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512)
    {
        iterCount = default(int);
        prf = default(KeyDerivationPrf);

        try
        {
            // Read header information
            prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
            iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
            int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }
            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            int subkeyLength = hashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(
                hashedPassword, 
                13 + salt.Length, 
                expectedSubkey, 
                0, 
                expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(
                password, 
                salt, 
                prf, 
                iterCount, 
                subkeyLength);
            #if NETSTANDARD2_0 || NETFRAMEWORK
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            #elif NETCOREAPP
            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            #else
                #error Update target frameworks
            #endif
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
        return ((uint)(buffer[offset + 0]) << 24)
            | ((uint)(buffer[offset + 1]) << 16)
            | ((uint)(buffer[offset + 2]) << 8)
            | ((uint)(buffer[offset + 3]));
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }

    public static Token CreatePublicToken(ClientInput clientInput, IConfiguration configuration)
    {
        Token token = new();

        var Claims = new List<Claim>
        {
            new Claim("IpAddress", clientInput.IpAddress!),
            new Claim("HostName", clientInput.HostName!),
            new Claim("MacAddress", clientInput.MacAddress!),
        };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SigningKey"]!));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        token.Expiration = DateTime.Now.AddMinutes(Convert.ToInt16(configuration["Token:Expiration"]));

        JwtSecurityToken jwtSecurityToken = new(
            issuer: configuration["Token:Issuer"],
            audience: configuration["Token:Audience"],
            claims: Claims,
            expires: token.Expiration,
            signingCredentials: credentials
            );

        token.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        byte[] numbers = new byte[32];
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
        random.GetBytes(numbers);
        token.RefreshToken = Convert.ToBase64String(numbers);

        return token;
    }

    public static Token CreatePrivateToken(
        LoginInput userInput, IConfiguration configuration)
    {
        Token token = new();

        var Claims = new List<Claim>
        {
            new Claim("Username", userInput.Username!),
            new Claim("Password", userInput.Password!)
        };

        SymmetricSecurityKey securityKey = 
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Token:SigningKey"]!));
        SigningCredentials credentials = 
            new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

        token.Expiration = 
            DateTime.Now.AddMinutes(
                Convert.ToInt16(configuration["Token:Expiration"]));

        JwtSecurityToken jwtSecurityToken = new(
            issuer: configuration["Token:Issuer"],
            audience: configuration["Token:Audience"],
            claims: Claims,
            expires: token.Expiration,
            signingCredentials: credentials
            );

        token.AccessToken = 
            new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        byte[] numbers = new byte[32];
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
        random.GetBytes(numbers);
        token.RefreshToken = Convert.ToBase64String(numbers);

        return token;
    }
}
