using System.Security.Cryptography;

public class SimplePasswordHasher {
    public string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        // Combine the salt and password
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

        // Generate the hash
        var hash = pbkdf2.GetBytes(20);

        // Combine the salt and hash
        var hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        // Convert to base64
        var savedPasswordHash = Convert.ToBase64String(hashBytes);

        return savedPasswordHash;
    }

    public bool VerifyPassword(string savedPasswordHash, string passwordToCheck)
    {
        return BCrypt.Net.BCrypt.Verify(passwordToCheck, savedPasswordHash);
    }
}