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
        // // Extract bytes
        // byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
        //
        // // Get salt
        // byte[] salt = new byte[16];
        // Array.Copy(hashBytes, 0, salt, 0, 16);
        //
        // // Compute the hash on the password the user entered
        // var pbkdf2 = new Rfc2898DeriveBytes(passwordToCheck, salt, 10000);
        // byte[] hash = pbkdf2.GetBytes(20);
        //
        // // Compare the results
        // for (int i=0; i < 20; i++)
        // {
        //     if (hashBytes[i+16] != hash[i])
        //     {
        //         return false;
        //     }
        // }
        // return true;
    }
}