namespace Project.auth.firestorerepository;

public class FirestoreUserAccount {
    private readonly string _encodedPassword;

    private readonly string _username;

    public FirestoreUserAccount(string username, string encodedPassword)
    {
        _username = username;
        _encodedPassword = encodedPassword;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        var other = (FirestoreUserAccount)obj;
        return _username == other._username && _encodedPassword == other._encodedPassword;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_username, _encodedPassword);
    }

    public override string ToString()
    {
        return $"FirestoreUserAccount{{username='{_username}', encodedPassword='{_encodedPassword}'}}";
    }

    public string getUsername()
    {
        return _username;
    }

    public string getEncodedPassword()
    {
        return _encodedPassword;
    }
}